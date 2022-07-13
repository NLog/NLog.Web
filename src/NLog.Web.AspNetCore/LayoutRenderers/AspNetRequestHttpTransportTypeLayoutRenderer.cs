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
    [LayoutRenderer("aspnet-request-http-transport-type")]
    public class AspNetRequestHttpTransportTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        ///<inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if (features == null)
            {
                return;
            }
            var httpTransportFeature = features.Get<IHttpTransportFeature>();
            if (httpTransportFeature == null)
            {
                return;
            }
            builder.Append(httpTransportFeature.TransportType);
        }
    }
}
#endif
