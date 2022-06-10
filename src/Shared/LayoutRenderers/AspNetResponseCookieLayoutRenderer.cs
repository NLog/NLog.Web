using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;
using NLog.Layouts;
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
        /// Separator between objects, like cookies. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat" />
        /// </summary>
        /// <remarks>Render with <see cref="GetRenderedObjectSeparator" /></remarks>
        public string ObjectSeparator { get => _objectSeparatorLayout?.OriginalText; set => _objectSeparatorLayout = new SimpleLayout(value ?? ""); }
        private SimpleLayout _objectSeparatorLayout = new SimpleLayout(";");

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

        /// <summary>
        /// Get the rendered <see cref="ObjectSeparator" />
        /// </summary>
        private string GetRenderedObjectSeparator(LogEventInfo logEvent)
        {
            return logEvent != null ? _objectSeparatorLayout.Render(logEvent) : ObjectSeparator;
        }

        /// <summary>
        /// Append the quoted name and value separated by a colon
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="skipPropertySeparator"></param>
        private static void AppendJsonProperty(StringBuilder builder, string name, string value, bool skipPropertySeparator = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                AppendQuoted(builder, name);
                builder.Append(':');
                AppendQuoted(builder, value);
                if (!skipPropertySeparator)
                {
                    builder.Append(',');
                }
            }
        }

        /// <summary>
        /// Append the quoted name and value separated by a value separator
        /// and ended by item separator
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="logEvent"></param>
        /// <param name="skipItemSeparator"></param>
        private void AppendFlatProperty(
            StringBuilder builder,
            string name,
            string value,
            LogEventInfo logEvent,
            bool skipItemSeparator = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                builder.Append(name);
                builder.Append(GetRenderedValueSeparator(logEvent));
                builder.Append(value);
                if (!skipItemSeparator)
                {
                    builder.Append(GetRenderedItemSeparator(logEvent));
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
                case AspNetRequestLayoutOutputFormat.JsonDictionary:
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
                    if (OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                    {
                        builder.Append('{');
                    }
                    else
                    {
                        builder.Append('[');
                    }
                }
                else
                {
                    builder.Append(',');
                }

                builder.Append('{');

                AppendJsonProperty(builder, nameof(cookie.Name), cookie.Name);
                AppendJsonProperty(builder, nameof(cookie.Value), cookie.Value);
                AppendJsonProperty(builder, nameof(cookie.Domain), cookie.Domain);
                AppendJsonProperty(builder, nameof(cookie.Path), cookie.Path);
                AppendJsonProperty(builder, nameof(cookie.Expires), cookie.Expires.ToUniversalTime().ToString("u"));
                AppendJsonProperty(builder, nameof(cookie.Secure), cookie.Secure.ToString());
                AppendJsonProperty(builder, nameof(cookie.HttpOnly), cookie.HttpOnly.ToString(),skipPropertySeparator: true);

                builder.Append('}');

                firstItem = false;
            }

            if (!firstItem)
            {
                if (OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                {
                    builder.Append('}');
                }
                else
                {
                    builder.Append(']');
                }
            }
        }

        private void SerializeAllPropertiesFlat(IEnumerable<HttpCookie> verboseCookieValues, StringBuilder builder, LogEventInfo logEvent)
        {
            var objectSeparator = GetRenderedObjectSeparator(logEvent);

            var firstItem = true;
            foreach (var cookie in verboseCookieValues)
            {
                if (!firstItem)
                {
                    builder.Append(objectSeparator);
                }

                firstItem = false;

                AppendFlatProperty(builder, nameof(cookie.Name),     cookie.Name,   logEvent);
                AppendFlatProperty(builder, nameof(cookie.Value),    cookie.Value,  logEvent);
                AppendFlatProperty(builder, nameof(cookie.Domain),   cookie.Domain, logEvent);
                AppendFlatProperty(builder, nameof(cookie.Path),     cookie.Path,   logEvent);
                AppendFlatProperty(builder, nameof(cookie.Expires),  cookie.Expires.ToUniversalTime().ToString("u"), logEvent);
                AppendFlatProperty(builder, nameof(cookie.Secure),   cookie.Secure.ToString(),   logEvent);
                AppendFlatProperty(builder, nameof(cookie.HttpOnly), cookie.HttpOnly.ToString(), logEvent, skipItemSeparator: true);
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
                case AspNetRequestLayoutOutputFormat.JsonDictionary:
                    SerializeAllPropertiesJson(verboseCookieValues, builder);
                    break;
            }
        }

        private void SerializeAllPropertiesJson(IEnumerable<SetCookieHeaderValue> verboseCookieValues, StringBuilder builder)
        {
            var firstItem = true;

            foreach (var cookie in verboseCookieValues)
            {
                if (firstItem)
                {
                    if (OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                    {
                        builder.Append('{');
                    }
                    else
                    {
                        builder.Append('[');
                    }
                }
                else
                {
                    builder.Append(',');
                }

                builder.Append('{');

                AppendJsonProperty(builder, nameof(cookie.Name),     cookie.Name.ToString());
                AppendJsonProperty(builder, nameof(cookie.Value),    cookie.Value.ToString());
                AppendJsonProperty(builder, nameof(cookie.Domain),   cookie.Domain.ToString());
                AppendJsonProperty(builder, nameof(cookie.Path),     cookie.Path.ToString());
                AppendJsonProperty(builder, nameof(cookie.Expires),  cookie.Expires?.ToUniversalTime().ToString("u"));
                AppendJsonProperty(builder, nameof(cookie.Secure),   cookie.Secure.ToString());
                AppendJsonProperty(builder, nameof(cookie.HttpOnly), cookie.HttpOnly.ToString());
                AppendJsonProperty(builder, nameof(cookie.SameSite), cookie.SameSite.ToString(), skipPropertySeparator: true);

                builder.Append('}');

                firstItem = false;
            }

            if (!firstItem)
            {
                if (OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                {
                    builder.Append('}');
                }
                else
                {
                    builder.Append(']');
                }
            }
        }

        private void SerializeAllPropertiesFlat(IEnumerable<SetCookieHeaderValue> verboseCookieValues, StringBuilder builder, LogEventInfo logEvent)
        {
            var objectSeparator = GetRenderedObjectSeparator(logEvent);

            var firstItem = true;
            foreach (var cookie in verboseCookieValues)
            {
                if (!firstItem)
                {
                    builder.Append(objectSeparator);
                }

                firstItem = false;

                AppendFlatProperty(builder, nameof(cookie.Name),     cookie.Name.ToString(),     logEvent);
                AppendFlatProperty(builder, nameof(cookie.Value),    cookie.Value.ToString(),    logEvent);
                AppendFlatProperty(builder, nameof(cookie.Domain),   cookie.Domain.ToString(),   logEvent);
                AppendFlatProperty(builder, nameof(cookie.Path),     cookie.Path.ToString(),     logEvent);
                AppendFlatProperty(builder, nameof(cookie.Expires),  cookie.Expires?.ToUniversalTime().ToString("u"), logEvent);
                AppendFlatProperty(builder, nameof(cookie.Secure),   cookie.Secure.ToString(),   logEvent);
                AppendFlatProperty(builder, nameof(cookie.HttpOnly), cookie.HttpOnly.ToString(), logEvent);
                AppendFlatProperty(builder, nameof(cookie.SameSite), cookie.SameSite.ToString(), logEvent, skipItemSeparator: true);
            }
        }
#endif
    }
}