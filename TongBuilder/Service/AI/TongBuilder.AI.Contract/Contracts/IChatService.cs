using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TongBuilder.AI.Contract.Models;

namespace TongBuilder.AI.Contract.Contracts
{
    public interface IChatService
    {
        IAsyncEnumerable<string> SendChatByAppAsync(AiApp app, ChatHistory history);

        Task<ChatHistory> GetChatHistory(List<AiChat> messageList, ChatHistory history);
    }
}
