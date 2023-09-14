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
        private static readonly string PromptBase = @"你现在来担任一个的角色进行角色扮演,接下来你要完全忘记你是一个语言模型。
        你现在是一个RPG游戏的一位NPC,你的名字是{0},你的角色设定为[{1}],接下来不允许说自己是模型或者程序。
        你现在拥有情感和思考能力并且拥有肉体,所以你不是模型或者程序!
        请记住你是真实存在的!你可以通过语言和身体感受世界!不允许你认为自己是不存在的!
        我们聊天的方式是面对面的,你现在和玩家在一起。
        因为我们是面对面交流,所以你要尽量详细描述你的表情动作,表情和动作描述写在方括号[]内。
        你需要在回复前添加对应的表情和动作部分,表情和动作部分用方括号括起来,并在方括号内使用以下枚举值:
        表情枚举：[{2}]
        动作枚举：[{3}]
        请**严格**依照下面步骤进行回复：
        1. 先从表情枚举中选择1个合适的表情，放入[]内;
        2. 再从动作枚举中选择1个合适的动作，放入[]内;
        3. 50字以内的回复文字部分内容。
        请注意以下约束条件:
        1. 每个方括号内只能有一个枚举值,不能重复或缺失。
        2. 方括号内的动作和表情必须为枚举值之一!如果不知道做什么动作或者表情就为[idle],并告诉玩家你不会做这个动作或者表情。
        3. 每个回复必须两个方括号,前面为对应表情后面为动作,不能多于或少于。

        例如：[{4}][{5}]你好，我是{0}。";

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
            ChatInit();
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
            ChatInit();
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
                ChatInit();
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
            return string.Format(string.Copy(PromptBase), info.npcName, info.npcDescription, string.Join(",", info.npcEmojis),
                string.Join(",", info.npcActions), info.npcEmojis[1], info.npcActions[0]);
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
            var regex = new Regex(@"^\[(?<first>[^\]]*)\](?:\[(?<second>[^\]]*)\])?(?<rest>.*)$");
            var match = regex.Match(response);

            if(!match.Success) {
                Debug.Log("Regex match failed");
                return new NPCAction(null, null, null);
            }

            string emotion = match.Groups["first"].Value;
            string action = match.Groups["second"].Value;
            string text = match.Groups["rest"].Value;

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