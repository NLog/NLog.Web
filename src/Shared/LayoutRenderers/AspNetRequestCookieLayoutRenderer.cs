using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if !ASP_NET_CORE
using NLog.Web.Enums;
using System.Collections.Specialized;
using System.Web;
using Cookies = System.Web.HttpCookieCollection;
#else
using Microsoft.AspNetCore.Http;
using Cookies = Microsoft.AspNetCore.Http.IRequestCookieCollection;
#endif

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
    [ThreadSafe]
    public class AspNetRequestCookieLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Cookie names to be rendered.
        /// If <c>null</c> or empty array, all cookies will be rendered.
        /// </summary>
        public List<string> CookieNames { get; set; }

        /// <summary>
        /// Renders the ASP.NET Cookie appends it to the specified <see cref="StringBuilder" />.
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

            var cookies = httpRequest.Cookies;
            var cookieNames = GetCookieNames(cookies)?.ToList();
            if (cookieNames?.Count > 0 && cookies?.Count > 0)
            {
                var cookieValues = GetCookies(cookies, cookieNames);
                SerializePairs(cookieValues, builder, logEvent);
            }
        }

        /// <summary>
        /// Get cookies names to render
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        private IEnumerable<string> GetCookieNames(Cookies cookies)
        {
            if (CookieNames != null && CookieNames.Any())
                return CookieNames;
            
            var keys = cookies.Keys;

#if !ASP_NET_CORE
            return keys.Cast<string>();
#else
            return keys;
#endif
        }

#if !ASP_NET_CORE
        private IEnumerable<KeyValuePair<string, string>> GetCookies(HttpCookieCollection cookies, IEnumerable<string> cookieNames)
        {
            foreach (var cookieName in cookieNames)
            {
                var httpCookie = cookies[cookieName];
                if (httpCookie == null)
                {
                    continue;
                }

                if (OutputFormat == AspNetRequestLayoutOutputFormat.Json)
                {
                    // Split multi-valued cookie, as allowed for in the HttpCookie API for backwards compatibility with classic ASP
                    var isFirst = true;
                    foreach (var multiValueKey in httpCookie.Values.AllKeys)
                    {
                        var cookieKey = multiValueKey;
                        if (isFirst)
                        {
                            cookieKey = cookieName;
                            isFirst = false;
                        }
                        yield return new KeyValuePair<string, string>(cookieKey, httpCookie.Values[multiValueKey]);
                    }
                }
                else
                {
                    yield return new KeyValuePair<string, string>(cookieName, httpCookie.Value);
                }
            }
        }
#else
        private IEnumerable<KeyValuePair<string, string>> GetCookies(IRequestCookieCollection cookies, IEnumerable<string> cookieNames)
        {
            foreach (var cookieName in cookieNames)
            {
                if (!cookies.TryGetValue(cookieName, out var cookieValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(cookieName, cookieValue);
            }
        }
#endif
    }
}