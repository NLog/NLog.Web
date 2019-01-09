using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpRequest Content-Type Header
    /// </summary>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-contenttype}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-contenttype")]
    [ThreadSafe]
    public class AspNetRequestContentTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = HttpContextAccessor.HttpContext.TryGetRequest();
            if (request == null)
                return;

            var contentType = request.ContentType;
            if (!string.IsNullOrEmpty(contentType))
                builder.Append(contentType);
        }
    }
}