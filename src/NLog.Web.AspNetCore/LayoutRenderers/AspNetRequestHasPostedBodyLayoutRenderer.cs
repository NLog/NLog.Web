#if NET5_0_OR_GREATER
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Used to indicate if the request has a body.
    /// Uses IHttpRequestBodyDetectionFeature
    ///
    /// This returns true when:
    /// - It's an HTTP/1.x request with a non-zero Content-Length or a 'Transfer-Encoding: chunked' header.
    /// - It's an HTTP/2 request that did not set the END_STREAM flag on the initial headers frame.
    /// The final request body length may still be zero for the chunked or HTTP/2 scenarios.
    ///
    /// This returns false when:
    /// - It's an HTTP/1.x request with no Content-Length or 'Transfer-Encoding: chunked' header, or the Content-Length is 0.
    /// - It's an HTTP/1.x request with Connection: Upgrade(e.g.WebSockets).
    ///     There is no HTTP request body for these requests and no data should be received until after the upgrade.
    /// - It's an HTTP/2 request that set END_STREAM on the initial headers frame. When false, the request body should never return data.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-has-body}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-Has-Posted-Body-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-has-posted-body")]
    public class AspNetRequestHasPostedBodyLayoutRenderer : AspNetLayoutRendererBase
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
