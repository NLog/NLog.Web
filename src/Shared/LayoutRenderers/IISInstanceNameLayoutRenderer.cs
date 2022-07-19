using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Hosting;
#else
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
#endif

namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering site name in IIS. <see cref="IHostingEnvironment.ApplicationName" />
    /// </summary>
#else
    /// <summary>
    /// Rendering site name in IIS. <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
#endif
    [LayoutRenderer("iis-site-name")]
    // ReSharper disable once InconsistentNaming
    [ThreadAgnostic]
    public class IISInstanceNameLayoutRenderer : AspNetHostEnvironmentLayoutRendererBase
    {
        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE
            builder.Append(HostEnvironment?.ApplicationName);
#else
            builder.Append(HostingEnvironment.SiteName);
#endif
        }
    }
}
