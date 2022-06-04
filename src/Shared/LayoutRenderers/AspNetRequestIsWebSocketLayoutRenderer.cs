using System.Text;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Is Request Web Socket
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-is-web-socket}
    /// </remarks>
    [LayoutRenderer("aspnet-request-is-web-socket")]
    public class AspNetRequestIsWebSocketLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Core HttpContext.WebSocketManager.IsWebSocketRequest variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            // Not available on .NET 3.5
#if ASP_NET_CORE
            var websockets = HttpContextAccessor.HttpContext?.WebSockets;
            builder.Append(websockets?.IsWebSocketRequest == true ? '1' : '0');
#elif NET46_OR_GREATER
            var httpContext = HttpContextAccessor.HttpContext;
            builder.Append(httpContext?.IsWebSocketRequest == true ? '1' : '0');
#endif
        }
    }
}
