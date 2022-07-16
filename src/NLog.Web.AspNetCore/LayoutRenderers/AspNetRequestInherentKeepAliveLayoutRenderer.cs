#if ASP_NET_CORE3
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Indicates if the connection transport has an "inherent keep-alive",
    /// which means that the transport will automatically inform the client that it is still present.
    ///
    /// The most common example of this feature is the Long Polling HTTP transport,
    /// which must (due to HTTP limitations) terminate each poll within a particular interval
    /// and return a signal indicating "the server is still here, but there is no data yet".
    /// This feature allows applications to add keep-alive functionality, but limit it only to
    /// transports that don't have some kind of inherent keep-alive.
    /// </summary>
    [LayoutRenderer("aspnet-request-inherent-keep-alive")]
    public class AspNetRequestInherentKeepAliveLayoutRenderer : AspNetLayoutRendererBase
    {
        ///<inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var value = features?.Get<IConnectionInherentKeepAliveFeature>()?.HasInherentKeepAlive ?? false;
            builder.Append(value ? '1' : '0');
        }
    }
}
#endif
