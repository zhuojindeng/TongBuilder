using TongBuilder.Contract;
using TongBuilder.Contract.Contracts;

namespace TongBuilder.BlazorMaui.App.Services
{
    /// <summary>
    /// 打开捆绑到应用软件包中的文件(MAUI)
    /// 原始资产在应用的 Resources/Raw 文件夹中
    /// </summary>
    public class ReadFile : IReadFile
    {
        public async Task<string> ReadContentAsync(string file)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(file);
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }

        public async Task<T> ReadContentAsync<T>(string file)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(file);
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            return Utils.FromJson<T>(content);
        }
    }
}
