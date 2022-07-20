using System;
using System.Text;
#if ASP_NET_CORE
#if ASP_NET_CORE2
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using NLog.Web.DependencyInjection;
#else
using NLog.Web.Internal;
#endif
using NLog.Config;
using NLog.LayoutRenderers;


namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering Application BasePath. <see cref="IHostEnvironment.ContentRootPath" /> (Previous IApplicationEnvironment.ApplicationBasePath)
    /// </summary>
#else
    /// <summary>
    /// Rendering Application BasePath. <see cref="IHostEnvironment.MapPath"/>("~")
    /// </summary>
#endif
    [LayoutRenderer("aspnet-appbasepath")]
    [ThreadAgnostic]
    public class AspNetAppBasePathLayoutRenderer : LayoutRenderer
    {
#if ASP_NET_CORE
        /// <summary>
        /// Context for DI
        /// </summary>
        private IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        public IHostEnvironment HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment = RetrieveHostEnvironment(ResolveService<IServiceProvider>(), LoggingConfiguration));
            set => _hostEnvironment = value;
        }

        internal static IHostEnvironment RetrieveHostEnvironment(IServiceProvider serviceProvider, LoggingConfiguration loggingConfiguration)
        {
            return ServiceLocator.ResolveService<IHostEnvironment>(serviceProvider, loggingConfiguration);
        }
#else
        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        internal IHostEnvironment HostEnvironment { get; set; } = Internal.HostEnvironment.Default;
#endif

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE
            builder.Append(ResolveAppBasePath(HostEnvironment?.ContentRootPath));
#else
            builder.Append(ResolveAppBasePath(HostEnvironment?.MapPath("~")));
#endif
        }

        private static string ResolveAppBasePath(string primaryDirectory)
        {
            if (!string.IsNullOrEmpty(primaryDirectory))
            {
                return primaryDirectory;
            }
#if ASP_NET_CORE
            try
            {
                primaryDirectory = Environment.GetEnvironmentVariable("ASPNETCORE_CONTENTROOT");
            }
            catch
            {
                // Not supported or access denied
            }
            if (string.IsNullOrEmpty(primaryDirectory))
            {
                primaryDirectory = AppContext.BaseDirectory;
            }
#else
            primaryDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
            if (string.IsNullOrEmpty(primaryDirectory))
            {
                try
                {
                    primaryDirectory = System.IO.Directory.GetCurrentDirectory();
                }
                catch
                {
                    // Not supported or access denied
                }
            }
            return primaryDirectory;
        }
    }
}