#if ASP_NET_CORE3
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Whether synchronous IO is allowed for the Body
    /// Uses IHttpBodyControlFeature
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-body-synchronous-io-allowed}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-body-synchronous-io-allowed")]
    public class AspNetRequestBodySynchronousIOAllowedLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var value = features?.Get<IHttpBodyControlFeature>()?.AllowSynchronousIO ?? false;
            builder.Append(value ? '1' : '0');
        }
    }
}
#endif
