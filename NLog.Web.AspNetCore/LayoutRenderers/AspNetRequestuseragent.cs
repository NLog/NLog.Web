using System.Text;
#if !NETSTANDARD_1plus
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
    /// <para>Example usage of ${aspnet-useragent}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-useragent} - Produces - User Agent String from the Request.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-useragent")]
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
#if !NETSTANDARD_1plus
            userAgent = httpRequest.UserAgent;

#else
            userAgent = httpRequest.Headers["User-Agent"].ToString();
#endif

            builder.Append(userAgent);

        }
    }
}
