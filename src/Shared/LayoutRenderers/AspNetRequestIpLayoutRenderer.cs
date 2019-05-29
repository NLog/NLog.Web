using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;

#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Render the request IP for ASP.NET Core
    /// </summary>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-ip}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-ip")]
    [ThreadSafe]
    public class AspNetRequestIpLayoutRenderer : AspNetLayoutRendererBase
    {
        private const string ForwardedForHeader = "X-Forwarded-For";

        /// <summary>
        /// Gets or sets whether the renderer should check value of X-Forwarded-For header
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public bool CheckForwardedForHeader { get; set; }

        /// <summary>
        /// Render IP
        /// </summary>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;

            var request = httpContext.TryGetRequest();
            if (request == null)
            {
                return;
            }

            var ip = CheckForwardedForHeader ? TryLookupForwardHeader(request) : string.Empty;

            if (string.IsNullOrEmpty(ip))
            {
#if !ASP_NET_CORE
                ip = request.ServerVariables?["REMOTE_ADDR"];
#else
                ip = httpContext.Connection?.RemoteIpAddress?.ToString();
#endif
            }

            builder.Append(ip);
        }

#if !ASP_NET_CORE
        string TryLookupForwardHeader(System.Web.HttpRequestBase httpRequest)
        {
            var forwardedHeader = httpRequest.Headers[ForwardedForHeader];

            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                var addresses = forwardedHeader.Split(',');
                if (addresses.Length > 0)
                {
                    return addresses[0];
                }
            }

            return string.Empty;
        }
#else
        private string TryLookupForwardHeader(HttpRequest httpRequest)
        {
            if (httpRequest.Headers?.ContainsKey(ForwardedForHeader) == true)
            {
                var forwardedHeaders = httpRequest.Headers.GetCommaSeparatedValues(ForwardedForHeader);
                if (forwardedHeaders.Length > 0)
                {
                    return forwardedHeaders[0];
                }
            }

            return string.Empty;
        }
#endif
    }
}