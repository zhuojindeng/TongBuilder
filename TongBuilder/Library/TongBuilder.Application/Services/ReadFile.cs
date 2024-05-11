using TongBuilder.Contract.Contracts;

namespace TongBuilder.Application.Services
{
    public class ReadFile : IReadFile
    {
        public async Task<string> ReadContent(string file)
        {
            using var reader = new StreamReader(file);

            return await reader.ReadToEndAsync();
        }
    }
}
