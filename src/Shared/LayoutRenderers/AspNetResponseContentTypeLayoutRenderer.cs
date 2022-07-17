using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpResponse Content-Type Header
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-contenttype}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Response-ContentType-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-contenttype")]
    public class AspNetResponseContentTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var response = HttpContextAccessor.HttpContext.TryGetResponse();
            if (response == null)
            {
                return;
            }

            var contentType = response.ContentType;
            if (!string.IsNullOrEmpty(contentType))
            {
                builder.Append(contentType);
            }
        }
    }
}