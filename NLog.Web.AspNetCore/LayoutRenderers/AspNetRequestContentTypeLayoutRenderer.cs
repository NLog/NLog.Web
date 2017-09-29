#if ASP_NET_CORE
using NLog.LayoutRenderers;
using System.Text;

using Microsoft.AspNetCore.Routing;

using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET content type.
    /// </summary>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-contenttype}    
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-contenttype")]
    public class AspNetRequestContentTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = HttpContextAccessor?.HttpContext?.TryGetRequest();

            var contentType = request?.ContentType;

            if (!string.IsNullOrEmpty(contentType))
                    builder.Append(contentType);
            
        }
    }
}
#endif