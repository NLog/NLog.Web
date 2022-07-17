#if ASP_NET_CORE3
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
    [LayoutRenderer("aspnet-request-http-transport-type")]
    public class AspNetRequestHttpTransportTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var value = features?.Get<IHttpTransportFeature>()?.TransportType ?? 
                                        Microsoft.AspNetCore.Http.Connections.HttpTransportType.None;
            builder.Append(value);
        }
    }
}
#endif
