#if NET5_0_OR_GREATER
using System.Text;
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// The error code for the protocol being used.  The property returns -1 if the error code hasn't been set.
    /// Uses IProtocolErrorCodeFeature
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-protocol-error-code}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-protocol-error-code")]
    public class AspNetRequestProtocolErrorCodeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var protocolErrorCodeFeature = features?.Get<IProtocolErrorCodeFeature>();
            builder.Append(protocolErrorCodeFeature?.Error ?? -1);
        }
    }
}
#endif