using System;
using System.Collections.Generic;
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
    /// ASP.NET Request Query String
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-querystring:OutputFormat=Flat}
    /// ${aspnet-request-querystring:OutputFormat=JsonArray}
    /// ${aspnet-request-querystring:OutputFormat=JsonDictionary}
    /// </code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-querystring")]
    public class AspNetQueryStringLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// If empty, then render all querystrings
        /// </summary>
        public List<string> QueryStringKeys { get; set; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

#if !ASP_NET_CORE
            var queryStrings = httpRequest.QueryString;
#else
            var queryStrings = httpRequest.Query;
#endif
            if (queryStrings == null || queryStrings.Count == 0)
                return;

            var queryStringKeys = QueryStringKeys;
            var printAllQueryString = queryStringKeys == null || queryStringKeys.Count == 0;
            if (printAllQueryString)
            {
                queryStringKeys = new List<string>(queryStrings.Count);
                foreach (var key in queryStrings.Keys)
                {
                    if (key != null)
                    {
                        queryStringKeys.Add(key.ToString());
                    }
                }
            }

            var pairs = GetPairs(queryStrings, queryStringKeys);
            SerializePairs(pairs, builder, logEvent);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetPairs(
#if !ASP_NET_CORE
            NameValueCollection queryStrings,
#else
            IQueryCollection queryStrings,
#endif
            List<string> queryStringKeys)
        {
            foreach (var key in queryStringKeys)
            {
                // This platoform specific code is to prevent an unncessary .ToString call otherwise. 
#if !ASP_NET_CORE
                var value = queryStrings[key];
#else
                if (!queryStrings.TryGetValue(key, out var objValue))
                {
                    continue;
                }

                var value = objValue.ToString();
#endif
                if (!string.IsNullOrEmpty(value))
                {
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }
    }
}