#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Synchronously retrieves the exception associated with the rejected HTTP request.
    /// Uses IBadRequestExceptionFeature
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-bad-request-exception}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-bad-request-exception")]
    public class AspNetRequestBadRequestExceptionLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var badRequestExceptionFeature = features?.Get<IBadRequestExceptionFeature>();
            builder.Append(badRequestExceptionFeature?.Error?.ToString());
        }
    }
}
#endif
