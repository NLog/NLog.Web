using System;
using System.Text;
#if ASP_NET_CORE
using System.IO;
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.DependencyInjection;
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
        private static IHostEnvironment _hostEnvironment;

        private static IHostEnvironment HostEnvironment => _hostEnvironment ?? (_hostEnvironment = ServiceLocator.ServiceProvider?.GetService<IHostEnvironment>());

        private string AppBasePath => HostEnvironment?.ContentRootPath ?? Directory.GetCurrentDirectory();
#else
        private string AppBasePath => _appBasePath ?? (_appBasePath = HostingEnvironment.MapPath("~"));
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
#else
            _appBasePath = null;
#endif
            base.CloseLayoutRenderer();
        }
    }
}