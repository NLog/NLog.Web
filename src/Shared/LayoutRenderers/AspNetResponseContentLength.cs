using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Response ContentLength
    /// </summary>
    /// <remarks>
    /// ${aspnet-response-contentlength}
    /// </remarks>
    [LayoutRenderer("aspnet-response-contentlength")]
    public class AspNetResponseContentLength : AspNetLayoutRendererBase
    {
        /// <summary>
        /// ASP.NET Http Response Status Code
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor.HttpContext.TryGetResponse();
            if (httpResponse == null)
            {
                return;
            }

#if ASP_NET_CORE
            var contentLength = httpResponse.ContentLength;
#else
            var contentLength = httpResponse.OutputStream?.Length;
#endif
            if (contentLength > 0L)
            {
                builder.Append(contentLength);
            }
        }
    }
}
