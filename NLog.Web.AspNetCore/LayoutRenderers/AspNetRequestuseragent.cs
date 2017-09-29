using System.Text;
#if !ASP_NET_CORE
using System.Web;
using System.Collections.Specialized;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using System;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User Agent
    /// </summary>
    /// <para>Example usage of ${aspnet-request-useragent}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-useragent} - Produces - User Agent String from the Request.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-useragent")]
    public class AspNetRequestUserAgent : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the ASP.NET User Agent
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();

            if (httpRequest == null)
                return;

            string userAgent = string.Empty;
#if !ASP_NET_CORE
            userAgent = httpRequest.UserAgent;

#else
            userAgent = httpRequest.Headers["User-Agent"].ToString();
#endif

            builder.Append(userAgent);

        }
    }
}
