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
    [LayoutRenderer("aspnet-request-contentlength")]
    public class AspNetRequestContentLength : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            long? contentLength = httpRequest.ContentLength;
            if (contentLength > 0L)
            {
                builder.Append(contentLength.Value);
            }
        }
    }
}
