using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TongBuilder.Contract.Contracts
{
    public interface IBizProvider
    {
        TimeSpan TimeOut { get; set; }

        Task<Dictionary<string, string>> GetHeader();
    }
}
