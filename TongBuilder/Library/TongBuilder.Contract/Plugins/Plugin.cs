﻿namespace TongBuilder.Contract.Plugins
{
    /// <summary>
    /// Plugin describes details of an extension that can be loaded in to application
    /// https://github.com/yiyungent/PluginCore
    /// </summary>
    public class Plugin
    {
        /// <summary>
        /// Relative path where plugin is located
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Type of plugin
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Assembly containing the plugin
        /// </summary>
        public string Name { get; set; }
    }
}
