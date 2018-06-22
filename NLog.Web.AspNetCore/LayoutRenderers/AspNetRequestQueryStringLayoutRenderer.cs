﻿using System.Text;
#if !ASP_NET_CORE
using System.Web;
using System.Collections.Specialized;
#else
using Microsoft.AspNetCore.Http;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using NLog.Web.Enums;
using System;
using System.Linq;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    /// <para>Example usage of ${aspnet-request-querystring}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-querystring:OutputFormat=Flat}
    /// ${aspnet-request-querystring:OutputFormat=Json}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-querystring")]
    public class AspNetQueryStringLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// If empty, then render all querystrings
        /// </summary>
        public List<String> QueryStringKeys { get; set; }

        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();
            if (httpRequest == null)
                return;




            var printAllQueryString = this.QueryStringKeys == null || this.QueryStringKeys.Count == 0;
            var queryStringKeys = this.QueryStringKeys;
#if !ASP_NET_CORE
            var queryStrings = httpRequest.QueryString;

            if (queryStrings == null)
                return;


            if (printAllQueryString)
            {
                queryStringKeys = new List<string>(queryStrings.Keys.Count);

                foreach (var key in queryStrings.Keys)
                {
                    if (key != null)
                    {
                        queryStringKeys.Add(key.ToString());
                    }
                }
            }
#else
            var queryStrings = httpRequest.Query;

            if (queryStrings == null)
                return;

            if (printAllQueryString)
            {
                queryStringKeys = queryStrings.Keys.ToList();
            }
#endif

            var values = GetValues(queryStrings, queryStringKeys);
            SerializePairs(values, builder);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetValues(
#if ASP_NET_CORE
            IQueryCollection queryStrings,
#else
            NameValueCollection queryStrings,
#endif
            IEnumerable<string> queryStringKeys)

        {
            if (queryStrings.Count > 0)
            {
                foreach (var key in queryStringKeys)
                {
                    // This platoform specific code is to prevent an unncessary .ToString call otherwise. 
#if !ASP_NET_CORE
                    var value = queryStrings[key];
#else
                    var value = queryStrings[key].ToString();
#endif
                    if (!String.IsNullOrEmpty(value))
                    {
                        yield return new KeyValuePair<string, string>(key, value);
                    }
                }
            }
        }
    }
}
