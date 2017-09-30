using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Web.Internal;
using NLog.Extensions.Logging;


#if NETSTANDARD2_0
using Microsoft.Extensions.DependencyInjection;
#endif

namespace NLog.Web
{
    /// <summary>
    /// Helpers for ASP.NET
    /// </summary>
    public static class AspNetExtensions
    {


        /// <summary>
        /// Enable NLog Web for ASP.NET Core.
        /// </summary>
        /// <param name="app"></param>
#if NETSTANDARD2_0
        [Obsolete("Use UseNLog() on IWebHostBuilder")]
#endif
        public static void AddNLogWeb(this IApplicationBuilder app)
        {
            ServiceLocator.ServiceProvider = app.ApplicationServices;
        }

        /// <summary>
        /// Apply NLog configuration from XML config.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configFileRelativePath">relative path to NLog configuration file.</param>
        /// <returns>LoggingConfiguration for chaining</returns>
        public static LoggingConfiguration ConfigureNLog(this IHostingEnvironment env, string configFileRelativePath)
        {
            var fileName = Path.Combine(env.ContentRootPath, configFileRelativePath);
            return ConfigureNLog(fileName);
        }

        /// <summary>
        /// Apply NLog configuration from XML config.
        /// </summary>
        /// <param name="fileName">absolute path  NLog configuration file.</param>
        /// <returns>LoggingConfiguration for chaining</returns>
        private static LoggingConfiguration ConfigureNLog(string fileName)
        {
            var configuration = new XmlLoggingConfiguration(fileName, true);
            LogManager.Configuration = configuration;
            return configuration;
        }


#if NETSTANDARD2_0


        /// <summary>
        /// Use NLog for Dependency Injected loggers. 
        /// </summary>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder)
        {
            return UseNLog(builder, null);
        }

        /// <summary>
        /// Use NLog for Dependency Injected loggers. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for logging to NLog with Dependency Injected loggers</param>
        /// <returns></returns>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder, NLogProviderOptions options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.ConfigureServices(services =>
                services.AddSingleton<ILoggerFactory>(serviceProvider =>
                {
                    ServiceLocator.ServiceProvider = serviceProvider;
                    ServiceLocator.Services = services;

                    return new NLogLoggerFactory();
                })
            );
            return builder;
        }
#endif


    }
}
