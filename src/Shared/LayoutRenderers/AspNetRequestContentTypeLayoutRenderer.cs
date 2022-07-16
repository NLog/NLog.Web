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
    [LayoutRenderer("aspnet-request-contenttype")]
    public class AspNetRequestContentTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = HttpContextAccessor.HttpContext.TryGetRequest();
            if (request == null)
            {
                return;
            }

            var contentType = request.ContentType;
            if (!string.IsNullOrEmpty(contentType))
            {
                builder.Append(contentType);
            }
        }
    }
}