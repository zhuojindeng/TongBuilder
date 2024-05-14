using TongBuilder.RazorLib.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TongBuilder.Contract.Contracts;

namespace AntDesignProApp.Services
{
    public interface IUserService
    {
        Task<CurrentUser> GetCurrentUserAsync();
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IReadFile _readFile;

        public UserService(HttpClient httpClient, IReadFile ReadFile)
        {
            _httpClient = httpClient;
            _readFile = ReadFile;
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            if(_httpClient.BaseAddress == null)
            {
                return await _readFile.ReadContentAsync<CurrentUser>("wwwroot/data/current_user.json");
            }
            return await _httpClient.GetFromJsonAsync<CurrentUser>("_content/TongBuilder.RazorLib/data/current_user.json");
        }
    }
}