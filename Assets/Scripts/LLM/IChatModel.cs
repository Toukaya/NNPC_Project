using System;
using Character.NPC;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LLM
{
    public abstract class ChatModel : MonoBehaviour
    {
        public abstract void Init(string prompt);
        public abstract UniTask<string> SendChatRequestAsync(string message);

        public abstract void LoadMemory(ConversationMemory memory);
    }
}