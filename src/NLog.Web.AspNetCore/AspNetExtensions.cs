using System;
using System.IO;
using System.Reflection;
using NLog.Config;
using NLog.Web.DependencyInjection;
#if ASP_NET_CORE1 || ASP_NET_CORE2
using Microsoft.AspNetCore.Builder;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE2 || ASP_NET_CORE3
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
#endif

namespace NLog.Web
{
    /// <summary>
    /// Helpers for ASP.NET
    /// </summary>
    public static class AspNetExtensions
    {
#if ASP_NET_CORE1 || ASP_NET_CORE2
        /// <summary>
        /// Enable NLog Web for ASP.NET Core.
        /// </summary>
        /// <param name="app"></param>
#if ASP_NET_CORE2
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, and NLog.Web.NLogBuilder.ConfigureNLog(). Or AddNLog() on ILoggingBuilder")]
#endif
        public static void AddNLogWeb(this IApplicationBuilder app)
        {
            app.ApplicationServices.SetupNLogServiceLocator();
        }
#endif

#if ASP_NET_CORE1 || ASP_NET_CORE2
        /// <summary>
        /// Apply NLog configuration from XML config.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configFileRelativePath">relative path to NLog configuration file.</param>
        /// <returns>LoggingConfiguration for chaining</returns>
#if ASP_NET_CORE2
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, and NLog.Web.NLogBuilder.ConfigureNLog(). Or AddNLog() on ILoggingBuilder")]
#endif
        public static LoggingConfiguration ConfigureNLog(this IHostingEnvironment env, string configFileRelativePath)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            var fileName = Path.Combine(env.ContentRootPath, configFileRelativePath);
            LogManager.LoadConfiguration(fileName);
            return LogManager.Configuration;
        }
#endif

        /// <summary>
        /// Override the default <see cref="IServiceProvider" /> used by the NLog ServiceLocator.
        /// NLog ServiceLocator uses the <see cref="IServiceProvider" /> to access context specific services (Ex. <see cref="Microsoft.AspNetCore.Http.IHttpContextAccessor" />)
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

#if ASP_NET_CORE2 || ASP_NET_CORE3

