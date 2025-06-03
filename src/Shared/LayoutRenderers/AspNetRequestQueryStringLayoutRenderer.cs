using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !ASP_NET_CORE
using System.Collections.Specialized;
#else
using Microsoft.AspNetCore.Http;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Query String
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-querystring:OutputFormat=Flat}
    /// ${aspnet-request-querystring:OutputFormat=JsonArray}
    /// ${aspnet-request-querystring:OutputFormat=JsonDictionary}
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-QueryString-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-querystring")]
    public class AspNetRequestQueryStringLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// If empty, then render all querystrings
        /// </summary>
        [DefaultParameter]
        public List<string>? Items { get; set; }

        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// If empty, then render all querystrings
        /// </summary>
        [Obsolete("Instead use Items-property. Marked obsolete with NLog.Web 5.3")]
        public List<string>? QueryStringKeys { get => Items; set => Items = value; }

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
        /// Initializes a new instance of the <see cref="AspNetRequestQueryStringLayoutRenderer" /> class.
        /// </summary>
        public AspNetRequestQueryStringLayoutRenderer()
        {
            Exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext.TryGetRequest();
            if (httpRequest is null)
                return;

#if !ASP_NET_CORE
            var queryStrings = httpRequest.QueryString;
#else
            var queryStrings = httpRequest.Query;
#endif
            if (queryStrings is null || queryStrings.Count == 0)
                return;

            var queryStringKeys = Items?.Count > 0 ? Items :
#if !ASP_NET_CORE
                queryStrings.Keys.Cast<string>();
#else
                queryStrings.Keys;
#endif
            var checkForExclude = (Exclude?.Count > 0 && (Items is null || Items.Count == 0)) ? Exclude : null;
            var pairs = GetQueryStringValues(queryStrings, queryStringKeys, checkForExclude);
            SerializePairs(pairs, builder, logEvent);
        }

        private static IEnumerable<KeyValuePair<string, string?>> GetQueryStringValues(
#if !ASP_NET_CORE
            NameValueCollection queryStrings,
#else
            IQueryCollection queryStrings,
#endif
            IEnumerable<string> queryStringKeys,
            ICollection<string>? checkForExclude)
        {
            foreach (var queryKey in queryStringKeys)
            {
                if (checkForExclude?.Contains(queryKey) == true)
                    continue;

#if !ASP_NET_CORE
                var value = queryStrings[queryKey];
#else
                if (!queryStrings.TryGetValue(queryKey, out var objValue))
                {
                    continue;
                }

                var value = objValue.ToString();
#endif
                if (!string.IsNullOrEmpty(value))
                    yield return new KeyValuePair<string, string?>(queryKey, value);
            }
        }
    }
}