using System;
using System.Text;
#if !ASP_NET_CORE
using System.Web;
using System.Collections.Specialized;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Referrer
    /// </summary>
    /// <example>
    /// <para>Example usage of ${aspnet-request-referrer}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-referrer} - Produces - Referrer URL String from the Request.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-referrer")]
    [ThreadSafe]
    public class AspNetRequestReferrerRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the Referrer URL from the HttpRequest <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
                return;

            string referrer = string.Empty;
#if !ASP_NET_CORE
            referrer = httpRequest.UrlReferrer?.ToString();
#else
            if (httpRequest.Headers.TryGetValue("Referer", out var referrerValue))
                referrer = referrerValue.ToString();
#endif
            builder.Append(referrer);

        }
    }
}