        /// <summary>
        /// Apply NLog configuration from XML config.
        /// 
        /// This call is not needed when <see cref="NLogBuilder.ConfigureNLog(string)" /> is used.
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>
        /// >
        /// <returns>LogFactory to get loggers, add events etc</returns>
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, and NLog.Web.NLogBuilder.ConfigureNLog(). Or AddNLog() on ILoggingBuilder")]
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
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, and NLog.Web.NLogBuilder.ConfigureNLog(). Or AddNLog() on ILoggingBuilder")]
        public static LogFactory ConfigureNLog(this ILoggingBuilder builder, LoggingConfiguration configuration)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            builder.AddNLog();
            LogManager.Configuration = configuration;
            return LogManager.LogFactory;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging
        /// </summary>
        /// <param name="builder">The logging builder</param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder)
        {
            return builder.AddNLogWeb(NLogAspNetCoreOptions.Default);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder, NLogAspNetCoreOptions options)
        {
            AddNLogLoggerProvider(builder.Services, null, options, (serviceProvider, config, opt) =>
            {
                return CreateNLogLoggerProvider(serviceProvider, config, opt);
            });
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and explicit load NLog.config from path
        /// </summary>
        /// <remarks>Recommended to use AddNLogWeb() to avoid name-collission issue with NLog.Extension.Logging namespace</remarks>
        /// <param name="builder">The logging builder</param>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>
        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, string configFileName)
        {
            return AddNLogWeb(builder, configFileName);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and explicit load NLog.config from path
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder, string configFileName)
        {
            AddNLogLoggerProvider(builder.Services, null, null, (serviceProvider, config, options) =>
            {
                var provider = CreateNLogLoggerProvider(serviceProvider, config, options);
                // Delay initialization of targets until we have loaded config-settings
                LogManager.LoadConfiguration(configFileName);
                return provider;
            });
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and explicit load NLog LoggingConfiguration
        /// </summary>
        /// <remarks>Recommended to use AddNLogWeb() to avoid name-collission issue with NLog.Extension.Logging namespace</remarks>
        /// <param name="builder">The logging builder</param>
        /// <param name="configuration">Config for NLog</param>
        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, LoggingConfiguration configuration)
        {
            return AddNLogWeb(builder, configuration);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and explicit load NLog LoggingConfiguration
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configuration">Config for NLog</param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder, LoggingConfiguration configuration)
        {
            return AddNLogWeb(builder, configuration, null);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and explicit load NLog LoggingConfiguration
        /// </summary>
        /// <remarks>Recommended to use AddNLogWeb() to avoid name-collission issue with NLog.Extension.Logging namespace</remarks>
        /// <param name="builder">The logging builder</param>
        /// <param name="configuration">Config for NLog</param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, LoggingConfiguration configuration, NLogAspNetCoreOptions options)
        {
            return AddNLogWeb(builder, configuration, options);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and explicit load NLog LoggingConfiguration
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configuration">Config for NLog</param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder, LoggingConfiguration configuration, NLogAspNetCoreOptions options)
        {
            AddNLogLoggerProvider(builder.Services, null, options, (serviceProvider, config, opt) =>
            {
                var logFactory = configuration?.LogFactory ?? LogManager.LogFactory;
                var provider = CreateNLogLoggerProvider(serviceProvider, config, opt, logFactory);
                // Delay initialization of targets until we have loaded config-settings
                logFactory.Configuration = configuration;
                return provider;
            });
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and provide isolated LogFactory
        /// </summary>
        /// <remarks>Recommended to use AddNLogWeb() to avoid name-collission issue with NLog.Extension.Logging namespace</remarks>
        /// <param name="builder"></param>
        /// <param name="factoryBuilder">Initialize NLog LogFactory with NLog LoggingConfiguration.</param>
        public static ILoggingBuilder AddNLog(this ILoggingBuilder builder, Func<IServiceProvider, LogFactory> factoryBuilder)
        {
            return AddNLogWeb(builder, factoryBuilder);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and provide isolated LogFactory
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="factoryBuilder">Initialize NLog LogFactory with NLog LoggingConfiguration.</param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder, Func<IServiceProvider, LogFactory> factoryBuilder)
        {
            AddNLogLoggerProvider(builder.Services, null, null, (serviceProvider, config, options) =>
            {
                config = SetupConfiguration(serviceProvider, config);
                // Delay initialization of targets until we have loaded config-settings
                var logFactory = factoryBuilder(serviceProvider);
                var provider = CreateNLogLoggerProvider(serviceProvider, config, options, logFactory);
                return provider;
            });
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and provide isolated LogFactory
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="logFactory">NLog LogFactory</param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder, LogFactory logFactory, NLogAspNetCoreOptions options)
        {
            AddNLogLoggerProvider(builder.Services, null, options, (serviceProvider, config, opt) =>
            {
                logFactory = logFactory ?? LogManager.LogFactory;
                var provider = CreateNLogLoggerProvider(serviceProvider, config, opt, logFactory);
                return provider;
            });
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder)
        {
            return UseNLog(builder, null);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder, NLogAspNetCoreOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureServices((builderContext, services) => AddNLogLoggerProvider(services, builderContext.Configuration, options, CreateNLogLoggerProvider));
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        public static IHostBuilder UseNLog(this IHostBuilder builder)
        {
            return UseNLog(builder, null);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static IHostBuilder UseNLog(this IHostBuilder builder, NLogAspNetCoreOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureServices((builderContext, services) => AddNLogLoggerProvider(services, builderContext.Configuration, options, CreateNLogLoggerProvider));
            return builder;
        }

        private static void AddNLogLoggerProvider(IServiceCollection services, IConfiguration configuration, NLogAspNetCoreOptions options, Func<IServiceProvider, IConfiguration, NLogAspNetCoreOptions, NLogLoggerProvider> factory)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);

            var sharedFactory = factory;

            if ((options ?? NLogAspNetCoreOptions.Default).ReplaceLoggerFactory)
            {
                NLogLoggerProvider singleInstance = null;   // Ensure that registration of ILoggerFactory and ILoggerProvider shares the same single instance
                sharedFactory = (provider, cfg, opt) => singleInstance ?? (singleInstance = factory(provider, cfg, opt));

                services.AddLogging(builder => builder?.ClearProviders());  // Cleanup the existing LoggerFactory, before replacing it with NLogLoggerFactory
                services.Replace(ServiceDescriptor.Singleton<ILoggerFactory, NLogLoggerFactory>(serviceProvider => new NLogLoggerFactory(sharedFactory(serviceProvider, configuration, options))));
            }

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NLogLoggerProvider>(serviceProvider => sharedFactory(serviceProvider, configuration, options)));

            if ((options ?? NLogAspNetCoreOptions.Default).RemoveLoggerFactoryFilter)
            {
                // Will forward all messages to NLog if not specifically overridden by user
                services.AddLogging(builder => builder?.AddFilter<NLogLoggerProvider>(null, Microsoft.Extensions.Logging.LogLevel.Trace));
            }

            //note: this one is called before  services.AddSingleton<ILoggerFactory>
            if ((options ?? NLogAspNetCoreOptions.Default).RegisterHttpContextAccessor)
            {
                services.AddHttpContextAccessor();
            }
        }

        private static NLogLoggerProvider CreateNLogLoggerProvider(IServiceProvider serviceProvider, IConfiguration configuration, NLogAspNetCoreOptions options)
        {
            return CreateNLogLoggerProvider(serviceProvider, configuration, options, null);
        }

        private static NLogLoggerProvider CreateNLogLoggerProvider(IServiceProvider serviceProvider, IConfiguration configuration, NLogAspNetCoreOptions options, NLog.LogFactory logFactory)
        {
            configuration = SetupConfiguration(serviceProvider, configuration);
            NLogLoggerProvider provider = new NLogLoggerProvider(options ?? NLogAspNetCoreOptions.Default, logFactory ?? LogManager.LogFactory);
            if (configuration != null && options == null)
            {
                provider.Configure(configuration.GetSection("Logging:NLog"));
            }
            return provider;
        }

        private static IConfiguration SetupConfiguration(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            ServiceLocator.ServiceProvider = serviceProvider;
            configuration = configuration ?? (serviceProvider?.GetService(typeof(IConfiguration)) as IConfiguration);
            if (configuration != null)
            {
                ConfigSettingLayoutRenderer.DefaultConfiguration = configuration;
            }
            return configuration;
        }
#endif
    }
}
