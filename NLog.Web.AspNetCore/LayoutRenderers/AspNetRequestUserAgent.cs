using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

#if !ASP_NET_CORE
using System.Collections.Specialized;
using System.Web;
#endif

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
    [ThreadSafe]
    public class AspNetRequestUserAgent : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the ASP.NET User Agent
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

            var userAgent = string.Empty;
#if !ASP_NET_CORE
            userAgent = httpRequest.UserAgent;
#else
            if (httpRequest.Headers.TryGetValue("User-Agent", out var userAgentValue))
            {
                userAgent = userAgentValue.ToString();
            }
#endif
            builder.Append(userAgent);
        }
    }
}