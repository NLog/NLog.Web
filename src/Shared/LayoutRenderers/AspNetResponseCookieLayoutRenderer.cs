using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;
#if !ASP_NET_CORE
using System.Web;
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
                var cookieValues = GetCookieValues(cookies);
                SerializePairs(cookieValues, builder, logEvent);
            }
        }

#if !ASP_NET_CORE

        /// <summary>
        /// Method to get cookies for .NET Framework
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private HttpCookieCollection GetCookies(HttpResponseBase response)
        {
            return response.Cookies;
        }

        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(HttpCookieCollection cookies)
        {
            var expandMultiValue = OutputFormat != AspNetRequestLayoutOutputFormat.Flat;
            return HttpCookieCollectionValues.GetCookieValues(cookies, CookieNames, Exclude, expandMultiValue);
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

        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(IList<SetCookieHeaderValue> cookies)
        {
            if (CookieNames?.Count > 0)
            {
                return GetCookieNameValues(cookies, CookieNames);
            }
            else
            {
                return GetCookieAllValues(cookies, Exclude);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetCookieNameValues(IList<SetCookieHeaderValue> cookies, List<string> cookieNames)
        {
            foreach (var needle in cookieNames)
            {
                for (int i = 0; i < cookies.Count; ++i)
                {
                    var cookie = cookies[i];
                    var cookieName = cookie.Name.ToString();
                    if (string.Equals(needle, cookieName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new KeyValuePair<string, string>(cookieName, cookie.Value.ToString());
                    }
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetCookieAllValues(IList<SetCookieHeaderValue> cookies, ICollection<string> excludeNames)
        {
            bool checkForExclude = excludeNames?.Count > 0;
            for (int i = 0; i < cookies.Count; ++i)
            {
                var cookie = cookies[i];
                var cookieName = cookie.Name.ToString();
                if (checkForExclude && excludeNames.Contains(cookieName))
                    continue;

                yield return new KeyValuePair<string, string>(cookieName, cookie.Value.ToString());
            }
        }
#endif
    }
}