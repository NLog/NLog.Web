
using NLog.LayoutRenderers;
using System.Text;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Routing;
#endif
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET host.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer host.
    /// </remarks>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-host}    
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-host")]
    public class AspNetRequestHostLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = HttpContextAccessor?.HttpContext?.TryGetRequest();
#if ASP_NET_CORE
            var host = request?.Host;
#else
            var host = request?.UserHostName;
#endif

            if (host != null)
            {
                var hostString = host.ToString();

                if (!string.IsNullOrEmpty(hostString))
                    builder.Append(hostString);
            }
        }
    }
}
