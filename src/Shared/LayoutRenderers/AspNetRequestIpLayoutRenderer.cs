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
    /// <code>
    /// ${aspnet-request-ip} - Return the Remote IP
    /// ${aspnet-request-ip:CheckForwardedForHeader=true} - Return first element in the X-Forwarded-For header
    /// ${aspnet-request-ip:CheckForwardedForHeaderOffset=1} - Return second element in the X-Forwarded-For header
    /// ${aspnet-request-ip:CheckForwardedForHeaderOffset=-1} - Return last element in the X-Forwarded-For header
    /// ${aspnet-request-ip:CheckForwardedForHeader=true:ForwardedForHeader=myHeader} - Return first element in the myHeader header
    /// </code>
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
        /// If a negative index is used, this is used as the position from the end of the array.
        /// Minus one will indicate the last element in the array.  If the negative index is too large the first index
        /// of the array is returned instead.
        /// </summary>
        public int CheckForwardedForHeaderOffset
        {
            get => _checkForwardedForHeaderOffset;
            set
            {
                _checkForwardedForHeaderOffset = value;
                CheckForwardedForHeader = true;
            }
        }
        private int _checkForwardedForHeaderOffset;

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor?.HttpContext;

            var httpRequest = httpContext.TryGetRequest();
            if (httpRequest is null)
                return;

            var ip = CheckForwardedForHeader && ForwardedForHeader != null ? TryLookupForwardHeader(httpRequest, logEvent) : string.Empty;

            if (string.IsNullOrEmpty(ip))
            {
#if !ASP_NET_CORE
                ip = httpRequest.ServerVariables?["REMOTE_ADDR"];
#else
                ip = httpContext?.Connection?.RemoteIpAddress?.ToString();
#endif
            }

            builder.Append(ip);
        }

        private int CalculatePosition(string[] headerContents)
        {
            var position = CheckForwardedForHeaderOffset;

            if (position < 0)
            {
                position = headerContents.Length + position;
            }
            if (position < 0)
            {
                position = 0;
            }
            if (position >= headerContents.Length)
            {
                position = headerContents.Length - 1;
            }
            return position;
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
                    var position = CalculatePosition(addresses);
                    return addresses[position]?.Trim();
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
                    var position = CalculatePosition(forwardedHeaders);
                    return forwardedHeaders[position]?.Trim();
                }
            }

            return string.Empty;
        }
#endif
    }
}