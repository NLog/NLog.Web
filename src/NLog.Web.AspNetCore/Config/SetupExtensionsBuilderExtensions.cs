using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Web.DependencyInjection;

namespace NLog.Web
{
    /// <summary>
    /// Extension methods to setup NLog extensions, so they are known when loading NLog LoggingConfiguration
    /// </summary>
    public static class SetupExtensionsBuilderExtensions
    {
        /// <summary>
        /// Replace with version from NLog.Extension.Logging when it has been released with NLog 4.7
        /// </summary>
        internal static ISetupExtensionsBuilder RegisterConfigSettings(this ISetupExtensionsBuilder setupBuilder, IConfiguration configuration)
        {
            ConfigSettingLayoutRenderer.DefaultConfiguration = configuration;
            return setupBuilder.RegisterLayoutRenderer<ConfigSettingLayoutRenderer>("configsetting");
        }

        /// <summary>
        /// Register the NLog.Web.AspNetCore LayoutRenderers
        /// </summary>
        /// <remarks>
        /// If not providing <paramref name="serviceProvider"/>, then output from aspnet-layoutrenderers will remain empty
        /// </remarks>
        public static ISetupExtensionsBuilder RegisterNLogWeb(this ISetupExtensionsBuilder setupBuilder, IServiceProvider serviceProvider = null)
        {
            if (serviceProvider != null)
            {
                ServiceLocator.ServiceProvider = serviceProvider;
            }

            return setupBuilder.RegisterAssembly(typeof(NLogAspNetCoreOptions).GetTypeInfo().Assembly);
        }
    }
}
