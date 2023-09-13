using Cysharp.Threading.Tasks;

namespace LLM
{
    public interface IChatModelBase
    {
        UniTask<string> SendChatRequestAsync(string message);
    }
}