using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using GUIModu;
using JetBrains.Annotations;
using LLM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character.NPC
{
    [RequireComponent(typeof(AudioSource), typeof(AnimatorControl))]
    public class NpcController : MonoBehaviour
    {
        [SerializeField] private NpcInfo info;

        private NPCAction _npcAction;
        
        private AudioSource _audioSource;

        private AnimatorControl _animatorControl;
        
        [SerializeField] [NotNull] private ChatModel chatModel;

        private bool _isSpeaking;
        
        public UnityEvent<string, string> onChatResponseEvent;

        private void Start()
        {
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
            _isSpeaking = false;
            _audioSource.Stop();
            ChatInit();
        }

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _animatorControl = GetComponent<AnimatorControl>();
        }

        private void ChatInit()
        {
            chatModel.Init(CreatePrompt());
            foreach (var conversationMemory in info.mainMemory)
                chatModel.LoadMemory(conversationMemory);
            foreach (var conversationMemory in info.GetMemory())
                chatModel.LoadMemory(conversationMemory);
        }

        public void Chat(string content)
        {
            ChatAsync(content).Forget();
        }

        private async UniTask ChatAsync(string content)
        {
            await UniTask.SwitchToMainThread();
            // 停止当前正在播放的音频
            if (_audioSource.isPlaying)
                _audioSource.Stop();
            
            _animatorControl.newAnimation = "thinking";
            var res = await chatModel.SendChatRequestAsync(content);
            
            //保存这次对话到记忆
            var conversation = new ConversationMemory
            {
                isPlayerMessage = true,
                content = content,
                date = DateTime.Now.ToString("yyyy年MM月dd日HH时"),
            };
            info.RecordMemory(conversation);
            HandleChatResponse(res);
        }
        
        public async UniTask ChatAsync(string content, DateTime date, string address)
        {
            await UniTask.SwitchToMainThread();
            // 停止当前正在播放的音频
            if (_audioSource.isPlaying)
                _audioSource.Stop();
            
            _animatorControl.newAnimation = "thinking";
            var res = await chatModel.SendChatRequestAsync(content);
            HandleChatResponse(res, address);
            
            //保存这次对话到记忆
            var conversation = new ConversationMemory
            {
                isPlayerMessage = true,
                content = "Player: " + content,
                date = date.ToString("yyyy年MM月dd日HH时"),
                address = address
            };
            info.RecordMemory(conversation);
        }

        
        /// <summary>
        /// WARNING: Make sure it run on MAIN Thread
        /// </summary>
        /// <param name="content"></param>
        /// <param name="date"></param>
        /// <param name="address"></param>
        public void Chat(string content, DateTime date, string address)
        {
            // 停止当前正在播放的音频
            if (_audioSource.isPlaying)
                _audioSource.Stop();
            
            UniTask.Create(async () =>
            {
                _animatorControl.newAnimation = "thinking";
                var res = await chatModel.SendChatRequestAsync(content);
                HandleChatResponse(res, address);
            });
            // StartCoroutine(_chat.SendChatRequest(content, s => HandleChatResponse(s, address)));

            //保存这次对话到记忆
            var conversation = new ConversationMemory
            {
                isPlayerMessage = true,
                content = "Player: " + content,
                date = date.ToString("yyyy年MM月dd日HH时"),
                address = address
            };
            info.RecordMemory(conversation);
        }

        private string CreatePrompt()
        {
            return info.GetPrompt();
        }

        private void HandleChatResponse(string response, string address)
        {
            _npcAction = ParseResponse(response);
            if (_npcAction.Text == null)
            {
                DialogSystem.Instance().Active("看来出现了一些错误，请稍后再试。", "System", Color.red);
                _animatorControl.newAnimation = info.npcActions[0];
                _animatorControl.expression = info.npcEmojis[0];
                return;
            }

            onChatResponseEvent?.Invoke(_npcAction.Text, info.npcVoice);

            //保存这次对话到记忆
            var conversation = new ConversationMemory
            {
                isPlayerMessage = false,
                content = response,
                date = DateTime.Now.ToString("yyyy年MM月dd日HH时"),
                address = address
            };
            info.RecordMemory(conversation);
        }
        private void HandleChatResponse(string response)
        {
            _npcAction = ParseResponse(response);
            if (_npcAction.Text == null)
            {
                DialogSystem.Instance().Active("看来出现了一些错误，请稍后再试。", "System", Color.red);
                _animatorControl.newAnimation = info.npcActions[0];
                _animatorControl.expression = info.npcEmojis[0];
                return;
            }

            onChatResponseEvent?.Invoke(_npcAction.Text, info.npcVoice);

            //保存这次对话到记忆
            var conversation = new ConversationMemory
            {
                isPlayerMessage = false,
                content = response,
                date = DateTime.Now.ToString("yyyy年MM月dd日HH时"),
            };
            info.RecordMemory(conversation);
        }

        private NPCAction ParseResponse(string response)
        {
            Debug.Log(response);
            var res = JsonMapper.ToObject(response);

            if (res == null || string.IsNullOrEmpty(res["text"].ToString()))
            {
                Debug.Log("Invalid chat response");
                return new NPCAction(null, null, null);
            }

            string emotion = res["emotion"].ToString();
            string action = res["action"].ToString();
            string text = res["text"].ToString();

            Debug.Log("Emotion: " + emotion);
            Debug.Log("Action: " + action);
            Debug.Log("Text: " + text);

            return new NPCAction(text, action != "" ? action : "idle", emotion != "" ? emotion : "idle");
        }
        
        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!_isSpeaking) return;
            
            var volume = data.Sum(Mathf.Abs);
            volume /= data.Length;
            volume /= 0.2f;
            volume = Mathf.Clamp01(volume);
            
            _animatorControl.Speak(volume);
        }

        public void PlayVoiceAndAction(AudioClip clip)
        {
            _audioSource.clip = clip;
            DialogSystem.Instance().Active(_npcAction.Text, info.npcName);
            _animatorControl.newAnimation = _npcAction.Action.ToLower();
            _animatorControl.expression = _npcAction.Emotion.ToLower();
            StartCoroutine(PlayVoice(clip.length));
        }

        private IEnumerator PlayVoice(float sec)
        {
            _audioSource.Play();
            _isSpeaking = true;
            yield return new WaitForSeconds(sec);
            _audioSource.Stop();
            _isSpeaking = false;
        }
        
    }
}