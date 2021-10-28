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
    /// ${aspnet-request-contentlength}
    /// </remarks>
    [LayoutRenderer("aspnet-request-contentlength")]
    public class AspNetRequestContentLength : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the ASP.NET posted body
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
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
