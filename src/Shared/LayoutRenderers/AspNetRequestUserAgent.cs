using System;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request User Agent String
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-useragent}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-UserAgent-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-useragent")]
    public class AspNetRequestUserAgent : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
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