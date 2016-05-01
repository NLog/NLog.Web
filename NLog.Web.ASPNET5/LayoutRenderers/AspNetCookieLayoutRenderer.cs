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
using NLog.Web.Enums;
using System;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    /// <para>Example usage of ${aspnet-request-cookie}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-cookie:OutputFormat=Flat}
    /// ${aspnet-request-cookie:OutputFormat=Json}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-cookie")]
    public class AspNetCookieLayoutRenderer : AspNetLayoutRendererBase
    {
        private const string doubleQuotes = "\"";
        private const string jsonStartBraces = "{";
        private const string jsonEndBraces = "}";
        private const string jsonElementSeparator = ",";

        private const string flatCookiesSeparator = "=";
        private const string flatItemSeperator = ",";


        /// <summary>
        /// Comma separated string of name of the cookies to rendered from Request.
        /// </summary>
        public string CookiesNames { get; set; }

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public AspNetCookieLayoutOutPutFormat OutputFormat { get; set; } = AspNetCookieLayoutOutPutFormat.Flat;

        /// <summary>
        /// Renders the ASP.NET Cookie appends it to the specified <see cref="StringBuilder" />.
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
                        var cookie = httpRequest.Cookies[items[i]];
                        this.SerializeCookie(cookie, builder, i);
                        builder.Append(cookie);
                    }
                }
            }
        }

#if !DNX
        private void SerializeCookie(HttpCookie cookie, StringBuilder builder, int index)
        {
            if (cookie != null)
            {
                switch (this.OutputFormat)
                {
                    case AspNetCookieLayoutOutPutFormat.Flat:
                        if (index > 0)
                            builder.Append($"{jsonElementSeparator}");

                        builder.Append($"{jsonStartBraces}{doubleQuotes}{cookie.Name}{flatCookiesSeparator}{cookie.Value}{doubleQuotes}{jsonEndBraces}");
                        break;
                    case AspNetCookieLayoutOutPutFormat.Json:
                        if (index > 0)
                            builder.Append($"{flatItemSeperator}");

                        builder.Append($"{cookie.Name}{flatCookiesSeparator}{cookie.Value}");
                        break;
                }
            }
        }
#endif

#if DNX
        private void SerializeCookie(StringValues cookie, StringBuilder builder, int index)
        {
            switch (this.OutputFormat)
            {
                case AspNetCookieLayoutOutPutFormat.Flat:
                    if (index > 0)
                        builder.Append($"{flatItemSeperator}");

                    builder.Append($"{cookie}");
                    break;
                case AspNetCookieLayoutOutPutFormat.Json:
                    if (index > 0)
                        builder.Append($"{jsonElementSeparator}");

                    builder.Append($"{jsonStartBraces}{doubleQuotes}{cookie}{doubleQuotes}{jsonEndBraces}");
                    break;
            }
        }
#endif
    }
}
