#if ASP_NET_CORE
using System;
using System.Reflection;
using NLog.Config;

namespace NLog.Web
{
    /// <summary>
    /// NLog helper for ASP.NET Standard 2
    /// </summary>
    public static class NLogBuilder
    {
        /// <summary>
        /// Configure NLog from XML config.
        /// </summary>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(string configFileName)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            return LogManager.LoadConfiguration(configFileName);
        }

        /// <summary>
        /// Configure NLog from API
        /// </summary>
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
#endif