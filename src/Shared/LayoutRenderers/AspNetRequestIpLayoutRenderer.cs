using System.ComponentModel;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Web.Internal;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request IP address of the remote client
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-ip}</code> to return the Remote IP
    /// <code>${aspnet-request-ip:CheckForwardedForHeader=true}</code> to return first element in the X-Forwarded-For header
    /// <code>${aspnet-request-ip:CheckForwardedForHeader=true:Index=1}</code> to return second element in the X-Forwarded-For header
    /// <code>${aspnet-request-ip:CheckForwardedForHeader=true:Index=1:ForwardedForHeader=myHeader}</code> to return second element in the myHeader header
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-IP-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-ip")]
    public class AspNetRequestIpLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// The header name to check for the Forwarded-For. Default "X-Forwarded-For". Needs <see cref="CheckForwardedForHeader"/>
        /// </summary>
        [DefaultValue("X-Forwarded-For")]
        public Layout ForwardedForHeader { get; set; } = "X-Forwarded-For";

        /// <summary>
        /// Gets or sets whether the renderer should check value of <see cref="ForwardedForHeader"/> header
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public bool CheckForwardedForHeader { get; set; }

        /// <summary>
        /// Gets or sets the array index of the X-Forwarded-For header to use, if the desired client IP is not at
        /// the zeroth index.  Defaults to zero.  If the index is too large the last array element is returned instead.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;

            var request = httpContext.TryGetRequest();
            if (request == null)
            {
                return;
            }

            var ip = CheckForwardedForHeader && ForwardedForHeader != null ? TryLookupForwardHeader(request, logEvent) : string.Empty;

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
        string TryLookupForwardHeader(HttpRequestBase httpRequest, LogEventInfo logEvent)
        {
            var headerName = ForwardedForHeader.Render(logEvent);
            var forwardedHeader = httpRequest.Headers[headerName];

            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                var addresses = forwardedHeader.Split(',');
                if (addresses.Length > 0)
                {
                    if (Index >= addresses.Length)
                    {
                        Index = addresses.Length - 1;
                    }
                    return addresses[Index]?.Trim();
                }
            }

            return string.Empty;
        }
#else
        private string TryLookupForwardHeader(HttpRequest httpRequest, LogEventInfo logEvent)
        {
            var headerName = ForwardedForHeader.Render(logEvent);
            if (httpRequest.Headers?.ContainsKey(headerName) == true)
            {
                var forwardedHeaders = httpRequest.Headers.GetCommaSeparatedValues(headerName);
                if (forwardedHeaders.Length > 0)
                {
                    if (Index >= forwardedHeaders.Length)
                    {
                        Index = forwardedHeaders.Length - 1;
                    }
                    return forwardedHeaders[Index]?.Trim();
                }
            }

            return string.Empty;
        }
#endif
    }
}