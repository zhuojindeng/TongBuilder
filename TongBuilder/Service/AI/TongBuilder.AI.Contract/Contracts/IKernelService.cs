using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TongBuilder.AI.Contract.Models;

namespace TongBuilder.AI.Contract.Contracts
{
    public interface IKernelService
    {
        Kernel GetKernelByAiApp(AiApp app);

        Kernel GetKernelByAIModel(string modelid);
    }
}
