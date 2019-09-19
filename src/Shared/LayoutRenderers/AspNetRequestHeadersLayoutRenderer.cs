using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if !ASP_NET_CORE
using System.Collections.Specialized;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Headers
    /// </summary>
    /// <para>Example usage of ${aspnet-request-headers}</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-headers:OutputFormat=Flat}
    /// ${aspnet-request-headers:OutputFormat=Json}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-headers")]
    [ThreadSafe]
    public class AspNetRequestHeadersLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Header names to be rendered.
        /// If <c>null</c> or empty array, all headers will be rendered.
        /// </summary>
        public List<string> HeaderNames { get; set; }

        /// <summary>
        /// Renders the ASP.NET Headers appends it to the specified <see cref="StringBuilder" />.
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

            var headers = httpRequest.Headers;
            var headerNames = GetHeaderNames(headers)?.ToList();
            if (headerNames?.Count > 0 && headers?.Count > 0)
            {
                var headerValues = GetHeaders(headers, headerNames);
                SerializePairs(headerValues, builder, logEvent);
            }
        }

#if !ASP_NET_CORE
        private IEnumerable<string> GetHeaderNames(NameValueCollection headers)
#else
        private IEnumerable<string> GetHeaderNames(IHeaderDictionary headers)
#endif
        {
            if (HeaderNames != null && HeaderNames.Any())
                return HeaderNames;
            
            var keys = headers.Keys;

#if !ASP_NET_CORE
            return keys.Cast<string>();
#else
            return keys;
#endif
        }

#if !ASP_NET_CORE
        private IEnumerable<KeyValuePair<string, string>> GetHeaders(NameValueCollection headers, IEnumerable<string> headerNames)
#else
        private IEnumerable<KeyValuePair<string, string>> GetHeaders(IHeaderDictionary headers, IEnumerable<string> headerNames)
#endif
        {
            foreach (var headerName in headerNames)
            {
#if !ASP_NET_CORE
                var headerValue = headers[headerName];
                if (headerValue == null)
#else
                if (!headers.TryGetValue(headerName, out var headerValue))
#endif
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(headerName, headerValue);
            }
        }
    }
}