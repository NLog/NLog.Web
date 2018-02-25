using System;
using System.Text;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web.DependencyInjection;
#else
using System.Web.Hosting;
#endif
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering Application BasePath. <see cref="IHostingEnvironment.ContentRootPath"/> (Previous IApplicationEnvironment.ApplicationBasePath)
    /// </summary>
#else
    /// <summary>
    /// Rendering Application BasePath. <see cref="HostingEnvironment.MapPath"/>("~")
    /// </summary>
#endif
    [LayoutRenderer("aspnet-appbasepath")]
    public class AspNetAppBasePathLayoutRenderer : LayoutRenderer
    {
#if ASP_NET_CORE
        private static IHostingEnvironment _hostingEnvironment;

        private static IHostingEnvironment HostingEnvironment => _hostingEnvironment ?? (_hostingEnvironment = ServiceLocator.ServiceProvider?.GetService<IHostingEnvironment>());

        private string AppBasePath => HostingEnvironment?.ContentRootPath ?? System.IO.Directory.GetCurrentDirectory();
#else
        private string AppBasePath => _appBasePath ?? (_appBasePath = HostingEnvironment.MapPath("~"));
        private static string _appBasePath;
#endif

        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(AppBasePath);
        }

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
#if ASP_NET_CORE
            _hostingEnvironment = null;
#else
            _appBasePath = null;
#endif
            base.CloseLayoutRenderer();
        }
    }
}