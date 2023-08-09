using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Web Socket Requested Protocols
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-web-socket-requested-protocols:OutputFormat=Flat}
    /// ${aspnet-request-web-socket-requested-protocols:OutputFormat=JsonArray}
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-WebSocket-Requested-Protocols-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-web-socket-requested-protocols")]
    public class AspNetRequestWebSocketRequestedProtocolsLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var protocolValues = ResolveWebSocketProtocols();
            if (protocolValues?.Count > 0)
            {
                SerializeValues(protocolValues, builder, logEvent);
            }
        }

        System.Collections.Generic.IList<string> ResolveWebSocketProtocols()
        {
#if ASP_NET_CORE
            var websockets = HttpContextAccessor.HttpContext.TryGetWebSocket();
            return websockets?.WebSocketRequestedProtocols;
#elif NET46_OR_GREATER
            var httpContext = HttpContextAccessor.HttpContext;
            return httpContext?.WebSocketRequestedProtocols;
#else
            return null;    // Not available on .NET 3.5
#endif
        }
    }
}
