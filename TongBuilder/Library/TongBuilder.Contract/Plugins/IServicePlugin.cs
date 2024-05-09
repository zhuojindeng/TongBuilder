using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TongBuilder.Contract.Plugins
{
    public interface IServicePlugin
    {
        void ConfigureService(IServiceCollection services, IConfiguration configuration);
    }
}
