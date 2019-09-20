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
    /// ${aspnet-request-cookie:OutputFormat=Json:CookieNames=username}
    /// ${aspnet-request-cookie:OutputFormat=Json:Exclude=access_token}
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
        /// Gets or sets the keys to exclude from the output. If omitted, none are excluded.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
#if ASP_NET_CORE
        public ISet<string> Exclude { get; set; }
#else
        public HashSet<string> Exclude { get; set; }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetRequestCookieLayoutRenderer" /> class.
        /// </summary>
        public AspNetRequestCookieLayoutRenderer()
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
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            var cookies = httpRequest.Cookies;
            if (cookies?.Count > 0)
            {
                bool checkForExclude = (CookieNames == null || CookieNames.Count == 0) && Exclude?.Count > 0;
                var cookieValues = GetCookieValues(cookies, checkForExclude);
                SerializePairs(cookieValues, builder, logEvent);
            }
        }

#if !ASP_NET_CORE
        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(HttpCookieCollection cookies, bool checkForExclude)
        {
            var cookieNames = CookieNames?.Count > 0 ? CookieNames : cookies.Keys.Cast<string>().ToList();
            foreach (var cookieName in cookieNames)
            {
                if (checkForExclude && Exclude.Contains(cookieName))
                    continue;

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
        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(IRequestCookieCollection cookies, bool checkForExclude)
        {
            var cookieNames = CookieNames?.Count > 0 ? CookieNames : cookies.Keys;
            foreach (var cookieName in cookieNames)
            {
                if (checkForExclude && Exclude.Contains(cookieName))
                    continue;

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