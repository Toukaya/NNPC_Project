using System.Collections.Generic;
using LitJson;

namespace VocaModu.xfyun.util
{
    public class JsonDataPool
    {
        private readonly Queue<JsonData> _pool = new();

        private readonly int _maxPoolSize;

        public static JsonData LastFrame { get; } = new()
        {
            ["data"] = new JsonData
            {
                ["status"] = 2,
                ["format"] = "audio/L16;rate=16000",
                ["audio"] = "",
                ["encoding"] = "raw" // 动态修正
            }
        };

        public JsonData CommonJsonData { get; }
        
        public JsonData BusinessJsonData { get; }
        
        public JsonData DataJsonData { get; }

        public JsonDataPool(int maxPoolSize)
        {
            _maxPoolSize = maxPoolSize;
            
            CommonJsonData = new JsonData();
            BusinessJsonData = new JsonData();
            DataJsonData = new JsonData();
        }

        public JsonData Get()
        {
            return _pool.Count > 0 ? _pool.Dequeue() : new JsonData();
        }

        public void Release(JsonData data)
        {
            if (_pool.Count >= _maxPoolSize) return;
            data.Clear();
            _pool.Enqueue(data);
        }
    }
}