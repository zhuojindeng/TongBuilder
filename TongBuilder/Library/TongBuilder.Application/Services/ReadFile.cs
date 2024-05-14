using System.Threading.Tasks;
using TongBuilder.Contract;
using TongBuilder.Contract.Contracts;

namespace TongBuilder.Application.Services
{
    public class ReadFile : IReadFile
    {
        public async Task<string> ReadContentAsync(string file)
        {
            using var reader = new StreamReader(file);

            return await reader.ReadToEndAsync();
        }

        public async Task<T> ReadContentAsync<T>(string file)
        {
            using var reader = new StreamReader(file);
            var content=await reader.ReadToEndAsync();
            return Utils.FromJson<T>(content);
        }
    }
}
