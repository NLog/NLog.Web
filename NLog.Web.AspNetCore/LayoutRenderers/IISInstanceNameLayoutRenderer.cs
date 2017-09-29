using System;
using System.Text;
#if !ASP_NET_CORE
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endif
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{

#if ASP_NET_CORE
    /// <summary>
    /// Rendering site name in IIS. <see cref="IHostingEnvironment"/>
    /// </summary>
#else
    /// <summary>
    /// Rendering site name in IIS. <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
 #endif
    [LayoutRenderer("iis-site-name")]
    // ReSharper disable once InconsistentNaming
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {
#if ASP_NET_CORE
        private static IHostingEnvironment _hostingEnvironment;

        private static IHostingEnvironment HostingEnvironment => _hostingEnvironment ?? (_hostingEnvironment = ServiceLocator.ServiceProvider?.GetService<IHostingEnvironment>());
#endif

        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {


#if ASP_NET_CORE
            builder.Append(HostingEnvironment?.ApplicationName);

#else
            builder.Append(HostingEnvironment.SiteName);
#endif

        }
#if ASP_NET_CORE

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _hostingEnvironment = null;
            base.CloseLayoutRenderer();
        }
#endif
    }
}
