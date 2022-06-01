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
    /// ${aspnet-request-contenttype}
    /// </remarks>
    [LayoutRenderer("aspnet-response-contenttype")]
    public class AspNetResponseContentTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
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