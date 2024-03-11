using System;
using System.Collections;
using System.Collections.Generic;
using Character.NPC;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace LLM.OpenAI
{
    public class ChatGPTScript : ChatModel
    {
        /// <summary>
        /// api地址
        /// </summary>
        [SerializeField] [NotNull] public string apiUrl = "https://api.openai.com/v1/chat/completions";
        /// <summary>
        /// gpt-3.5-turbo
        /// </summary>
        [SerializeField] [NotNull] public string gptModel = "gpt-3.5-turbo";
        /// <summary>
        /// 缓存对话
        /// </summary>
        [SerializeField] private List<SendData> dataList = new();
        /// <summary>
        ///
        /// </summary> 
        [SerializeField] [NotNull] public string openAIKey = "sk-xxx";

        public override void Init(string prompt)
        {
            ClearCached();
            dataList.Add(new SendData("system", prompt));
        }

        public override void LoadMemory(ConversationMemory memory)
        {
            dataList.Add(new SendData(memory.isPlayerMessage ? "user" : "assistant", memory.ToString()));
        }

        public void ClearCached()
        {
            dataList.Clear();
        }

        public override async UniTask<string> SendChatRequestAsync(string message)
        {
            //缓存发送的信息列表
            dataList.Add(new SendData("user", message));

            using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            PostData postData = new PostData
            {
                model = gptModel,
                messages = dataList
            };
            
            dataList.ForEach(x => Debug.Log($"msg: {x.role}-{x.content}"));
     
            string jsonText = JsonUtility.ToJson(postData);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonText);
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
     
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {openAIKey}");
            
            var op = await request.SendWebRequest();
         
            if (request.responseCode != 200) return "";
            string msg = request.downloadHandler.text;
#if DEBUG
            Debug.Log(msg);            
#endif
            MessageBack textBack = JsonUtility.FromJson<MessageBack>(msg);
            if (textBack == null || textBack.choices.Count <= 0) return "";
            string backMsg = textBack.choices[0].message.content;
            //添加记录
            dataList.Add(new SendData("assistant", backMsg));
            return backMsg;
        }

        public IEnumerator SendChatRequest(string message, Action<string> callback)
        {
            //缓存发送的信息列表
            dataList.Add(new SendData("user", message));
            Debug.Log(string.Join("\n", dataList));

            using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            PostData postData = new PostData
            {
                model = gptModel,
                messages = dataList
            };
     
            string jsonText = JsonUtility.ToJson(postData);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonText);
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
     
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {openAIKey}");
     
            yield return request.SendWebRequest(); 

            // Debug.Log("CODE: " + request.responseCode);
            if (request.responseCode != 200) yield break;
            string msg = request.downloadHandler.text;
            MessageBack textBack = JsonUtility.FromJson<MessageBack>(msg);
            if (textBack == null || textBack.choices.Count <= 0) yield break;
            string backMsg = textBack.choices[0].message.content;
            //添加记录
            dataList.Add(new SendData("assistant", backMsg));
            // Debug.Log("MESSAGE: " + backMsg);
            callback(backMsg);
        }
        
        #region DataPackage
     
        [Serializable] public class PostData
        {
            public string model;
            public List<SendData> messages;
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
        [Serializable]
        public class MessageBack
        {
            public string id;
            public string created;
            public string model;
            public List<MessageBody> choices;
        }
        [Serializable]
        public class MessageBody
        {
            public Message message;
            public string finishReason;
            public string index;
        }
        [Serializable]
        public class Message
        {
            public string role;
            public string content;
        }
     
        #endregion
    }
}