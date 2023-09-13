using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.NPC
{
    //保存一条对话记录
    [Serializable]
    public record ConversationMemory
    {
        public bool isPlayerMessage;
        public string content;
        public string date;
        public string address;

        public override string ToString()
        {
            return content;
        }
    }
    [CreateAssetMenu(fileName = "NpcInfoData", menuName = "ScriptableObject/NpcInfo", order = 0)]
    public class NpcInfo : ScriptableObject
    {
        [SerializeField] public string npcName;
        [TextArea] public string npcDescription;
        [SerializeField] public List<string> npcActions = new();        
        [SerializeField] public List<string> npcEmojis = new();
        [SerializeField] public string npcVoice;
        [SerializeField] [Range(1, 10)] public byte maxMemoryCapacity = 5;
        [SerializeField] public List<ConversationMemory> mainMemory = new();
        private readonly Queue<ConversationMemory> _memory = new();

        public void RecordMemory(ConversationMemory memory)
        {
            if (_memory.Count >= maxMemoryCapacity)
                _memory.Dequeue();
            _memory.Enqueue(memory);
        }
        public void ClearMemory()
        {
            _memory.Clear();
        }
        public Queue<ConversationMemory> GetMemory()
        {
            return _memory;
        }
      
    }
}