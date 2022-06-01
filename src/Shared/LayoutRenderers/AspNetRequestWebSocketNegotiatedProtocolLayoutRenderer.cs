using System.Text;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Web Socket Negotiated Protocol
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-negotiated-protocol}
    /// </remarks>
    [LayoutRenderer("aspnet-request-web-socket-negotiated-protocol")]
    public class AspNetRequestWebSocketNegotiatedProtocolLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Core HttpContext.WebSocketManager.NegotiatedProtocols variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            // Not available on .NET 3.5
#if ASP_NET_CORE
            // Not available in ASP.NET Core
#elif NET46_OR_GREATER
            var httpContext = HttpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return;
            }

            builder.Append(httpContext.WebSocketNegotiatedProtocol);
#endif
        }
    }
}
