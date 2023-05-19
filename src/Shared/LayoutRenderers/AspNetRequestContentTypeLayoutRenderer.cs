using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpRequest Content-Type Header
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-contenttype}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-ContentType-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-contenttype")]
    public class AspNetRequestContentTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = HttpContextAccessor.HttpContext.TryGetRequest();
            var contentType = request?.ContentType;
            builder.Append(contentType);
        }
    }
}