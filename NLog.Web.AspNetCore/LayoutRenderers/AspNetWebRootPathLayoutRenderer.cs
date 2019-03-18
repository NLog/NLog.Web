using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web.DependencyInjection;

#else
using System.Web.Hosting;
#endif

namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering WebRootPath. <see cref="IHostingEnvironment" />
    /// </summary>
#else
    /// <summary>
    /// Rendering WebRootPath. <see cref="HostingEnvironment.MapPath"/>("/")
    /// </summary>
#endif
    [LayoutRenderer("aspnet-webrootpath")]
    [ThreadAgnostic]
    [ThreadSafe]
    public class AspNetWebRootPathLayoutRenderer : LayoutRenderer
    {
#if ASP_NET_CORE
        private static IHostingEnvironment _hostingEnvironment;

        private static IHostingEnvironment HostingEnvironment => _hostingEnvironment ?? (_hostingEnvironment = ServiceLocator.ServiceProvider?.GetService<IHostingEnvironment>());

        private string WebRootPath => HostingEnvironment?.WebRootPath;
#else
        private string WebRootPath => _webRootPath ?? (_webRootPath = HostingEnvironment.MapPath("/"));
        private static string _webRootPath;
#endif

        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(WebRootPath);
        }

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
#if ASP_NET_CORE
            _hostingEnvironment = null;
#else
            _webRootPath = null;
#endif
            base.CloseLayoutRenderer();
        }
    }
}