using System.Reflection;
using TongBuilder.Contract.Contracts;

namespace TongBuilder.RazorLib.Services
{
    public abstract class PlatformService : IPlatformService
    {
        public virtual string GetVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                return string.Empty;
            }

            var assemblyFileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (assemblyFileVersionAttribute == null)
            {
                return string.Empty;
            }

            return assemblyFileVersionAttribute.Version;
        }

        public virtual Task OpenBrowserUrl(string url)
        {
            //return Browser.Default.OpenAsync(url);
            throw new NotImplementedException();
        }

        public virtual Task<T> ReadJsonAsync<T>(string baseUri)
        {
            throw new NotImplementedException();
        }
    }
}
