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
        internal static IHostEnvironment RetrieveHostEnvironment(IServiceProvider serviceProvider, LoggingConfiguration loggingConfiguration)
        {
            return ServiceLocator.ResolveService<IHostEnvironment>(serviceProvider, loggingConfiguration);
        }
#endif

        private IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        internal IHostEnvironment HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment =
#if ASP_NET_CORE
                    RetrieveHostEnvironment(ResolveService<IServiceProvider>(), LoggingConfiguration)
#else

                    Internal.HostEnvironment.Default
#endif
                );
            set => _hostEnvironment = value;
        }

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE
            builder.Append(GetCachedAppBasePath(HostEnvironment?.ContentRootPath));
#else
            builder.Append(GetCachedAppBasePath(HostEnvironment?.MapPath("~")));
#endif
        }

        private string _appBasePath;

        private string GetCachedAppBasePath(string primaryDirectory)
        {
            return _appBasePath ?? (_appBasePath = ResolveAppBasePath(primaryDirectory));
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

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            HostEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}