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
    /// ${aspnet-request-referrer}
    /// </remarks>
    [LayoutRenderer("aspnet-request-referrer")]
    public class AspNetRequestReferrerRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the Referrer URL from the HttpRequest <see cref="StringBuilder" />.
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