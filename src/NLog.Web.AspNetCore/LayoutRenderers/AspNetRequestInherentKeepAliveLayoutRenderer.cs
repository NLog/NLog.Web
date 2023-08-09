#if NETCOREAPP3_0_OR_GREATER
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
    /// <remarks>
    /// <code>${aspnet-request-inherent-keep-alive}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-Inherent-KeepAlive-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-inherent-keep-alive")]
    public class AspNetRequestInherentKeepAliveLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var connectionFeature = HttpContextAccessor.HttpContext?.TryGetFeature<IConnectionInherentKeepAliveFeature>();
            var value = connectionFeature?.HasInherentKeepAlive ?? false;
            builder.Append(value ? '1' : '0');
        }
    }
}
#endif
