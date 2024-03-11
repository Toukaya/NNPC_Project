using System;
using System.Collections.Generic;
using Character.NPC;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.Serialization;

namespace LLM.Aliyun
{
    public class QwenModel : ChatModel
    {
        /// <summary>
        /// api地址
        /// </summary>
        [SerializeField] [NotNull] public string apiUrl = "https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation";

        [SerializeField] [NotNull] public string model = "qwen-v1";
        /// <summary>
        /// 缓存对话
        /// </summary>
        [SerializeField] private List<SendData> history = new();
 
        [SerializeField] [NotNull] public string apiKey = "sk-xxx";

        [SerializeField] [Range(0, 1f)] public float topP = 0.5f;
        [SerializeField] [Range(0, 100)] public int topK = 50;
        [SerializeField] public uint seed = 1234;
        [SerializeField] public bool enableSearch = false;
        
        public override void Init(string prompt)
        {
            history.Clear();
            history.Add(new SendData("system", prompt));
        }

        public override async UniTask<string> SendChatRequestAsync(string message)
        {
            //缓存发送的信息列表
            history.Add(new SendData("user", message));

            using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
     
            string jsonText = CreateJsonData();

            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonText);
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // Set the authorization header
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            // Set the content type header
            request.SetRequestHeader("Content-Type", "application/json");
            
            var op = await request.SendWebRequest();
         
            if (request.result != UnityWebRequest.Result.Success) return "";
            string msg = request.downloadHandler.text;

#if DEBUG
            Debug.Log(msg);            
#endif
            
            // 解析JSON
            var res = JsonMapper.ToObject(msg);
            string backMsg = res["output"]["text"].ToString();
            //添加记录
            history.Add(new SendData("assistant", backMsg));
            return backMsg;
        }

        public override void LoadMemory(ConversationMemory memory)
        {
            history.Add(new SendData(memory.isPlayerMessage ? "user" : "assistant", memory.ToString()));
        }

        private string CreateJsonData()
        {
            var postData = new PostData
            {
                model = model,
                input = new InputData
                {
                    messages = history
                },
                parameters = new ParametersData(topP, topK,  (int)seed, enableSearch)
            };
            
            var jsonText = JsonUtility.ToJson(postData);
#if DEBUG
            Debug.Log(jsonText);
#endif
            return jsonText;
        }
        
        #region DataPackage

        [Serializable] public class PostData
        {
            public string model;
            public InputData input;
            public ParametersData parameters;
        }
        
        [Serializable] public class InputData
        {
            public List<SendData> messages;
        }

        [Serializable] public class ParametersData
        {
            public float top_p;
            public int top_k;
            public int seed;
            public bool enable_search;

            public ParametersData(float topP, int topK, int seed, bool enableSearch)
            {
                top_p = topP;
                top_k = topK;
                this.seed = seed;
                enable_search = enableSearch;
            }
        }   
    
        [Serializable] public class SendData
        {
            public string role;
            public string content;
            public SendData() { }
            public SendData(string role,string content) {
                this.role = role;
                this.content = content;
            }
        }
        #endregion 
        
    }
    
}