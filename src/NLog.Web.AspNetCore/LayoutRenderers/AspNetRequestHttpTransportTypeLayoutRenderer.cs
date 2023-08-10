#if NETCOREAPP3_0_OR_GREATER
using Microsoft.AspNetCore.Http.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Specifies transports that the client can use to send HTTP requests.
    ///
    /// This enumeration supports a bitwise combination of its member values.
    ///
    /// None
    /// WebSockets
    /// ServerSentEvents
    /// LongPolling
    ///
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-http-transport-type}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-HTTP-Transport-Type-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-http-transport-type")]
    public class AspNetRequestHttpTransportTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var transportFeature = HttpContextAccessor?.HttpContext?.TryGetFeature<IHttpTransportFeature>();
            var value = transportFeature?.TransportType ?? Microsoft.AspNetCore.Http.Connections.HttpTransportType.None;
            if (value != Microsoft.AspNetCore.Http.Connections.HttpTransportType.None)
                builder.Append(value);
        }
    }
}
#endif
