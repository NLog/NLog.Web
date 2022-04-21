using System;
using System.Reflection;
using NLog.Config;

namespace NLog.Web
{
    /// <summary>
    /// NLog helpers to ensure registration of NLog.Web-extensions before loading NLog-configuration
    /// </summary>
    /// <remarks>
    /// It is now recommended to use NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
    /// </remarks>
    public static class NLogBuilder
    {
        /// <summary>
        /// Configure NLog from XML config.
        /// </summary>
        /// <remarks>
        /// It is now recommended to use NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
        /// </remarks>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config</param>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(string configFileName)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            return LogManager.LoadConfiguration(configFileName);
        }

        /// <summary>
        /// Configure NLog from API
        /// </summary>
        /// <remarks>
        /// It is now recommended to use NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
        /// </remarks>
        /// <param name="configuration">Config for NLog</param>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(LoggingConfiguration configuration)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.Configuration = configuration;
            return LogManager.LogFactory;
        }
    }
}