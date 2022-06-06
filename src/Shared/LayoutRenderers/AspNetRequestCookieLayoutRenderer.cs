using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if !ASP_NET_CORE
using NLog.Web.Enums;
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    /// <example>
    /// <para>Example usage of ${aspnet-request-cookie}</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-cookie:OutputFormat=Flat}
    /// ${aspnet-request-cookie:OutputFormat=JsonArray}
    /// ${aspnet-request-cookie:OutputFormat=JsonDictionary}
    /// ${aspnet-request-cookie:OutputFormat=JsonDictionary:CookieNames=username}
    /// ${aspnet-request-cookie:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-cookie")]
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
                var cookieValues = GetCookieValues(cookies);
                SerializePairs(cookieValues, builder, logEvent);
            }
        }

#if !ASP_NET_CORE
        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(HttpCookieCollection cookies)
        {
            var expandMultiValue = OutputFormat != AspNetRequestLayoutOutputFormat.Flat;
            return HttpCookieCollectionValues.GetCookieValues(cookies, CookieNames, Exclude, expandMultiValue);
        }
#else
        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(IRequestCookieCollection cookies)
        {
            if (CookieNames?.Count > 0)
            {
                foreach (var cookieName in CookieNames)
                {
                    if (cookies.TryGetValue(cookieName, out var cookieValue))
                    {
                        yield return new KeyValuePair<string, string>(cookieName, cookieValue);
                    }
                }
            }
            else
            {
                bool checkForExclude = Exclude?.Count > 0;
                foreach (var cookie in cookies)
                {
                    if (checkForExclude && Exclude.Contains(cookie.Key))
                        continue;

                    yield return new KeyValuePair<string, string>(cookie.Key, cookie.Value);
                }
            }
        }
#endif
    }
}