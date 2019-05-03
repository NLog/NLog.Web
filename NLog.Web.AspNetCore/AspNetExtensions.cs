using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Web.DependencyInjection;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#if ASP_NET_CORE2
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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
#if ASP_NET_CORE2
        [Obsolete("Use UseNLog() on IWebHostBuilder")]
#endif
        public static void AddNLogWeb(this IApplicationBuilder app)
        {
            app.ApplicationServices.SetupNLogServiceLocator();
        }

        /// <summary>
        /// Apply NLog configuration from XML config.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configFileRelativePath">relative path to NLog configuration file.</param>
        /// <returns>LoggingConfiguration for chaining</returns>
        public static LoggingConfiguration ConfigureNLog(this IHostingEnvironment env, string configFileRelativePath)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            var fileName = Path.Combine(env.ContentRootPath, configFileRelativePath);
            LogManager.LoadConfiguration(fileName);
            return LogManager.Configuration;
        }

        /// <summary>
        /// Override the default <see cref="IServiceProvider" /> used by the NLog ServiceLocator.
        /// NLog ServiceLocator uses the <see cref="IServiceProvider" /> to access context specific services (Ex. <see cref="IHttpContextAccessor" />)
        /// </summary>
        /// <remarks>
        /// Should only be used if the standard approach for configuring NLog is not enough
        /// </remarks>
        /// <param name="serviceProvider"></param>
        public static IServiceProvider SetupNLogServiceLocator(this IServiceProvider serviceProvider)
        {
            ServiceLocator.ServiceProvider = serviceProvider;
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            return serviceProvider;
        }

#if ASP_NET_CORE2

        /// <summary>
        /// Apply NLog configuration from XML config.
        /// 
        /// This call is not needed when <see cref="NLogBuilder.ConfigureNLog(string)" /> is used.
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>
        /// >
        /// <returns>LogFactory to get loggers, add events etc</returns>
        [Obsolete("Use UseNLog() on IWebHostBuilder, and NLog.Web.NLogBuilder.ConfigureNLog()")]
        public static LogFactory ConfigureNLog(this ILoggingBuilder builder, string configFileName)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            builder.AddNLog();
            return LogManager.LoadConfiguration(configFileName);
        }

        /// <summary>
        /// Configure NLog from API
        /// 
        /// This call is not needed when <see cref="NLogBuilder.ConfigureNLog(LoggingConfiguration)" /> is used.
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configuration">Config for NLog</param>
        /// <returns>LogFactory to get loggers, add events etc</returns>
        [Obsolete("Use UseNLog() on IWebHostBuilder, and NLog.Web.NLogBuilder.ConfigureNLog()")]
        public static LogFactory ConfigureNLog(this ILoggingBuilder builder, LoggingConfiguration configuration)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            builder.AddNLog();
            LogManager.Configuration = configuration;
            return LogManager.LogFactory;
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
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureServices(services => { ConfigureServicesNLog(options, services, serviceProvider => serviceProvider.GetService<IConfiguration>()); });
            return builder;
        }

        /// <summary>
        /// Use NLog for Dependency Injected loggers.
        /// </summary>
        public static IHostBuilder UseNLog(this IHostBuilder builder)
        {
            return UseNLog(builder, null);
        }

        /// <summary>
        /// Use NLog for Dependency Injected loggers.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for logging to NLog with Dependency Injected loggers</param>
        /// <returns></returns>
        public static IHostBuilder UseNLog(this IHostBuilder builder, NLogAspNetCoreOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureServices((hostbuilder, services) => { ConfigureServicesNLog(options, services, serviceProvider => hostbuilder.Configuration); });
            return builder;
        }

        private static void ConfigureServicesNLog(NLogAspNetCoreOptions options, IServiceCollection services, Func<IServiceProvider, IConfiguration> lookupConfiguration)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);

            services.AddSingleton<ILoggerProvider>(serviceProvider =>
            {
                ServiceLocator.ServiceProvider = serviceProvider;

                var provider = new NLogLoggerProvider(options ?? new NLogProviderOptions());
                var configuration = lookupConfiguration(serviceProvider);
                if (configuration != null)
                {
                    ConfigSettingLayoutRenderer.DefaultConfiguration = configuration;
                    if (options == null)
                    {
                        provider.Configure(configuration.GetSection("Logging:NLog"));
                    }
                }

                return provider;
            });

            //note: this one is called before  services.AddSingleton<ILoggerFactory>
            if ((options ?? NLogAspNetCoreOptions.Default).RegisterHttpContextAccessor)
            {
                services.AddHttpContextAccessor();
            }
        }
#endif
    }
}