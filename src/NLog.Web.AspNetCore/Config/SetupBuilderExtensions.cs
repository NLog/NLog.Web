using System;
#if !ASP_NET_CORE2
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
#if !ASP_NET_CORE2
        /// <summary>
        /// Loads NLog LoggingConfiguration from appsettings.json from the NLog-section
        /// </summary>
        public static ISetupBuilder LoadConfigurationFromAppSettings(this ISetupBuilder setupBuilder, string basePath = null, string environment = null, string nlogConfigSection = "NLog", bool optional = true, bool reloadOnChange = false)
        {
            environment = environment ?? GetAspNetCoreEnvironment("ASPNETCORE_ENVIRONMENT") ?? GetAspNetCoreEnvironment("DOTNET_ENVIRONMENT") ?? "Production";

            var currentBasePath = basePath;
            if (currentBasePath is null)
            {
                currentBasePath = AppContext.BaseDirectory;
                if (string.IsNullOrEmpty(currentBasePath))
                {
                    currentBasePath = Directory.GetCurrentDirectory();
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
                return setupBuilder.SetupExtensions(e => e.RegisterNLogWeb()).LoadConfigurationFromSection(config, nlogConfigSection);
            }
            else
            {
                setupBuilder.SetupExtensions(e => e.RegisterNLogWeb().RegisterConfigSettings(config));

                if (!string.IsNullOrEmpty(basePath))
                {
                    if (!string.IsNullOrEmpty(environment))
                    {
                        setupBuilder.LoadConfigurationFromFile(Path.Combine(basePath, $"nlog.{environment}.config"), optional: true);
                    }

                    setupBuilder.LoadConfigurationFromFile(Path.Combine(basePath, "nlog.config"), optional: true);
                }
                else if (!string.IsNullOrEmpty(environment))
                {
                    setupBuilder.LoadConfigurationFromFile($"nlog.{environment}.config", optional: true);
                }

                return setupBuilder.LoadConfigurationFromFile();    // No effect, if config already loaded
            }
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

            if (configuration != null)
            { 
                setupBuilder.SetupExtensions(e => e.RegisterConfigSettings(configuration));
            }

            return setupBuilder;
        }
    }
}
