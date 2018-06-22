﻿using System.Text;
#if !ASP_NET_CORE
using System.Collections.Specialized;
using System.Web;
#else
using Microsoft.Extensions.Primitives;
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
    /// <para>Example usage of ${aspnet-request-cookie}</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-cookie:OutputFormat=Flat}
    /// ${aspnet-request-cookie:OutputFormat=Json}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-cookie")]
    public class AspNetRequestCookieLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Cookie Key as String to be rendered from Request.
        /// </summary>
        public List<string> CookieNames { get; set; }

        /// <summary>
        /// Only render cookie values if true, otherwise render key value pairs.
        /// </summary>
        public bool ValuesOnly { get; set; }

        /// <summary>
        /// Renders the ASP.NET Cookie appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();

            if (httpRequest == null)
            {
                return;
            }

            var cookies = httpRequest.Cookies;

            if (this.CookieNames?.Count > 0 && cookies?.Count > 0)
            {
                var cookieKeyValuePairs = GetCookies(cookies);
                if (!ValuesOnly)
                {
                    SerializePairs(cookieKeyValuePairs, builder);
                }
                else
                {
                    SerializeValues(cookieKeyValuePairs, builder);
                }
            }
        }


#if !ASP_NET_CORE

        private IEnumerable<KeyValuePair<string, string>> GetCookies(HttpCookieCollection cookies)
        {
            if (CookieNames != null)
            {
                foreach (var cookieName in CookieNames)
                {
                    var value = cookies[cookieName];
                    
                    if (value != null)
                    {
                        if (this.OutputFormat == AspNetRequestLayoutOutputFormat.Json)
                        {
                            //split
                            var isFirst = true;
                            foreach (var key in value.Values.AllKeys)
                            {
                                var key2 = key;
                                if (isFirst)
                                {
                                    key2 = cookieName;
                                    isFirst = false;
                                }
                                yield return new KeyValuePair<string, string>(key2, value.Values[key]);
                            }
                        }
                        else
                        {
                            yield return new KeyValuePair<string, string>(cookieName, value.Value);
                        }
                    }
                }
            }
        }
#else

        private IEnumerable<KeyValuePair<string, string>> GetCookies(IRequestCookieCollection cookies)
        {
            var cookieNames = this.CookieNames;
            if (cookieNames != null)
            {
                foreach (var cookieName in cookieNames)
                {
                    if (cookies.TryGetValue(cookieName, out var cookieValue))
                    {
                        yield return new KeyValuePair<string, string>(cookieName, cookieValue);
                    }
                }
            }
        }
#endif


    }
}
