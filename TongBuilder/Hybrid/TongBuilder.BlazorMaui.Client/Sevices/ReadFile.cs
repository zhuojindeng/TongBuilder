﻿using TongBuilder.Contract;
using TongBuilder.Contract.Contracts;

namespace TongBuilder.BlazorMaui.Client.Sevices
{
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
