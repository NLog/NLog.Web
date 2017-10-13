#if NETSTANDARD2_0


using System;
using System.Collections.Generic;
using System.Reflection;
using NLog.Common;
using NLog.Config;

namespace NLog.Web
{


    /// <summary>
    /// NLog helper for ASP.NET Standard 2
    /// </summary>
    public static class NLogBuilder
    {
        /// <summary>
        /// Register NLog.Web.AspNetCore, so so &lt;extension&gt; in NLog's config isn't needed.
        /// </summary>
        internal static void RegisterNLogWebAspNetCore()
        {
            try
            {
                InternalLogger.Debug("Registering NLog.Web.AspNetCore");
                ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "Registering NLog.Web.AspNetCore failed");
            }
            try
            {
                LogManager.AddHiddenAssembly(typeof(NLogBuilder).GetTypeInfo().Assembly);
            }
            catch (Exception ex)
            {
                InternalLogger.Warn(ex, "Hidding assembly of {0} failed", nameof(NLogBuilder));
            }

        }

        /// <summary>
        /// Configure NLog from XML config.
        /// </summary>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(string configFileName)
        {
            RegisterNLogWebAspNetCore();
            return ConfigureNLog(new XmlLoggingConfiguration(configFileName));
        }

        /// <summary>
        /// Configure NLog from API
        /// </summary>
        /// <param name="configuration">Config for NLog</param>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(LoggingConfiguration configuration)
        {
            RegisterNLogWebAspNetCore();
            LogManager.Configuration = configuration;
            return LogManager.LogFactory;
        }
    }
}
#endif