using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Web Socket Requested Protocols
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-web-socket-requested-protocols}
    /// </remarks>
    /// <example>
    /// <para>Example usage of ${aspnet-request-web-socket-requested-protocols}</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-web-socket-requested-protocols:OutputFormat=Flat}
    /// ${aspnet-request-web-socket-requested-protocols:OutputFormat=JsonArray}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-web-socket-requested-protocols")]
    public class AspNetRequestWebSocketRequestedProtocolsLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Core HttpContext.WebSocketManager.WebSocketRequestedProtocols variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            // Not available on .NET 3.5
#if ASP_NET_CORE
            var websockets = HttpContextAccessor.HttpContext.TryGetWebSocket();
            if (websockets == null)
            {
                return;
            }

            if (websockets.WebSocketRequestedProtocols == null)
            {
                return;

            }

            if (websockets.WebSocketRequestedProtocols.Count == 0)
            {
                return;
            }

            SerializeValues(websockets.WebSocketRequestedProtocols, builder, logEvent);

#elif NET46_OR_GREATER
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            if (httpContext.WebSocketRequestedProtocols == null)
            {
                return;

            }

            if (httpContext.WebSocketRequestedProtocols.Count == 0)
            {
                return;
            }

            SerializeValues(httpContext.WebSocketRequestedProtocols, builder, logEvent);
#endif
        }
    }
}
