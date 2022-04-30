using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <example>
    /// <para>Example usage of ${aspnet-request-headers}</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-headers:OutputFormat=Flat}
    /// ${aspnet-request-headers:OutputFormat=JsonArray}
    /// ${aspnet-request-headers:OutputFormat=JsonDictionary}
    /// ${aspnet-request-headers:OutputFormat=JsonDictionary:HeaderNames=username}
    /// ${aspnet-request-headers:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-headers")]
    public class AspNetRequestHeadersLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Header names to be rendered.
        /// If <c>null</c> or empty array, all headers will be rendered.
        /// </summary>
        public List<string> HeaderNames { get; set; }

        /// <summary>
        /// Gets or sets the keys to exclude from the output. If omitted, none are excluded.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
#if ASP_NET_CORE
        public ISet<string> Exclude { get; set; }
#else
        public HashSet<string> Exclude { get; set; }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetRequestHeadersLayoutRenderer" /> class.
        /// </summary>
        public AspNetRequestHeadersLayoutRenderer()
        {
            Exclude = new HashSet<string>(new[] { "ALL_HTTP", "ALL_RAW", "AUTH_PASSWORD" }, StringComparer.OrdinalIgnoreCase);
        }

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
            if (headers?.Count > 0)
            {
                bool checkForExclude = (HeaderNames == null || HeaderNames.Count == 0) && Exclude?.Count > 0;
                var headerValues = GetHeaderValues(headers, checkForExclude);
                SerializePairs(headerValues, builder, logEvent);
            }
        }

#if !ASP_NET_CORE
        private IEnumerable<KeyValuePair<string, string>> GetHeaderValues(NameValueCollection headers, bool checkForExclude)
        {
            var headerNames = HeaderNames?.Count > 0 ? HeaderNames : headers.Keys.Cast<string>();
            foreach (var headerName in headerNames)
            {
                if (checkForExclude && Exclude.Contains(headerName))
                    continue;

                var headerValue = headers[headerName];
                if (headerValue == null)
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(headerName, headerValue);
            }
        }
#else
        private IEnumerable<KeyValuePair<string, string>> GetHeaderValues(IHeaderDictionary headers, bool checkForExclude)
        {
            var headerNames = HeaderNames?.Count > 0 ? HeaderNames : headers.Keys;
            foreach (var headerName in headerNames)
            {
                if (checkForExclude && Exclude.Contains(headerName))
                    continue;

                if (!headers.TryGetValue(headerName, out var headerValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(headerName, headerValue);
            }
        }
#endif
    }
}