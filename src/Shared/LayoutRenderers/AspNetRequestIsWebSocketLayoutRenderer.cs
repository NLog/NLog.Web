using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Is Request Web Socket
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-is-web-socket}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-IsWebSocket-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-is-web-socket")]
    public class AspNetRequestIsWebSocketLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            // Not available on .NET 3.5
#if ASP_NET_CORE
            var websockets = HttpContextAccessor?.HttpContext?.TryGetWebSocket();
            builder.Append(websockets?.IsWebSocketRequest == true ? '1' : '0');
#elif NET46_OR_GREATER
            var httpContext = HttpContextAccessor?.HttpContext;
            builder.Append(httpContext?.IsWebSocketRequest == true ? '1' : '0');
#endif
        }
    }
}
