using System;
#if NETCOREAPP3_0_OR_GREATER
using System.IO;
using System.Linq;
#endif
using Microsoft.Extensions.Configuration;
using NLog.Config;
using NLog.Extensions.Logging;

namespace NLog.Web
{
    /// <summary>
    /// Extension methods to setup LogFactory options
    /// </summary>
    public static class SetupBuilderExtensions
    {
#if NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// Loads NLog LoggingConfiguration from appsettings.json from the NLog-section
        /// </summary>
        /// <param name="setupBuilder"></param>
        /// <param name="basePath">Override SetBasePath for <see cref="ConfigurationBuilder"/> with AddJsonFile. Default resolves from environment variables, else fallback to current directory.</param>
        /// <param name="environment">Override Environment for appsettings.{environment}.json with AddJsonFile. Default resolves from environment variables, else fallback to "Production"</param>
        /// <param name="nlogConfigSection">Override configuration-section-name to resolve NLog-configuration</param>
        /// <param name="optional">Override optional with AddJsonFile</param>
        /// <param name="reloadOnChange">Override reloadOnChange with AddJsonFile. Required for "autoReload":true to work.</param>
        public static ISetupBuilder LoadConfigurationFromAppSettings(this ISetupBuilder setupBuilder, string basePath = null, string environment = null, string nlogConfigSection = "NLog", bool optional = true, bool reloadOnChange = false)
        {
            environment = environment ?? GetAspNetCoreEnvironment("ASPNETCORE_ENVIRONMENT") ?? GetAspNetCoreEnvironment("DOTNET_ENVIRONMENT") ?? "Production";
            basePath = basePath ?? GetAspNetCoreEnvironment("ASPNETCORE_CONTENTROOT") ?? GetAspNetCoreEnvironment("DOTNET_CONTENTROOT");

            var currentBasePath = basePath;
            if (currentBasePath is null)
            {
                currentBasePath = Environment.CurrentDirectory;

                var normalizeCurDir = Path.GetFullPath(currentBasePath).TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                var normalizeAppDir = Path.GetFullPath(AppContext.BaseDirectory).TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                if (string.IsNullOrWhiteSpace(normalizeCurDir) || normalizeAppDir.IndexOf(normalizeCurDir, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    currentBasePath = AppContext.BaseDirectory; // Avoid using Windows-System32 as current directory
                }
            }

            var builder = new ConfigurationBuilder()
                // Host Configuration
                .SetBasePath(currentBasePath)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddEnvironmentVariables(prefix: "DOTNET_")
                // App Configuration
                .AddJsonFile("appsettings.json", optional, reloadOnChange)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: reloadOnChange)
                .AddEnvironmentVariables();

            var config = builder.Build();
            if (!string.IsNullOrEmpty(nlogConfigSection) && config.GetSection(nlogConfigSection)?.GetChildren().Any() == true)
            {
                // "NLog"-section in appsettings.json has first priority
                return setupBuilder.SetupExtensions(e => e.RegisterNLogWeb().RegisterConfigSettings(config)).LoadConfigurationFromSection(config, nlogConfigSection);
            }
            else
            {
                setupBuilder.SetupExtensions(e => e.RegisterNLogWeb().RegisterConfigSettings(config));

                if (!string.IsNullOrEmpty(basePath))
                {
                    if (!string.IsNullOrEmpty(environment))
                    {
                        setupBuilder.LoadConfigurationFromFile(Path.Combine(basePath, $"nlog.{environment}.config"), optional: true);
                        setupBuilder.LoadConfiguration(config =>
                        {
                            if (!IsLoggingConfigurationLoaded(config.Configuration))
                            {
                                // Fallback when environment-specific NLog config could not load
                                var nlogConfigFilePath = Path.Combine(basePath, "nlog.config");
                                if (File.Exists(nlogConfigFilePath))
                                    config.Configuration = new XmlLoggingConfiguration(nlogConfigFilePath, config.LogFactory);
                            }
                        });
                    }
                    else
                    {
                        setupBuilder.LoadConfigurationFromFile(Path.Combine(basePath, "nlog.config"), optional: true);
                    }
                }
                else if (!string.IsNullOrEmpty(environment))
                {
                    setupBuilder.LoadConfigurationFromFile($"nlog.{environment}.config", optional: true);
                }

                return setupBuilder.LoadConfigurationFromFile();    // No effect, if config already loaded
            }
        }

        private static bool IsLoggingConfigurationLoaded(LoggingConfiguration cfg)
        {
            return cfg?.LoggingRules?.Count > 0 && cfg?.AllTargets?.Count > 0;
        }

        private static string GetAspNetCoreEnvironment(string variableName)
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable(variableName);
                if (string.IsNullOrWhiteSpace(environment))
                    return null;

                return environment.Trim();
            }
            catch (Exception ex)
            {
                NLog.Common.InternalLogger.Error(ex, "Failed to lookup environment variable {0}", variableName);
                return null;
            }
        }
#endif

        /// <summary>
        /// Convience method to register aspnet-layoutrenders in NLog.Web as one-liner before loading NLog.config
        /// </summary>
        /// <remarks>
        /// If not providing <paramref name="serviceProvider"/>, then output from aspnet-layoutrenderers will remain empty
        /// </remarks>
        public static ISetupBuilder RegisterNLogWeb(this ISetupBuilder setupBuilder, IConfiguration configuration = null, IServiceProvider serviceProvider = null)
        {
            setupBuilder.SetupExtensions(e => e.RegisterNLogWeb(serviceProvider));

            if (configuration == null && serviceProvider != null)
            {
                configuration = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            }

            setupBuilder.SetupExtensions(e => e.RegisterConfigSettings(configuration));
            return setupBuilder;
        }
    }
}
