using System.Text;
#if !NETSTANDARD_1plus
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
    public class AspNetRequestCookieLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// List Cookie Key as String to be rendered from Request.
        /// </summary>
        public List<string> CookieNames { get; set; }

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public AspNetRequestLayoutOutputFormat OutputFormat { get; set; } = AspNetRequestLayoutOutputFormat.Flat;

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
                var cookieValues = GetCookies(cookies);
                SerializeValues(cookieValues, builder, this.OutputFormat);
            }
        }


#if !NETSTANDARD_1plus

        private IEnumerable<KeyValuePair<string, string>> GetCookies(HttpCookieCollection cookies)
        {
            var cookieNames = this.CookieNames;
            if (cookieNames != null)
            {
                foreach (var cookieName in cookieNames)
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
