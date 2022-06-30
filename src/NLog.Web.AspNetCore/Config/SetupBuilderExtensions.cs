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
        /// Loads NLog LoggingConfiguration from the NLog-section from json file, respecting environment
        /// </summary>
        public static ISetupBuilder LoadConfigurationFromAppSettings(this ISetupBuilder setupBuilder, string fileName = "appsettings.json", string basePath = null, string environment = null, string nlogConfigSection = "NLog", bool optional = true, bool reloadOnChange = false)
        {
            environment = environment ?? GetAspNetCoreEnvironment("ASPNETCORE_ENVIRONMENT") ?? GetAspNetCoreEnvironment("DOTNET_ENVIRONMENT") ?? "Production";

            string noextension = Path.GetFileNameWithoutExtension(fileName);

            basePath ??= AppContext.BaseDirectory;
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = Directory.GetCurrentDirectory();
            }

            return setupBuilder.LoadConfigurationUsingConfigBuilder(() =>
            {
                return new ConfigurationBuilder()
                    // Host Configuration
                    .SetBasePath(basePath)
                    .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                    .AddEnvironmentVariables(prefix: "DOTNET_")
                    // App Configuration
                    .AddJsonFile($"{noextension}.json", optional, reloadOnChange)
                    .AddJsonFile($"{noextension}.{environment}.json", optional: true, reloadOnChange: reloadOnChange)
                    .AddEnvironmentVariables();
            }, nlogConfigSection, basePath, environment);
        }

        /// <summary>
        /// Load configuration using your own builder pipeline
        /// </summary>
        public static ISetupBuilder LoadConfigurationUsingConfigBuilder(this ISetupBuilder setupBuilder, Func<IConfigurationBuilder> builderFunc, string nlogConfigSection = "NLog", string basePath = null, string environment = null)
        {
            if (builderFunc == null)
                throw new ArgumentException("Please define builder function.", nameof(builderFunc));

            IConfigurationBuilder builder = builderFunc();
            var config = builder.Build();
            if (!string.IsNullOrEmpty(nlogConfigSection) && config.GetSection(nlogConfigSection)?.GetChildren().Any() == true)
            {
                return setupBuilder.SetupExtensions(e => e.RegisterNLogWeb()).LoadConfigurationFromSection(config, nlogConfigSection);
            }

            //if no config section found load ${configsetting} from json and the rest from nlog.config
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
