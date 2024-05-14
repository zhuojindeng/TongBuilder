using TongBuilder.RazorLib.Models;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TongBuilder.Contract.Contracts;

namespace TongBuilder.RazorLib.Services
{
    public interface IProjectService
    {
        Task<NoticeType[]> GetProjectNoticeAsync();
        Task<ActivitiesType[]> GetActivitiesAsync();
        Task<ListItemDataType[]> GetFakeListAsync(int count = 0);
        Task<NoticeItem[]> GetNoticesAsync();
    }

    public class ProjectService : IProjectService
    {
        private readonly HttpClient _httpClient;
        private readonly IReadFile _readFile;

        public ProjectService(HttpClient httpClient, IReadFile ReadFile)
        {
            _httpClient = httpClient;
            _readFile = ReadFile;
        }

        public async Task<NoticeType[]> GetProjectNoticeAsync()
        {
            if(_httpClient.BaseAddress == null)
            {
                return await _readFile.ReadContentAsync<NoticeType[]>("wwwroot/data/notice.json");
            }
            return await _httpClient.GetFromJsonAsync<NoticeType[]>("_content/TongBuilder.RazorLib/data/notice.json");
        }

        public async Task<NoticeItem[]> GetNoticesAsync()
        {
            if (_httpClient.BaseAddress == null)
            {
                return await _readFile.ReadContentAsync<NoticeItem[]>("wwwroot/data/notices.json");
            }
            return await _httpClient.GetFromJsonAsync<NoticeItem[]>("_content/TongBuilder.RazorLib/data/notices.json");
        }

        public async Task<ActivitiesType[]> GetActivitiesAsync()
        {
            if (_httpClient.BaseAddress == null)
            {
                return await _readFile.ReadContentAsync<ActivitiesType[]>("wwwroot/data/activities.json");
            }
            return await _httpClient.GetFromJsonAsync<ActivitiesType[]>("_content/TongBuilder.RazorLib/data/activities.json");
        }

        public async Task<ListItemDataType[]> GetFakeListAsync(int count = 0)
        {
            if (_httpClient.BaseAddress == null)
            {
                return await _readFile.ReadContentAsync<ListItemDataType[]>("wwwroot/data/fake_list.json");
            }
            var data = await _httpClient.GetFromJsonAsync<ListItemDataType[]>("_content/TongBuilder.RazorLib/data/fake_list.json");
            return count > 0 ? data.Take(count).ToArray() : data;
        }
    }
}