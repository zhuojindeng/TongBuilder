using TongBuilder.RazorLib.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AntDesignProApp.Services
{
    public interface IUserService
    {
        Task<CurrentUser> GetCurrentUserAsync();
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CurrentUser> GetCurrentUserAsync()
        {
            return await _httpClient.GetFromJsonAsync<CurrentUser>("_content/TongBuilder.RazorLib/data/current_user.json");
        }
    }
}