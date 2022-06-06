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
    /// ${aspnet-response-cookie:OutputFormat=JsonArray:CookieNames=username}
    /// ${aspnet-response-cookie:OutputFormat=JsonArray:Exclude=access_token}
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
        /// Render all of the cookie properties, such as Daom and Path, not merely Name and Value
        /// </summary>
        public bool Verbose { get; set; }

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
                if (!Verbose)
                {
                    var cookieValues = GetCookieValues(cookies);
                    SerializePairs(cookieValues, builder, logEvent);
                }
                else
                {
                    var verboseCookieValues = GetVerboseCookieValues(cookies);
                    SerializeAllProperties(verboseCookieValues, builder, logEvent);
                }
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

        private IEnumerable<HttpCookie> GetVerboseCookieValues(HttpCookieCollection cookies)
        {
            var expandMultiValue = OutputFormat != AspNetRequestLayoutOutputFormat.Flat;
            return HttpCookieCollectionValues.GetVerboseCookieValues(cookies, CookieNames, Exclude, expandMultiValue);
        }

        private void SerializeAllProperties(IEnumerable<HttpCookie> verboseCookieValues, StringBuilder builder, LogEventInfo logEvent)
        {
            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializeAllPropertiesFlat(verboseCookieValues, builder, logEvent);
                    break;
                case AspNetRequestLayoutOutputFormat.JsonArray:
                    SerializeAllPropertiesJson(verboseCookieValues, builder);
                    break;
            }
        }

        private void SerializeAllPropertiesJson(IEnumerable<HttpCookie> verboseCookieValues, StringBuilder builder)
        {
            var firstItem = true;

            foreach (var cookie in verboseCookieValues)
            {
                if (firstItem)
                {
                    builder.Append('[');
                }
                else
                {
                    builder.Append(',');
                }

                builder.Append("{");

                AppendJsonProperty(builder, "Name", cookie.Name.ToString());
                builder.Append(",");
                AppendJsonProperty(builder, "Value", cookie.Value.ToString());
                builder.Append(",");
                AppendJsonProperty(builder, "Domain", cookie.Domain.ToString());
                builder.Append(",");
                AppendJsonProperty(builder, "Path", cookie.Path.ToString());
                builder.Append(",");
                AppendJsonProperty(builder, "Expires", cookie.Expires.ToUniversalTime().ToString("u"));
                builder.Append(",");
                AppendJsonProperty(builder, "Secure", cookie.Secure.ToString());
                builder.Append(",");
                AppendJsonProperty(builder, "HttpOnly", cookie.HttpOnly.ToString());

                builder.Append("}");

                firstItem = false;
            }

            if (!firstItem)
            {
                builder.Append("]");
            }
        }

        private void SerializeAllPropertiesFlat(IEnumerable<HttpCookie> verboseCookieValues, StringBuilder builder, LogEventInfo logEvent)
        {
            var objectSeparator = GetRenderedObjectSeparator(logEvent);
            var itemSeparator = GetRenderedItemSeparator(logEvent);
            var valueSeparator = GetRenderedValueSeparator(logEvent);

            var firstItem = true;
            foreach (var cookie in verboseCookieValues)
            {
                if (!firstItem)
                {
                    builder.Append(objectSeparator);
                }
                firstItem = false;
                builder.Append($"Name{valueSeparator}{cookie.Name}{itemSeparator}");
                builder.Append($"Value{valueSeparator}{cookie.Value}{itemSeparator}");
                builder.Append($"Domain{valueSeparator}{cookie.Domain}{itemSeparator}");
                builder.Append($"Path{valueSeparator}{cookie.Path}{itemSeparator}");
                builder.Append($"Expires{valueSeparator}{cookie.Expires.ToUniversalTime():u}{itemSeparator}");
                builder.Append($"Secure{valueSeparator}{cookie.Secure}{itemSeparator}");
                builder.Append($"HttpOnly{valueSeparator}{cookie.HttpOnly}");
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

        private IEnumerable<SetCookieHeaderValue> GetVerboseCookieValues(IList<SetCookieHeaderValue> cookies)
        {
            if (CookieNames?.Count > 0)
            {
                return GetCookieVerboseValues(cookies, CookieNames);
            }
            else
            {
                return GetCookieVerboseAllValues(cookies, Exclude);
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

        private static IEnumerable<SetCookieHeaderValue> GetCookieVerboseValues(IList<SetCookieHeaderValue> cookies, List<string> cookieNames)
        {
            foreach (var needle in cookieNames)
            {
                for (int i = 0; i < cookies.Count; ++i)
                {
                    var cookie = cookies[i];
                    var cookieName = cookie.Name.ToString();
                    if (string.Equals(needle, cookieName, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return cookie;
                    }
                }
            }
        }

        private static IEnumerable<SetCookieHeaderValue> GetCookieVerboseAllValues(IList<SetCookieHeaderValue> cookies, ICollection<string> excludeNames)
        {
            bool checkForExclude = excludeNames?.Count > 0;
            for (int i = 0; i < cookies.Count; ++i)
            {
                var cookie = cookies[i];
                var cookieName = cookie.Name.ToString();
                if (checkForExclude && excludeNames.Contains(cookieName))
                    continue;

                yield return cookie;
            }
        }

        private void SerializeAllProperties(IEnumerable<SetCookieHeaderValue> verboseCookieValues, StringBuilder builder, LogEventInfo logEvent)
        {
            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializeAllPropertiesFlat(verboseCookieValues, builder, logEvent);
                    break;
                case AspNetRequestLayoutOutputFormat.JsonArray:
                    SerializeAllPropertiesJson(verboseCookieValues, builder);
                    break;
            }
        }

        private static void SerializeAllPropertiesJson(IEnumerable<SetCookieHeaderValue> verboseCookieValues, StringBuilder builder)
        {
            var firstItem = true;

            foreach (var cookie in verboseCookieValues)
            {
                if (firstItem)
                {
                    builder.Append('[');
                }
                else
                {
                    builder.Append(',');
                }

                builder.Append('{');

                AppendJsonProperty(builder, "Name", cookie.Name.ToString());
                builder.Append(',');
                AppendJsonProperty(builder, "Value", cookie.Value.ToString());
                builder.Append(',');
                AppendJsonProperty(builder, "Domain", cookie.Domain.ToString());
                builder.Append(',');
                AppendJsonProperty(builder, "Path", cookie.Path.ToString());
                builder.Append(',');
                AppendJsonProperty(builder, "Expires", cookie.Expires?.ToUniversalTime().ToString("u"));
                builder.Append(',');
                AppendJsonProperty(builder, "Secure", cookie.Secure.ToString());
                builder.Append(',');
                AppendJsonProperty(builder, "HttpOnly", cookie.HttpOnly.ToString());
                builder.Append(',');
                AppendJsonProperty(builder, "SameSite", cookie.SameSite.ToString());

                builder.Append('}');

                firstItem = false;
            }

            if (!firstItem)
            {
                builder.Append(']');
            }
        }

        private void SerializeAllPropertiesFlat(IEnumerable<SetCookieHeaderValue> verboseCookieValues, StringBuilder builder, LogEventInfo logEvent)
        {
            var objectSeparator = GetRenderedObjectSeparator(logEvent);
            var itemSeparator = GetRenderedItemSeparator(logEvent);
            var valueSeparator = GetRenderedValueSeparator(logEvent);

            var firstItem = true;
            foreach (var cookie in verboseCookieValues)
            {
                if (!firstItem)
                {
                    builder.Append(objectSeparator);
                }
                firstItem = false;
                builder.Append($"Name{valueSeparator}{cookie.Name}{itemSeparator}");
                builder.Append($"Value{valueSeparator}{cookie.Value}{itemSeparator}");
                builder.Append($"Domain{valueSeparator}{cookie.Domain}{itemSeparator}");
                builder.Append($"Path{valueSeparator}{cookie.Path}{itemSeparator}");
                builder.Append($"Expires{valueSeparator}{cookie.Expires?.ToUniversalTime():u}{itemSeparator}");
                builder.Append($"Secure{valueSeparator}{cookie.Secure}{itemSeparator}");
                builder.Append($"HttpOnly{valueSeparator}{cookie.HttpOnly}{itemSeparator}");
                builder.Append($"SameSite{valueSeparator}{cookie.SameSite}");
            }
        }
#endif
    }
}