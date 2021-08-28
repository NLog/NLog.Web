using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Hosting;
#else
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.DependencyInjection;
using NLog.Web.DependencyInjection;
#endif

namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering site name in IIS. <see cref="IHostingEnvironment" />
    /// </summary>
#else
    /// <summary>
    /// Rendering site name in IIS. <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
#endif
    [LayoutRenderer("iis-site-name")]
    // ReSharper disable once InconsistentNaming
    [ThreadAgnostic]
    [ThreadSafe]
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {
#if ASP_NET_CORE
        private static IHostEnvironment _hostEnvironment;

        private static IHostEnvironment HostEnvironment => _hostEnvironment ?? (_hostEnvironment = ServiceLocator.ServiceProvider?.GetService<IHostEnvironment>());
#endif

        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE
            builder.Append(HostEnvironment?.ApplicationName);
#else
            builder.Append(HostingEnvironment.SiteName);
#endif
        }

#if ASP_NET_CORE
        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _hostEnvironment = null;
            base.CloseLayoutRenderer();
        }
#endif
    }
}