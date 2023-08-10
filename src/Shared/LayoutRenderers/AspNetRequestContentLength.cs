using System;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET request contentlength of the posted body
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-contentlength}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-ContentLength-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-contentlength")]
    public class AspNetRequestContentLength : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();

            long? contentLength = httpRequest?.ContentLength;
            if (contentLength > 0L)
            {
                builder.Append(contentLength.Value);
            }
        }
    }
}
