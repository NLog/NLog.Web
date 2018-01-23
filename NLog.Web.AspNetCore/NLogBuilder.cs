#if ASP_NET_CORE2

using System;
using System.Collections.Generic;
using System.Reflection;
using NLog.Common;
using NLog.Config;
using NLog.Extensions.Logging;

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
        [Obsolete("Instead use NLog.LogManager.LoadConfiguration()")]
        public static LogFactory ConfigureNLog(string configFileName)
        {
            return LogManager.LoadConfiguration(configFileName);
        }

        /// <summary>
        /// Configure NLog from API
        /// </summary>
        /// <param name="configuration">Config for NLog</param>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        [Obsolete("Instead assign property NLog.LogManager.Configuration")]
        public static LogFactory ConfigureNLog(LoggingConfiguration configuration)
        {
            LogManager.Configuration = configuration;
            return LogManager.LogFactory;
        }
    }
}
#endif