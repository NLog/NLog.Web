using System.Text;
#if !NETSTANDARD_1plus
using System.Web;
using System.Collections.Specialized;
#else
using Microsoft.Extensions.Primitives;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using NLog.Web.Enums;
using System;
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
    public class AspNetCookieLayoutRenderer : AspNetLayoutRendererBase
    {
        private const string flatCookiesSeparator = "=";
        private const string flatItemSeperator = ",";

        /// <summary>
        /// List Cookie Key as String to be rendered from Request.
        /// </summary>
        public List<string> CookieNames { get; set; }

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public AspNetLayoutOutputFormat OutputFormat { get; set; } = AspNetLayoutOutputFormat.Flat;

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

            if (this.CookieNames?.Count > 0 && httpRequest?.Cookies?.Count > 0)
            {
                bool firstItem = true;
                foreach (var cookieName in this.CookieNames)
                {
                    this.SerializeCookie(httpRequest.Cookies[cookieName], builder, firstItem);
                    firstItem = false;
                }
            }
        }

#if !NETSTANDARD_1plus
        /// <summary>
        /// To Serialize the HttpCookie based on the configured output format.
        /// </summary>
        /// <param name="cookie">The current cookie item.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="firstItem">Whether it is first item.</param>
        private void SerializeCookie(HttpCookie cookie, StringBuilder builder, bool firstItem)
        {
            if (cookie != null)
            {
                var cookieRaw = $"{cookie.Name}{flatCookiesSeparator}{cookie.Value}";

                SerializeCookie(cookieRaw, builder, firstItem);
            }
        }

#endif
        private void SerializeCookie(string cookieRaw, StringBuilder builder, bool firstItem)
        {
            switch (this.OutputFormat)
            {
                case AspNetLayoutOutputFormat.Flat:
                    if (!firstItem)
                        builder.Append($"{flatItemSeperator}");
                    builder.Append(cookieRaw);
                    break;
                case AspNetLayoutOutputFormat.Json:
                    if (!firstItem)
                        builder.Append($"{GlobalConstants.jsonElementSeparator}");

                    builder.Append($"{GlobalConstants.jsonElementStartBraces}{GlobalConstants.doubleQuotes}{cookieRaw}{GlobalConstants.doubleQuotes}{GlobalConstants.jsonElementEndBraces}");
                    break;
            }
        }
    }
}
