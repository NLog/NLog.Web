using System;
using System.Text;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#endif
using NLog.LayoutRenderers;
using NLog.Web.Internal;

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
            var forwardedIp = string.Empty;
            var httpContext = HttpContextAccessor.HttpContext;

#if !ASP_NET_CORE
            var request = httpContext.TryGetRequest();

            if (request == null)
            {
                return;
            }

            if (CheckForwardedForHeader)
            {
                var forwardedHeader = request.Headers[ForwardedForHeader];

                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    var addresses = forwardedHeader.Split(',');
                    if (addresses.Length > 0)
                    {
                        forwardedIp = addresses[0];
                    }
                }
            }

            var ip = request.ServerVariables["REMOTE_ADDR"];
#else
            if (CheckForwardedForHeader && httpContext.Request.Headers.ContainsKey(ForwardedForHeader))
            {
                var forwardedHeaders = httpContext.Request.Headers.GetCommaSeparatedValues(ForwardedForHeader);

                if (forwardedHeaders.Length > 0)
                {
                    forwardedIp = forwardedHeaders[0];
                }
            }

            var ip = httpContext.Connection?.RemoteIpAddress?.ToString();
#endif
            builder.Append(string.IsNullOrEmpty(forwardedIp) ? ip : forwardedIp);
        }
    }
}