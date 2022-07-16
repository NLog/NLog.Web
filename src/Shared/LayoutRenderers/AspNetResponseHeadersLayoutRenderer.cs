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
    /// ASP.NET Response Headers
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-response-headers:OutputFormat=Flat}
    /// ${aspnet-response-headers:OutputFormat=JsonArray}
    /// ${aspnet-response-headers:OutputFormat=JsonDictionary}
    /// ${aspnet-response-headers:OutputFormat=JsonDictionary:HeaderNames=username}
    /// ${aspnet-response-headers:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </remarks>
    [LayoutRenderer("aspnet-response-headers")]
    public class AspNetResponseHeadersLayoutRenderer : AspNetLayoutMultiValueRendererBase
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
        /// Initializes a new instance of the <see cref="AspNetResponseHeadersLayoutRenderer" /> class.
        /// </summary>
        public AspNetResponseHeadersLayoutRenderer()
        {
            Exclude = new HashSet<string>(new[] { "ALL_HTTP", "ALL_RAW", "AUTH_PASSWORD" }, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor.HttpContext.TryGetResponse();
            if (httpResponse == null)
            {
                return;
            }

            var headers = httpResponse.Headers;
            if (headers?.Count > 0)
            {
                bool checkForExclude = (HeaderNames == null || HeaderNames.Count == 0) && Exclude?.Count > 0;
                var headersDictionary = GetHeaders(headers);
                var headerValues = GetHeaderValues(headersDictionary, checkForExclude);
                SerializePairs(headerValues, builder, logEvent);
            }
        }

#if!ASP_NET_CORE
        private static Dictionary<string, string> GetHeaders(NameValueCollection headers)
        {
            return headers.Keys.Cast<string>().ToDictionary(headerKey => headerKey, headerKey => headers[headerKey]);
        }
#else
        private static Dictionary<string, string> GetHeaders(IHeaderDictionary headers)
        {
            return headers.Keys.ToDictionary<string, string, string>(headerKey => headerKey, headerKey => headers[headerKey]);
        }
#endif
        private IEnumerable<KeyValuePair<string, string>> GetHeaderValues(Dictionary<string,string> headers, bool checkForExclude)
        {
            var headerNames = HeaderNames?.Count > 0 ? HeaderNames : headers.Keys.ToList();
            foreach (var headerName in headerNames)
            {
                if (checkForExclude && Exclude.Contains(headerName))
                {
                    continue;
                }

                if (!headers.ContainsKey(headerName))
                {
                    continue;
                }

                if (!headers.TryGetValue(headerName, out var headerValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(headerName, headerValue);
            }
        }
    }
}