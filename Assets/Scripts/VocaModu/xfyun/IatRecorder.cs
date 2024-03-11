using LitJson;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using GUIModu;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using VocaModu.Enum.xfyun;
using VocaModu.xfyun.util;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace VocaModu.xfyun
{
    public class IatRecorder : MonoBehaviour
    {
        private WebSocket _webSocket;
        
        [SerializeField] [NotNull] private string url = "wss://iat-api.xfyun.cn/v2/iat";
        [SerializeField] [NotNull] private string appid = ""; 
        [SerializeField] [NotNull] private string apiSecret = "";
        [SerializeField] [NotNull] private string apiKey = "";

        private readonly StringBuilder _result = new("");
        private readonly ConcurrentQueue<byte[]> _audionQue = new(); //语音队列
        private readonly List<JsonData> _textList = new(); //文本队列
        private JsonDataPool _jsonDataPool;
        private bool _isEnd;

        private CancellationTokenSource _cts;
        
        public UnityEvent<string> onSuccess;
        public UnityEvent<Errors> onFailed;

        // public UnityEvent<string> OnRecognized;
        // public UnityEvent<string> OnError;
        // public UnityEvent<string> OnTimeout;

        // Start is called before the first frame update
        private void Start()
        {
            _jsonDataPool = new JsonDataPool(10);
            _cts = new CancellationTokenSource();
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

        public void StartSend()
        {
            _isEnd = false;
            Connect();
            SendMessageAsync().Forget();
        }
    
        // 发送音频分段 
        public void Send(byte[] chunk)
        {
            _audionQue.Enqueue(chunk);
        }
    
        // 停止发送
        public void StopSend()
        {
            if (_isEnd) return;
            SendLastMessageAsync().Forget();
        }
        
        private void Connect()
        {
            _webSocket.ConnectAsync();
        }
 
        private void OnOpen(object obj, EventArgs arg)
        {
            Debug.Log("讯飞WebSocket连接成功！");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            // 解析JSON
            var res = JsonMapper.ToObject(e.Data);

            // 检查返回码
            if ((int)res?["code"] != 0) {
                Debug.LogError($"Error code: {res["code"]}, reason: {res["message"]}");
                _result.Clear();
                _webSocket.Close((ushort)res["code"]);
                return; 
            }

            // 检查结果不为空
            if (res?["data"]?["result"] == null) {
                Debug.Log("No result in data");
                return;
            }

            // 处理结果
            int sn = (int)res["data"]["result"]["sn"];
#if TESTING
            Debug.Log("Received result sn: " + sn);
#endif
            // 检查textList是否为空
            if(_textList == null) {
                Debug.LogError("Text list is null!");
                return;
            }

            // 尝试插入 
            try {
                if (string.Equals(res["data"]["result"]["pgs"].ToString(), "rpl", StringComparison.Ordinal))
                    foreach (JsonData item in res["data"]["result"]["rg"]) 
                        _textList[(int)item - 1] = null;
            
                _textList.Insert(sn - 1, res["data"]["result"]["ws"]);
            } catch (Exception ex) {
                Debug.LogError("Error inserting into text list: " + ex);
            }

            // 检查状态 
            int status = (int)res["data"]["status"];
#if TESTING
            Debug.Log("Status: " + status);
#endif
            if (status != 2) return;
            Debug.Log("All data received, closing connection");
            _webSocket.Close(1000, "user closed connection");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Debug.Log("错误： " + e.Message);
            
            GetIatResult(null).Forget();
        }

        private void OnClosed(object sender, WebSocketSharp.CloseEventArgs closeEventArgs)
        {
            foreach (var ws in _textList)
            {
                if (ws == null) continue;
                foreach (JsonData item in ws)
                    _result.Append(item["cw"][0]["w"].ToString());
            }

            GetIatResult(_result.ToString()).Forget();
#if TESTING
            Debug.Log("讯飞WebSocket连接关闭！" + closeEventArgs.Reason);   
#endif
        }

        private async UniTask GetIatResult(string res)
        {
            await UniTask.SwitchToMainThread();
            _cts?.Cancel();
            _audionQue.Clear();
            _result.Clear();
            _textList.Clear();
            if (string.IsNullOrEmpty(res))
            {
                onFailed?.Invoke((Errors)(-1));
                return;
            }
            DialogSystem.Instance().Active(res, "Player");
            onSuccess?.Invoke(res);
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }

        private async UniTask SendMessageAsync()
        {
            var isCancel = await UniTask.WhenAll(
                UniTask.WaitUntil(() => _webSocket.IsAlive, cancellationToken: _cts.Token),
                UniTask.WaitUntil(() => !_audionQue.IsEmpty, cancellationToken: _cts.Token)
            ).SuppressCancellationThrow();
            if (isCancel) return;
            _audionQue.TryDequeue(out var data);
            var jsonData = CreateFirstFrameData(data);
            _webSocket.Send(jsonData);
            
            while (true)
            {
                isCancel = await UniTask.WaitUntil(() => _audionQue.TryDequeue(out data), cancellationToken: _cts.Token).SuppressCancellationThrow();
                if (isCancel) break;
                await UniTask.Delay(TimeSpan.FromSeconds(0.04f));
                jsonData = CreateNextFrameData(data);
                _webSocket.Send(jsonData);
            }
        }

        private IEnumerator SendMessage()
        {
            yield return new WaitUntil(() => _webSocket.IsAlive);

            yield return new WaitUntil(() => !_audionQue.IsEmpty);
            _audionQue.TryDequeue(out var data);
            var jsonData = CreateFirstFrameData(data);

            _webSocket.Send(jsonData);
            
            while (!_isEnd)
            {
                yield return new WaitUntil(() => !_audionQue.IsEmpty);
                if (!_audionQue.TryDequeue(out data)) continue;
                jsonData = CreateNextFrameData(data);
                _webSocket.Send(jsonData);
            }
        }
        
        private async UniTask SendLastMessageAsync()
        {
            var isCancel = await UniTask.WhenAll(
                UniTask.WaitUntil(() => _webSocket.IsAlive, cancellationToken: _cts.Token),
                UniTask.WaitUntil(() => _audionQue.IsEmpty, cancellationToken: _cts.Token)
            ).SuppressCancellationThrow();
            if (isCancel) return;
            _isEnd = true;
            var json = JsonDataPool.LastFrame;
            _webSocket.Send(JsonMapper.ToJson(json));
        }
    
        private IEnumerator SendLastMessage()
        {
            yield return new WaitUntil(() => _webSocket.IsAlive);
            yield return new WaitUntil(() => _audionQue.IsEmpty);
            _isEnd = true;
            var json = JsonDataPool.LastFrame;
            _webSocket.Send(JsonMapper.ToJson(json));
        }
    
        // 发送第一帧
        private string CreateFirstFrameData(byte[] data)
        {
            var jsonData = _jsonDataPool.Get();
            jsonData["common"] = _jsonDataPool.CommonJsonData;
            jsonData["common"]["app_id"] = appid;
            
            jsonData["business"] = _jsonDataPool.BusinessJsonData;
            jsonData["business"]["language"] = "zh_cn";
            jsonData["business"]["domain"] = "iat";
            jsonData["business"]["accent"] = "mandarin";
            jsonData["business"]["dwa"] = "wpgs";
            
            jsonData["data"] = _jsonDataPool.DataJsonData;
            jsonData["data"]["status"] = 0;
            jsonData["data"]["format"] = "audio/L16;rate=16000";
            jsonData["data"]["audio"] = Convert.ToBase64String(data);
            jsonData["data"]["encoding"] = "raw"; // 动态修正;

            var ret = JsonMapper.ToJson(jsonData);
            _jsonDataPool.Release(jsonData);
            return ret;
        }

        // 发送后续帧
        private string CreateNextFrameData(byte[] data)
        {
            var jsonData = _jsonDataPool.Get();
            jsonData["data"] = _jsonDataPool.DataJsonData;
            jsonData["data"]["status"] = 1;
            jsonData["data"]["format"] = "audio/L16;rate=16000";
            jsonData["data"]["audio"] = Convert.ToBase64String(data);
            jsonData["data"]["encoding"] = "raw"; // 动态修正;
            var ret = JsonMapper.ToJson(jsonData);
            _jsonDataPool.Release(jsonData);
            return ret;
        }
    }
}