using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
// using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GUIModu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_ANDROID 
using UnityEngine.Android;
#endif

namespace VocaModu
{
    public enum Code {
        Success = 0,
        Failed = -1,

        // 达到最大录音时长
        MaxDurationReached = 1,  

        // 麦克风设备不可用
        MicNotAvailable = 2,

        // 未知错误
        Unknown = 100
    }
    
    public class Recorder : MonoBehaviour
    {
        private static Recorder _instance = null;
        
        [SerializeField] private string deviceNameMic = null;

        [SerializeField, Range(1, 60)] 
        private int maxRecordingDuration = 15;

        private CancellationTokenSource _cts;
        [SerializeField] private int frequency = 16000;

        //获取录音状态
        private bool IsRecording { get; set; } = false;

        public AudioClip recordedClip;
        
        public UnityEvent onRecordingStarted;
        public UnityEvent onRecordingStopped;
        public UnityEvent<Code> onRecordingInterrupted;
        public UnityEvent<byte[]> onAudioSegmentSent;

        private void Awake()
        {
            if (_instance != null) return;
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
#if UNITY_ANDROID
            if (Permission.HasUserAuthorizedPermission(Permission.Microphone)) return;
            Permission.RequestUserPermission(Permission.Microphone);
#endif
        }

        public static Recorder Instance() {
            if (!Exists ()) {
                throw new Exception ("Recorder could not find the Recorder object. Please ensure you have added the Recorder Prefab to your scene.");
            }
            return _instance;
        }
        
        public static bool Exists() {
            return _instance != null;
        }

        public void StartRecording()
        {
            TryEnableMicrophoneAsync().Forget();
        }

        private async UniTask SendAudioClipAsync()
        {
            await UniTask.WaitUntil(() => Microphone.GetPosition(deviceNameMic) <= 0);
            var pos = Microphone.GetPosition(deviceNameMic);
            const int maxlength = 1280; //最多发送1280字节
            int lastPos = 0;
            
            while (IsRecording && pos < recordedClip.samples)
            {
                await UniTask.DelayFrame(1);
                
                if (Microphone.IsRecording(deviceNameMic))
                {
                    pos = Microphone.GetPosition(deviceNameMic);
                }
                if (pos <= lastPos)
                {
                    // 防止出现当前采样位置和上一帧采样位置一样，导致length为0
                    // 那么在调用AudioClip.GetData(float[] data, int offsetSamples);时，将报错
                    continue;
                }
                
                int length = pos - lastPos > maxlength ? maxlength : pos - lastPos;
                byte[] data = GetAudioClip(lastPos, length, recordedClip);
                
                onAudioSegmentSent?.Invoke(data);
                lastPos += length;
            }
        }

        private async UniTask TryEnableMicrophoneAsync()
        {
            await UniTask.WaitUntil(() => !IsRecording);
            // 尝试启动录音
            recordedClip = Microphone.Start(deviceNameMic, false, 60, frequency);
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                DialogSystem.Instance().Active("No microphone input detected", "System", Color.red);
                Microphone.End(deviceNameMic);
                onRecordingInterrupted?.Invoke(Code.MicNotAvailable);
                return;
            }
#else
            if(Microphone.devices.Length == 0) {
                DialogSystem.Instance().Active("No microphone input detected", "System", Color.red);
                Microphone.End(deviceNameMic);
                onRecordingInterrupted?.Invoke(Code.MicNotAvailable);
                return; 
            }
#endif
            IsRecording = true;
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            CalRecordingDuration().Forget();
            SendAudioClipAsync().Forget();
            onRecordingStarted?.Invoke();
        }

        private async UniTask CalRecordingDuration()
        {
            if (!IsRecording) return;
            var isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(maxRecordingDuration), cancellationToken: _cts.Token).SuppressCancellationThrow();
            if (isCanceled) return;
#if DEBUG
            Debug.Log("end of recording due to timeout.");
#endif
            onRecordingInterrupted?.Invoke(Code.MaxDurationReached);
            StopRecording();
        }

        public void StopRecording()
        {
            HandleStopASync().Forget();
        }

        private async UniTask HandleStopASync()
        {
            if (!IsRecording) return;
            _cts.Cancel();
            await UniTask.Delay(TimeSpan.FromSeconds(.8f));
            IsRecording = false;
            Microphone.End(deviceNameMic);
            onRecordingStopped?.Invoke();
        }

        private IEnumerator HandleStop()
        {
            if (!IsRecording) yield break;
            yield return new WaitForSeconds(.8f);
            IsRecording = false;
            Microphone.End(deviceNameMic);
            onRecordingStopped?.Invoke();
        } 
        
        /// <summary>
        /// 获取音频流片段
        /// </summary>
        /// <param name="start">起始采样点</param>
        /// <param name="length">采样长度</param>
        /// <param name="recordedClip">音频</param>
        /// <returns></returns>
        public static byte[] GetAudioClip(int start, int length, AudioClip recordedClip)
        {
            float[] soundata = new float[length];
            recordedClip.GetData(soundata, start);
            int rescaleFactor = 32767;
            byte[] outData = new byte[soundata.Length * 2];
            for (int i = 0; i < soundata.Length; i++)
            {
                short temshort = (short) (soundata[i] * rescaleFactor);
                byte[] temdata = BitConverter.GetBytes(temshort);
                outData[i * 2] = temdata[0];
                outData[i * 2 + 1] = temdata[1];
            }

            return outData;
        }

        private byte[] AudioClipDataToByteArray(IReadOnlyList<float> data)
        {
            int bytesPerSecond = frequency * 2; // 一个sample占2个byte
            float totalSeconds = data.Count / (float)frequency; 

            MemoryStream dataStream = new MemoryStream();
            
            int x = sizeof(short);
            const Int16 maxValue = short.MaxValue;
            int bytesToWrite = (int)(totalSeconds * bytesPerSecond); // 根据时间和采样率计算总字节数

            for (int i = 0; i < bytesToWrite; i += x) {
                float sample = data[i/x] * maxValue; // 取出sample并转换范围
                byte[] bytes = BitConverter.GetBytes(Convert.ToInt16(sample));
                dataStream.Write(bytes, 0, x); // 写入这一个sample的字节数据
            }

            byte[] result =  dataStream.ToArray();

            dataStream.Dispose();
            return result;
        }

        private void OnDestroy() {
            _instance = null;
            _cts.Dispose();
        }
        
    }
}