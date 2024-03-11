using System;
using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using LitJson;
using WebSocketSharp;
using UnityEngine;
using UnityEngine.Events;
using VocaModu.xfyun.util;
using WebSocket = WebSocketSharp.WebSocket;

namespace VocaModu.xfyun
{
    public class TTSConverter : MonoBehaviour
    {
        private WebSocket _webSocket;
        private bool _isConnecting;
        
        [SerializeField] [NotNull] private string url = "wss://tts-api.xfyun.cn/v2/tts";
        [SerializeField] [NotNull] private string appid = ""; 
        [SerializeField] [NotNull] private string apiSecret = "";
        [SerializeField] [NotNull] private string apiKey = "";
        
        private int _audioLength; //语音长度
        private readonly ConcurrentQueue<float> _audionQue = new(); //转后的语音队列
        private JsonDataPool _jsonDataPool;
        private AudioClip _clip;

        private string _jsonData;
        
        public UnityEvent<AudioClip> onSpeechGenerated;

        // Start is called before the first frame update
        private void Start()
        {
            _audioLength = 0;
            _audionQue.Clear();
            _jsonDataPool = new JsonDataPool(10);
        }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _webSocket = new WebSocket(Util.GetUrl(url, apiSecret, apiKey));
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnMessage += OnMessage;
            _webSocket.OnError += OnError;
            _webSocket.OnClose += OnClosed;
        }

        private void Connect()
        {
            _webSocket.ConnectAsync();
        }

        private void OnOpen(object obj, EventArgs arg)
        {
            Debug.Log("讯飞WebSocket连接成功！");
            _isConnecting = true;
            _webSocket.Send(_jsonData);
        }
        
        private void OnMessage(object sender, MessageEventArgs e)
        {
            JsonData js = JsonMapper.ToObject(e.Data);
            if (js["message"].ToString() != "success") return;
            if (js["data"]?["audio"] == null) return;
            var fs = Util.BytesToFloat(Convert.FromBase64String(js["data"]["audio"].ToString()));
            _audioLength += fs.Length;
            foreach (var data in fs)
                _audionQue.Enqueue(data);

            #region end
            if ((int)js["data"]["status"] != 2) return; //2为结束标志符
            UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                _clip =
                    AudioClip.Create("voice", _audioLength, 1, 16000, true,
                        OnAudioRead);
                onSpeechGenerated?.Invoke(_clip);
            });
            _webSocket.Close();
            #endregion
        }

        /// <summary>
        /// 采样回调
        /// </summary>
        /// <param name="data"></param>
        private void OnAudioRead(float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (_audionQue.Count > 0)
                {
                    _audionQue.TryDequeue(out var result);
                    data[i] = result;
                }
                else
                {
                    if (_webSocket is not { IsAlive: true })
                        _audioLength++;
                    data[i] = 0;
                }
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _isConnecting = false;
            Debug.Log("websocket connect err: " + e.Message);
        }

        private void OnClosed(object sender, WebSocketSharp.CloseEventArgs closeEventArgs)
        {
            _isConnecting = false;
            Debug.Log("讯飞WebSocket连接关闭！" + closeEventArgs.Reason);
        }
        
        private string CreateJsonData(string text, string vcn)
        {
            var jsonData = _jsonDataPool.Get();
            jsonData["common"] = _jsonDataPool.CommonJsonData;
            jsonData["common"]["app_id"] = appid;
            
            jsonData["business"] = _jsonDataPool.BusinessJsonData;
            jsonData["business"]["aue"] = "raw";
            jsonData["business"]["auf"] = "audio/L16;rate=16000";
            jsonData["business"]["vcn"] = vcn;
            jsonData["business"]["tte"] = "UTF8";
            
            jsonData["data"] = _jsonDataPool.DataJsonData;
            jsonData["data"]["status"] = 2;
            jsonData["data"]["text"] = Util.ToBase64String(text);

            var ret = JsonMapper.ToJson(jsonData);
            _jsonDataPool.Release(jsonData);
            return ret;
        }

        public void GenerateSpeech(string text, string vcn)
        {
            _audioLength = 0;
            _audionQue.Clear();
            _jsonData = CreateJsonData(text, vcn);
            Connect();
        }
    }
}