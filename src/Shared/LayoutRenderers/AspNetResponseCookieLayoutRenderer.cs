using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;
#if !ASP_NET_CORE
using System.Collections.Specialized;
using System.Web;
using Cookies = System.Web.HttpCookieCollection;
#else
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Response Cookie
    /// </summary>
    /// <example>
    /// <para>Example usage of ${aspnet-response-cookie}</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-response-cookie:OutputFormat=Flat}
    /// ${aspnet-response-cookie:OutputFormat=JsonArray}
    /// ${aspnet-response-cookie:OutputFormat=JsonDictionary}
    /// ${aspnet-response-cookie:OutputFormat=JsonDictionary:CookieNames=username}
    /// ${aspnet-response-cookie:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-response-cookie")]
    public class AspNetResponseCookieLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Cookie names to be rendered.
        /// If <c>null</c> or empty array, all cookies will be rendered.
        /// </summary>
        public List<string> CookieNames { get; set; }

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
        /// Initializes a new instance of the <see cref="AspNetResponseCookieLayoutRenderer" /> class.
        /// </summary>
        public AspNetResponseCookieLayoutRenderer()
        {
            Exclude = new HashSet<string>(new[] { "AUTH", "SESS_ID" }, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Renders the ASP.NET Cookie appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor.HttpContext.TryGetResponse();
            if (httpResponse == null)
            {
                return;
            }

            var cookies = GetCookies(httpResponse);
            if (cookies.Count > 0)
            {
                bool checkForExclude = (CookieNames == null || CookieNames.Count == 0) && Exclude?.Count > 0;
                var cookieValues = GetCookieValues(cookies, checkForExclude);
                SerializePairs(cookieValues, builder, logEvent);
            }
        }

#if !ASP_NET_CORE

        /// <summary>
        /// Method to get cookies for .NET Framework
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private Cookies GetCookies(HttpResponseBase response)
        {
            return response.Cookies;
        }

        private List<string> GetCookieNames(HttpCookieCollection cookies)
        {
            return CookieNames?.Count > 0 ? CookieNames : cookies.Keys.Cast<string>().ToList();
        }

        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(HttpCookieCollection cookies, bool checkForExclude)
        {
            var response = new List<KeyValuePair<string, string>>();
            var cookieNames = GetCookieNames(cookies);
            foreach (var cookieName in cookieNames)
            {
                if (checkForExclude && Exclude.Contains(cookieName))
                    continue;

                var httpCookie = cookies[cookieName];
                if (httpCookie == null)
                {
                    continue;
                }
                response.AddRange(GetCookieValue(httpCookie,cookieName));
            }
            return response;
        }

        private IEnumerable<KeyValuePair<string, string>> GetCookieValue(HttpCookie httpCookie, string cookieName)
        {
            if (OutputFormat != AspNetRequestLayoutOutputFormat.Flat)
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

#else
        /// <summary>
        /// Method to get cookies for all ASP.NET Core versions
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static IList<SetCookieHeaderValue> GetCookies(HttpResponse response)
        {
            var queryResults = response.Headers[HeaderNames.SetCookie];
            if (queryResults.Count > 0 && SetCookieHeaderValue.TryParseList(queryResults, out var result))
                return result;
            else
                return Array.Empty<SetCookieHeaderValue>();
        }

        private List<string> GetCookieNames(IEnumerable<SetCookieHeaderValue> cookies)
        {
            return CookieNames?.Count > 0 ? CookieNames : cookies.Select(row => row.Name.ToString()).ToList();
        }

        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(IEnumerable<SetCookieHeaderValue> cookies, bool checkForExclude)
        {
            var cookieNames = GetCookieNames(cookies);
            foreach (var cookieName in cookieNames)
            {
                if (checkForExclude && Exclude.Contains(cookieName))
                    continue;

                var httpCookie = cookies.SingleOrDefault(cookie => cookie.Name.ToString() == cookieName);
                if (httpCookie == null)
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(cookieName, httpCookie.Value.ToString());
            }
        }
#endif
    }
}