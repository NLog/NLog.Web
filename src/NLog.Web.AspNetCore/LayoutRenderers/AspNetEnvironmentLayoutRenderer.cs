using System;
using System.Text;
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Rendering development environment. <see cref="IHostingEnvironment" />
    /// </summary>
    [LayoutRenderer("aspnet-environment")]
    [ThreadAgnostic]
    public class AspNetEnvironmentLayoutRenderer : AspNetHostEnvironmentLayoutRendererBase
    {
        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(HostEnvironment?.EnvironmentName);
        }
    }
}
