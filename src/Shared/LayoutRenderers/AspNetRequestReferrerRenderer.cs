using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

#if !ASP_NET_CORE
using System.Web;
using System.Collections.Specialized;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Referrer URL String
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-referrer}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-Referrer-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-referrer")]
    public class AspNetRequestReferrerRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            var referrer = string.Empty;
#if !ASP_NET_CORE
            referrer = httpRequest.UrlReferrer?.ToString();
#else
            if (httpRequest.Headers.TryGetValue("Referer", out var referrerValue))
            {
                referrer = referrerValue.ToString();
            }
#endif
            builder.Append(referrer);
        }
    }
}