using System.Text;
#if !DNX
using System.Web;
using System.Collections.Specialized;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Primitives;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using System;



namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    [LayoutRenderer("aspnet-request-cookie")]
    public class AspNetCookieLayoutRenderer : AspNetLayoutRendererBase
    {
        private static string doubleQuotes = "\"";
        private static string jsonStartBraces = "{";
        private static string jsonEndBraces = "}";
        private static string jsonElementSeparator = ",";

        private static string flatCookiesSeparator = "=";
        private static string flatItemSeperator = ",";


        /// <summary>
        /// Comma separated string of name of the cookies to rendered from Request.
        /// </summary>
        public string CookiesNames { get; set; }

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public string OutputFormat { get; set; } = "FLAT";

        /// <summary>
        /// Renders the ASP.NET Session ID appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (this.CookiesNames != null)
            {
                var items = this.CookiesNames.Split(new String[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

                if (items.Length > 0)
                {
                    var httpRequest = HttpContextAccessor.HttpContext.Request;

                    if (httpRequest?.Cookies?.Count == 0)
                        return;

                    for (int i = 0; i < items.Length; i++)
                    {
#if !DNX
                        var cookie = httpRequest.Cookies[items[i]];
                        this.SerializeCookie(cookie, builder, i);

#else
                        var cookie = httpRequest.Cookies[items[i]];
                        builder.Append(cookie);
#endif
                    }
                }
            }
        }

#if !DNX
        private void SerializeCookie(HttpCookie cookie, StringBuilder builder, int index)
        {
            if (cookie != null)
            {
                if (this.OutputFormat?.ToUpperInvariant() == "JSON")
                {
                    if (index > 0)
                        builder.Append($"{jsonElementSeparator}");

                    builder.Append($"{jsonStartBraces}{doubleQuotes}{cookie.Name}{flatCookiesSeparator}{cookie.Value}{doubleQuotes}{jsonEndBraces}");

                }
                else
                {
                    if (index > 0)
                        builder.Append($"{flatItemSeperator}");

                    builder.Append($"{cookie.Name}{flatCookiesSeparator}{cookie.Value}");
                }
            }
        }
#endif

#if DNX
        private void SerializeCookie(StringValues cookie, StringBuilder builder, int index)
        {

            if (this.OutputFormat?.ToUpperInvariant() == "JSON")
            {
                if (index > 0)
                    builder.Append($"{jsonElementSeparator}");

                builder.Append($"{jsonStartBraces}{doubleQuotes}{cookie}{doubleQuotes}{jsonEndBraces}");

            }
            else
            {
                if (index > 0)
                    builder.Append($"{flatItemSeperator}");

                builder.Append($"{cookie}");
            }
        }
#endif
    }
}
