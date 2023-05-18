﻿using System;
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
    public class AspNetQueryStringLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// If empty, then render all querystrings
        /// </summary>
        [DefaultParameter]
        public List<string> Items { get; set; }

        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// If empty, then render all querystrings
        /// </summary>
        [Obsolete("Instead use Items-property. Marked obsolete with NLog.Web 5.3")]
        public List<string> QueryStringKeys { get => Items; set => Items = value; }

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
        /// Initializes a new instance of the <see cref="AspNetQueryStringLayoutRenderer" /> class.
        /// </summary>
        public AspNetQueryStringLayoutRenderer()
        {
            Exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

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

            var queryStringKeys = Items?.Count > 0 ? Items :
#if !ASP_NET_CORE
                queryStrings.Keys.Cast<string>();
#else
                queryStrings.Keys;
#endif
            bool checkForExclude = (Items == null || Items.Count == 0) && Exclude?.Count > 0;
            var pairs = GetQueryStringValues(queryStrings, queryStringKeys, checkForExclude, Exclude);
            SerializePairs(pairs, builder, logEvent);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetQueryStringValues(
#if !ASP_NET_CORE
            NameValueCollection queryStrings,
#else
            IQueryCollection queryStrings,
#endif
            IEnumerable<string> queryStringKeys,
            bool checkForExclude,
            ICollection<string> excludeNames)
        {
            foreach (var queryKey in queryStringKeys)
            {
                if (checkForExclude && excludeNames.Contains(queryKey))
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
                    yield return new KeyValuePair<string, string>(queryKey, value);
            }
        }
    }
}