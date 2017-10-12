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

        }

        /// <summary>
        /// Configure NLog
        /// </summary>
        /// <param name="fileName">Path to NLog configuration file, e.g. nlog.config. </param>>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(string fileName)
        {
            RegisterNLogWebAspNetCore();
            return ConfigureNLog(new XmlLoggingConfiguration(fileName));
        }

        /// <summary>
        /// Configure NLog
        /// </summary>
        /// <param name="configuration"></param>
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