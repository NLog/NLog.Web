using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !ASP_NET_CORE
using System.Web;
using NLog.Web.Enums;
#else
using Microsoft.AspNetCore.Http;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-cookie:OutputFormat=Flat}
    /// ${aspnet-request-cookie:OutputFormat=JsonArray}
    /// ${aspnet-request-cookie:OutputFormat=JsonDictionary}
    /// ${aspnet-request-cookie:OutputFormat=JsonDictionary:Items=username}
    /// ${aspnet-request-cookie:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-Cookie-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-cookie")]
    public class AspNetRequestCookieLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Cookie names to be rendered.
        /// If <c>null</c> or empty array, all cookies will be rendered.
        /// </summary>
        [DefaultParameter]
        public List<string> Items { get; set; }

        /// <summary>
        /// Cookie names to be rendered.
        /// If <c>null</c> or empty array, all cookies will be rendered.
        /// </summary>
        [Obsolete("Instead use Items-property. Marked obsolete with NLog.Web 5.3")]
        public List<string> CookieNames { get => Items; set => Items = value; }

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

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            var cookies = httpRequest?.Cookies;
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
            return HttpCookieCollectionValues.GetCookieValues(cookies, Items, Exclude, expandMultiValue);
        }
#else
        private IEnumerable<KeyValuePair<string, string>> GetCookieValues(IRequestCookieCollection cookies)
        {
            if (Items?.Count > 0)
            {
                foreach (var cookieName in Items)
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