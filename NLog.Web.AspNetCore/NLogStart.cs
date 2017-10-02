using System;
using System.Collections.Generic;
using System.Reflection;
using NLog.Common;
using NLog.Config;

namespace NLog.Web
{
    /// <summary>
    /// NLog start helper
    /// </summary>
    public static class NLogStart
    {

        static NLogStart()
        {
            try
            {
                //register yourself, so <extension> isn't needed.
                ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "Registering NLog.Web.AspNetCore failed");
            }

        }

        /// <summary>
        /// Init the NLog
        /// 
        /// If this method is used, there is no need for ConfigureNLog
        /// </summary>
        /// <param name="fileName">Path to NLog configuration file, e.g. nlog.config. </param>>
        /// <returns>Logger to start logging</returns>
        public static Logger InitLogger(string fileName)
        {
            var configuration = new XmlLoggingConfiguration(fileName);
            return InitLogger(configuration);
        }

        /// <summary>
        /// Init the NLog
        /// 
        /// If this method is used, there is no need for ConfigureNLog
        /// </summary>
        /// <param name="configuration">NLog config</param>>
        /// <returns>Logger to start logging</returns>
        public static Logger InitLogger(LoggingConfiguration configuration)
        {
            LogManager.Configuration = configuration;
            var logger = LogManager.GetCurrentClassLogger();
            return logger;
        }
    }
}
