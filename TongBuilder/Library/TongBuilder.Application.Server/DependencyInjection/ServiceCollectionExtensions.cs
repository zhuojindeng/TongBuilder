using Microsoft.Extensions.DependencyInjection;
using McMaster.NETCore.Plugins;
using TongBuilder.Contract.Plugins;

namespace TongBuilder.Application.Server.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Load a service plugin from plugins directory and invoke the ConfigureService() method on plugin.
        /// This will allow plugin to register the required services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="pluginName"></param>
        /// <param name="sharedTypes"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlugin<T>(this IServiceCollection services, Plugin plugin,
            Action<T, IServiceCollection> configure)
        {
            string pluginsDirectory = Path.Combine(AppContext.BaseDirectory, plugin.Path, plugin.Name);
            if (Directory.Exists(pluginsDirectory))
            {
                var pluginFile = Directory.GetFiles(pluginsDirectory, "*.dll").Where(f => Path.GetFileNameWithoutExtension(f).Equals(plugin.Name)).Single();
                var loader = PluginLoader.CreateFromAssemblyFile(pluginFile, c => { c.PreferSharedTypes = true; });
                foreach (var type in loader.LoadDefaultAssembly().GetTypes()
                    .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    var servicePlugin = (T)Activator.CreateInstance(type);
                    configure(servicePlugin, services);
                }
            }
            return services;
        }
    }
}
