﻿using System;
using System.Linq;
using System.Reflection;
using NLog.Config;
using NLog.Web.DependencyInjection;
#if !NETCOREAPP3_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web.Internal;

namespace NLog.Web
{
    /// <summary>
    /// Helpers for ASP.NET
    /// </summary>
    public static class AspNetExtensions
    {
#if !NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// Enable NLog Web for ASP.NET Core.
        /// </summary>
        /// <param name="app"></param>
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, or AddNLogWeb() on ILoggingBuilder")]
        public static void AddNLogWeb(this IApplicationBuilder app)
        {
            Guard.ThrowIfNull(app);
            app.ApplicationServices.SetupNLogServiceLocator();
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
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, or AddNLogWeb() on ILoggingBuilder")]
        public static IServiceProvider SetupNLogServiceLocator(this IServiceProvider serviceProvider)
        {
            ServiceLocator.ServiceProvider = serviceProvider;
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            return serviceProvider;
        }

#if !NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// Apply NLog configuration from XML config.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configFileRelativePath">relative path to NLog configuration file.</param>
        /// <returns>LoggingConfiguration for chaining</returns>
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, or AddNLogWeb() on ILoggingBuilder")]
        public static LoggingConfiguration ConfigureNLog(this IHostEnvironment env, string configFileRelativePath)
        {
            Guard.ThrowIfNull(env);

            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            LogManager.AddHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            var fileName = System.IO.Path.Combine(env.ContentRootPath, configFileRelativePath);
            LogManager.LoadConfiguration(fileName);
            return LogManager.Configuration;
        }
#endif

        /// <summary>
        /// Apply NLog configuration from XML config.
        /// 
        /// This call is not needed when <see cref="NLogBuilder.ConfigureNLog(string)" /> is used.
        /// </summary>
        /// <param name="builder">The logging builder</param>
        /// <param name="configFileName">Path to NLog configuration file, e.g. nlog.config. </param>
        /// >
        /// <returns>LogFactory to get loggers, add events etc</returns>
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, or AddNLogWeb() on ILoggingBuilder")]
        public static LogFactory ConfigureNLog(this ILoggingBuilder builder, string configFileName)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            builder.AddNLogWeb();
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
        [Obsolete("Use UseNLog() on IHostBuilder / IWebHostBuilder, or AddNLogWeb() on ILoggingBuilder")]
        public static LogFactory ConfigureNLog(this ILoggingBuilder builder, LoggingConfiguration configuration)
        {
            ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly);
            builder.AddNLogWeb();
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
            Guard.ThrowIfNull(builder);

            AddNLogLoggerProvider(builder.Services, null, null, options, (serviceProvider, config, env, opt) =>
            {
                return CreateNLogLoggerProvider(serviceProvider, config, env, opt);
            });
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging, and provide isolated LogFactory
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        /// <param name="factoryBuilder">Initialize NLog LogFactory with NLog LoggingConfiguration.</param>
        public static ILoggingBuilder AddNLogWeb(this ILoggingBuilder builder, NLogAspNetCoreOptions options, Func<IServiceProvider, LogFactory> factoryBuilder)
        {
            Guard.ThrowIfNull(builder);
            Guard.ThrowIfNull(factoryBuilder);

            AddNLogLoggerProvider(builder.Services, null, null, options, (serviceProvider, config, env, opt) =>
            {
                config = SetupNLogConfigSettings(serviceProvider, config, LogManager.LogFactory);
                // Delay initialization of targets until we have loaded config-settings
                var logFactory = factoryBuilder(serviceProvider);
                var provider = CreateNLogLoggerProvider(serviceProvider, config, env, opt, logFactory);
                return provider;
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
            Guard.ThrowIfNull(builder);

            AddNLogLoggerProvider(builder.Services, null, null, null, (serviceProvider, config, env, options) =>
            {
                var provider = CreateNLogLoggerProvider(serviceProvider, config, env, options);
                // Delay initialization of targets until we have loaded config-settings
                provider.LogFactory.Setup().LoadConfigurationFromFile(configFileName);
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
            return AddNLogWeb(builder, configuration, NLogAspNetCoreOptions.Default);
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
            Guard.ThrowIfNull(builder);

            AddNLogLoggerProvider(builder.Services, null, null, options, (serviceProvider, config, env, opt) =>
            {
                var logFactory = configuration?.LogFactory ?? LogManager.LogFactory;
                var provider = CreateNLogLoggerProvider(serviceProvider, config, env, opt, logFactory);
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
            Guard.ThrowIfNull(builder);
            Guard.ThrowIfNull(factoryBuilder);

            AddNLogLoggerProvider(builder.Services, null, null, null, (serviceProvider, config, env, options) =>
            {
                config = SetupNLogConfigSettings(serviceProvider, config, LogManager.LogFactory);
                // Delay initialization of targets until we have loaded config-settings
                var logFactory = factoryBuilder(serviceProvider);
                var provider = CreateNLogLoggerProvider(serviceProvider, config, env, options, logFactory);
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
            Guard.ThrowIfNull(builder);

            AddNLogLoggerProvider(builder.Services, null, null, options, (serviceProvider, config, env, opt) =>
            {
                logFactory = logFactory ?? LogManager.LogFactory;
                var provider = CreateNLogLoggerProvider(serviceProvider, config, env, opt, logFactory);
                return provider;
            });
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder)
        {
            return UseNLog(builder, NLogAspNetCoreOptions.Default);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static IWebHostBuilder UseNLog(this IWebHostBuilder builder, NLogAspNetCoreOptions options)
        {
            Guard.ThrowIfNull(builder);

            builder.ConfigureServices((builderContext, services) => AddNLogLoggerProvider(services, builderContext.Configuration, builderContext.HostingEnvironment as IHostEnvironment, options, CreateNLogLoggerProvider));
            return builder;
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        public static IHostBuilder UseNLog(this IHostBuilder builder)
        {
            return UseNLog(builder, NLogAspNetCoreOptions.Default);
        }

        /// <summary>
        /// Enable NLog as logging provider for Microsoft Extension Logging.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">Options for registration of the NLog LoggingProvider and enabling features.</param>
        public static IHostBuilder UseNLog(this IHostBuilder builder, NLogAspNetCoreOptions options)
        {
            Guard.ThrowIfNull(builder);

#if NETCOREAPP3_0_OR_GREATER
            builder.ConfigureServices((builderContext, services) => AddNLogLoggerProvider(services, builderContext.Configuration, builderContext.HostingEnvironment, options, CreateNLogLoggerProvider));
#else
            builder.ConfigureServices((builderContext, services) => AddNLogLoggerProvider(services, builderContext.Configuration, null, options, CreateNLogLoggerProvider));
#endif
            return builder;
        }

        private static void AddNLogLoggerProvider(IServiceCollection services, IConfiguration hostConfiguration, IHostEnvironment hostEnvironment, NLogAspNetCoreOptions options, Func<IServiceProvider, IConfiguration, IHostEnvironment, NLogAspNetCoreOptions, NLogLoggerProvider> factory)
        {
            options = options ?? NLogAspNetCoreOptions.Default;
            options.Configure(hostConfiguration?.GetSection("Logging:NLog"));

            var sharedFactory = factory;

            if (options.ReplaceLoggerFactory)
            {
                NLogLoggerProvider singleInstance = null;   // Ensure that registration of ILoggerFactory and ILoggerProvider shares the same single instance
                sharedFactory = (provider, cfg, env, opt) => singleInstance ?? (singleInstance = factory(provider, cfg, env, opt));

                services.AddLogging(builder => builder?.ClearProviders());  // Cleanup the existing LoggerFactory, before replacing it with NLogLoggerFactory
                services.Replace(ServiceDescriptor.Singleton<ILoggerFactory, NLogLoggerFactory>(serviceProvider => new NLogLoggerFactory(sharedFactory(serviceProvider, hostConfiguration, hostEnvironment, options))));
            }

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NLogLoggerProvider>(serviceProvider => sharedFactory(serviceProvider, hostConfiguration, hostEnvironment, options)));

            if (options.RemoveLoggerFactoryFilter)
            {
                // Will forward all messages to NLog if not specifically overridden by user
                services.AddLogging(builder => builder?.AddFilter<NLogLoggerProvider>(null, Microsoft.Extensions.Logging.LogLevel.Trace));
            }

            //note: this one is called before  services.AddSingleton<ILoggerFactory>
            if (options.RegisterHttpContextAccessor)
            {
                services.AddHttpContextAccessor();
            }
        }

        private static NLogLoggerProvider CreateNLogLoggerProvider(IServiceProvider serviceProvider, IConfiguration hostConfiguration, IHostEnvironment hostEnvironment, NLogAspNetCoreOptions options)
        {
            return CreateNLogLoggerProvider(serviceProvider, hostConfiguration, hostEnvironment, options, null);
        }

        private static NLogLoggerProvider CreateNLogLoggerProvider(IServiceProvider serviceProvider, IConfiguration hostConfiguration, IHostEnvironment hostEnvironment, NLogAspNetCoreOptions options, NLog.LogFactory logFactory)
        {
            NLogLoggerProvider provider = new NLogLoggerProvider(options, logFactory ?? LogManager.LogFactory);

            var configuration = SetupNLogConfigSettings(serviceProvider, hostConfiguration, provider.LogFactory);

            if (configuration != null && (!ReferenceEquals(configuration, hostConfiguration) || options is null))
            {
                provider.Configure(configuration.GetSection("Logging:NLog"));
            }

            if (serviceProvider != null && provider.Options.RegisterServiceProvider)
            {
                provider.LogFactory.ServiceRepository.RegisterService(typeof(IServiceProvider), serviceProvider);
            }

            if (configuration is null || !TryLoadConfigurationFromSection(provider, configuration))
            {
                string nlogConfigFile = string.Empty;
                string contentRootPath = hostEnvironment?.ContentRootPath;
                string environmentName = hostEnvironment?.EnvironmentName;
                if (!string.IsNullOrWhiteSpace(contentRootPath) || !string.IsNullOrWhiteSpace(environmentName))
                {
                    provider.LogFactory.Setup().LoadConfiguration(cfg =>
                    {
                        if (!IsLoggingConfigurationLoaded(cfg.Configuration))
                        {
                            nlogConfigFile = ResolveEnvironmentNLogConfigFile(contentRootPath, environmentName);
                            cfg.Configuration = null;
                        }
                    });
                }

                if (!string.IsNullOrEmpty(nlogConfigFile))
                {
                    provider.LogFactory.Setup().LoadConfigurationFromFile(nlogConfigFile, optional: true);
                }
            }

            if (provider.Options.ShutdownOnDispose || !provider.Options.AutoShutdown)
            {
                provider.LogFactory.AutoShutdown = false;
            }

            return provider;
        }

        private static string ResolveEnvironmentNLogConfigFile(string basePath, string environmentName)
        {
            if (!string.IsNullOrWhiteSpace(basePath))
            {
                if (!string.IsNullOrWhiteSpace(environmentName))
                {
                    var nlogConfigEnvFilePath = System.IO.Path.Combine(basePath, $"nlog.{environmentName}.config");
                    if (System.IO.File.Exists(nlogConfigEnvFilePath))
                        return System.IO.Path.GetFullPath(nlogConfigEnvFilePath);
                    nlogConfigEnvFilePath = System.IO.Path.Combine(basePath, $"NLog.{environmentName}.config");
                    if (System.IO.File.Exists(nlogConfigEnvFilePath))
                        return System.IO.Path.GetFullPath(nlogConfigEnvFilePath);
                }

                var nlogConfigFilePath = System.IO.Path.Combine(basePath, "nlog.config");
                if (System.IO.File.Exists(nlogConfigFilePath))
                    return System.IO.Path.GetFullPath(nlogConfigFilePath);
                nlogConfigFilePath = System.IO.Path.Combine(basePath, "NLog.config");
                if (System.IO.File.Exists(nlogConfigFilePath))
                    return System.IO.Path.GetFullPath(nlogConfigFilePath);
            }

            if (!string.IsNullOrWhiteSpace(environmentName))
                return $"nlog.{environmentName}.config";

            return null;
        }

        private static bool IsLoggingConfigurationLoaded(LoggingConfiguration cfg)
        {
            return cfg?.LoggingRules?.Count > 0 && cfg?.AllTargets?.Count > 0;
        }

        private static IConfiguration SetupNLogConfigSettings(IServiceProvider serviceProvider, IConfiguration configuration, LogFactory logFactory)
        {
            configuration = configuration ?? (serviceProvider?.GetService(typeof(IConfiguration)) as IConfiguration);
            logFactory.Setup()
                .SetupExtensions(ext => ext.RegisterConfigSettings(configuration).RegisterNLogWeb(serviceProvider))
                .SetupLogFactory(ext => ext.AddCallSiteHiddenAssembly(typeof(AspNetExtensions).GetTypeInfo().Assembly));
            return configuration;
        }

        private static bool TryLoadConfigurationFromSection(NLogLoggerProvider loggerProvider, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(loggerProvider.Options.LoggingConfigurationSectionName))
                return false;

            var nlogConfig = configuration.GetSection(loggerProvider.Options.LoggingConfigurationSectionName);
            if (nlogConfig?.GetChildren()?.Any() == true)
            {
                loggerProvider.LogFactory.Setup().LoadConfiguration(configBuilder =>
                {
                    if (configBuilder.Configuration.LoggingRules.Count == 0 && configBuilder.Configuration.AllTargets.Count == 0)
                    {
                        configBuilder.Configuration = new NLogLoggingConfiguration(nlogConfig, loggerProvider.LogFactory);
                    }
                });
                return true;
            }
            else
            {
                Common.InternalLogger.Debug("Skip loading NLogLoggingConfiguration from empty config section: {0}", loggerProvider.Options.LoggingConfigurationSectionName);
                return false;
            }
        }
    }
}
