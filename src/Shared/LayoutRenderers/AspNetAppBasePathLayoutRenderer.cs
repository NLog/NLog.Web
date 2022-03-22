using System;
using System.Text;
#if ASP_NET_CORE
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using NLog.Web.DependencyInjection;
#else
using System.Web.Hosting;
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
    /// Rendering Application BasePath. <see cref="HostingEnvironment.MapPath"/>("~")
    /// </summary>
#endif
    [LayoutRenderer("aspnet-appbasepath")]
    [ThreadAgnostic]
    public class AspNetAppBasePathLayoutRenderer : LayoutRenderer
    {
#if ASP_NET_CORE
        private IHostEnvironment HostEnvironment => _hostEnvironment ?? (_hostEnvironment = ServiceLocator.ResolveService<IHostEnvironment>(ResolveService<IServiceProvider>(), LoggingConfiguration));
        private IHostEnvironment _hostEnvironment;

        private string AppBasePath => _appBasePath ?? ResolveAppBasePath(HostEnvironment?.ContentRootPath, out _appBasePath);
        private string _appBasePath;
#else
        private string AppBasePath => _appBasePath ?? ResolveAppBasePath(HostingEnvironment.MapPath("~"), out _appBasePath);
        private static string _appBasePath;
#endif

        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(AppBasePath);
        }

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
#if ASP_NET_CORE
            _hostEnvironment = null;
#endif
            _appBasePath = null;
            base.CloseLayoutRenderer();
        }

        private static string ResolveAppBasePath(string primaryDirectory, out string appBasePath)
        {
            if (string.IsNullOrEmpty(primaryDirectory))
            {
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

                appBasePath = null;
                return primaryDirectory;
            }
            else
            {
                appBasePath = primaryDirectory;
                return appBasePath;
            }
        }
    }
}