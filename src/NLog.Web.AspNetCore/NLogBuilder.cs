using System;
using NLog.Config;

namespace NLog.Web
{
    /// <summary>
    /// NLog helpers to ensure registration of NLog.Web-extensions before loading NLog-configuration
    /// </summary>
    /// <remarks>
    /// It is now recommended to use NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
    /// </remarks>
    [Obsolete("Use NLog.LogManager.Setup().LoadConfigurationFromAppSettings() instead. Marked obsolete with NLog.Web 5.3")]
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
        [Obsolete("Use NLog.LogManager.Setup().LoadConfigurationFromAppSettings() instead. Marked obsolete with NLog.Web 5.3")]
        public static LogFactory ConfigureNLog(string configFileName)
        {
            return LogManager.Setup().RegisterNLogWeb().LoadConfigurationFromFile(configFileName, optional: false).LogFactory;
        }

        /// <summary>
        /// Configure NLog from API
        /// </summary>
        /// <remarks>
        /// It is now recommended to use NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
        /// </remarks>
        /// <param name="configuration">Config for NLog</param>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        [Obsolete("Use NLog.LogManager.Setup().LoadConfigurationFromAppSettings() instead. Marked obsolete with NLog.Web 5.3")]
        public static LogFactory ConfigureNLog(LoggingConfiguration configuration)
        {
            return LogManager.Setup().RegisterNLogWeb().LoadConfiguration(configuration).LogFactory;
        }
    }
}