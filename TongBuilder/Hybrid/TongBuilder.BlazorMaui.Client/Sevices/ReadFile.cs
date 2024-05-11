using TongBuilder.Contract.Contracts;

namespace TongBuilder.BlazorMaui.Client.Sevices
{
    public class ReadFile : IReadFile
    {
        public async Task<string> ReadContent(string file)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(file);
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}
