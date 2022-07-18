using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace NLog.Web.Internal
{
    internal static class HttpCookieCollectionValues
    {
        internal static IEnumerable<KeyValuePair<string, string>> GetCookieValues(HttpCookieCollection cookies, List<string> cookieNames, HashSet<string> excludeNames, bool expandMultiValue)
        {
            if (cookieNames?.Count > 0)
            {
                return GetCookieNameValues(cookies, cookieNames, expandMultiValue);
            }
            else
            {
                return GetCookieAllValues(cookies, excludeNames, expandMultiValue);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetCookieNameValues(HttpCookieCollection cookies, List<string> cookieNames, bool expandMultiValue)
        {
            foreach (var cookieName in cookieNames)
            {
                var httpCookie = cookies[cookieName];
                if (httpCookie == null)
                    continue;

                if (expandMultiValue)
                {
                    var values = httpCookie.Values;
                    if (values?.Count > 1)
                    {
                        foreach (var cookieValue in GetCookieMultiValues(httpCookie.Name, values))
                            yield return new KeyValuePair<string, string>(cookieValue.Key, cookieValue.Value);
                        continue;
                    }
                }

                yield return new KeyValuePair<string, string>(httpCookie.Name, httpCookie.Value);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetCookieAllValues(HttpCookieCollection cookies, HashSet<string> excludeNames, bool expandMultiValue)
        {
            bool checkForExclude = excludeNames?.Count > 0;

            foreach (string cookieName in cookies.Keys)
            {
                if (checkForExclude && excludeNames.Contains(cookieName))
                    continue;

                var httpCookie = cookies[cookieName];
                if (httpCookie == null)
                    continue;

                if (expandMultiValue)
                {
                    var values = httpCookie.Values;
                    if (values?.Count > 1)
                    {
                        foreach (var cookieValue in GetCookieMultiValues(httpCookie.Name, values))
                            yield return new KeyValuePair<string, string>(cookieValue.Key, cookieValue.Value);
                        continue;
                    }
                }

                yield return new KeyValuePair<string, string>(httpCookie.Name, httpCookie.Value);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetCookieMultiValues(string firstKey, NameValueCollection httpCookiValues)
        {
            // Split multi-valued cookie, as allowed for in the HttpCookie API for backwards compatibility with classic ASP
            var isFirst = true;
            foreach (var multiValueKey in httpCookiValues.AllKeys)
            {
                var cookieKey = multiValueKey;
                if (isFirst)
                {
                    cookieKey = firstKey;
                    isFirst = false;
                }
                yield return new KeyValuePair<string, string>(cookieKey, httpCookiValues[multiValueKey]);
            }
        }

        private static IEnumerable<HttpCookie> GetCookieVerboseMultiValues(HttpCookie cookie)
        {
            var cookieValues = cookie.Values;
            // Split multi-valued cookie, as allowed for in the HttpCookie API for backwards compatibility with classic ASP
            var isFirst = true;
            foreach (var multiValueKey in cookieValues.AllKeys)
            {
                var cookieKey = multiValueKey;
                if (isFirst)
                {
                    cookieKey = cookie.Name;
                    isFirst = false;
                }

                yield return new HttpCookie(cookieKey, cookieValues[multiValueKey])
                {
                    Domain = cookie.Domain,
                    Path = cookie.Path,
                    Expires = cookie.Expires,
                    Secure = cookie.Secure,
                    HttpOnly = cookie.HttpOnly
                };
            }
        }

        internal static IEnumerable<HttpCookie> GetVerboseCookieValues(HttpCookieCollection cookies, List<string> cookieNames, HashSet<string> excludeNames, bool expandMultiValue)
        {
            if (cookieNames?.Count > 0)
            {
                return GetCookieVerboseNameValues(cookies, cookieNames, expandMultiValue);
            }
            else
            {
                return GetCookieVerboseAllValues(cookies, excludeNames, expandMultiValue);
            }
        }

        private static IEnumerable<HttpCookie> GetCookieVerboseNameValues(HttpCookieCollection cookies, List<string> cookieNames, bool expandMultiValue)
        {
            foreach (var cookieName in cookieNames)
            {
                var httpCookie = cookies[cookieName];
                if (httpCookie == null)
                    continue;

                if (expandMultiValue && httpCookie.Values.Count > 1)
                {
                    foreach (var cookie in GetCookieVerboseMultiValues(httpCookie))
                        yield return cookie;
                }
                else
                {
                    yield return httpCookie;
                }
            }
        }

        private static IEnumerable<HttpCookie> GetCookieVerboseAllValues(HttpCookieCollection cookies, HashSet<string> excludeNames, bool expandMultiValue)
        {
            bool checkForExclude = excludeNames?.Count > 0;
            foreach (string cookieName in cookies.Keys)
            {
                if (checkForExclude && excludeNames.Contains(cookieName))
                    continue;

                var httpCookie = cookies[cookieName];
                if (httpCookie == null)
                    continue;

                if (expandMultiValue && httpCookie.Values.Count > 1)
                {
                    foreach (var cookie in GetCookieVerboseMultiValues(httpCookie))
                        yield return cookie;
                }
                else
                {
                    yield return httpCookie;
                }
            }
        }
    }
}
