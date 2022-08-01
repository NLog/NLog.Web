#if NET5_0_OR_GREATER
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Used to indicate if the request can have a body.
    /// Uses IHttpRequestBodyDetectionFeature
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-can-have-body}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-can-have-body")]
    public class AspNetRequestCanHaveBodyLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var value = features?.Get<IHttpRequestBodyDetectionFeature>()?.CanHaveBody ?? false;
            builder.Append(value ? '1' : '0');
        }
    }
}
#endif
