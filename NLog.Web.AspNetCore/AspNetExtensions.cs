using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Web.Internal;
using NLog.Extensions.Logging;
using NLog.Web.AspNetCore;
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
        /// <param name="fileName">Path to NLog configuration file, e.g. nlog.config. </param>
        /// <returns>LoggingConfiguration for chaining</returns>
        private static LoggingConfiguration ConfigureNLog(string fileName)
        {
            var configuration = new XmlLoggingConfiguration(fileName, true);
            LogManager.Configuration = configuration;
            return configuration;
        }


#if NETSTANDARD2_0

        /// <summary>
        /// Apply NLog configuration from XML config.
        /// 
        /// This call is not needed when <see cref="NLogBuilder.ConfigureNLog(string)"/> is used.
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(this ILoggingBuilder builder, string configFileName)
        {
            return NLogBuilder.ConfigureNLog(configFileName);
        }

        /// <summary>
        /// Configure NLog from API
        /// 
        /// This call is not needed when <see cref="NLogBuilder.ConfigureNLog(LoggingConfiguration)"/> is used.
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configuration">Config for NLog</param>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        public static LogFactory ConfigureNLog(this ILoggingBuilder builder, LoggingConfiguration configuration)
        {
            return NLogBuilder.ConfigureNLog(configuration);
        }

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
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder, NLogAspNetCoreOptions options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            options = options ?? NLogAspNetCoreOptions.Default;

            builder.ConfigureServices(services =>
            {
                //note: when registering ILoggerFactory, all non NLog stuff and stuff before this will be removed
                services.AddSingleton<ILoggerProvider>(serviceProvider =>
                {
                    ServiceLocator.ServiceProvider = serviceProvider;

                    NLogBuilder.RegisterNLogWebAspNetCore();

                    LogManager.Configuration?.Reload();
                    return new NLogLoggerProvider(options);
                });

                //note: this one is called before  services.AddSingleton<ILoggerFactory>
                if (options.RegisterHttpContextAccessor)
                {
                    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                }

            });

            return builder;
        }



#endif


    }
}
