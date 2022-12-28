using System;
using System.Collections.Generic;
using System.Linq;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Collections.Specialized;
#endif

namespace NLog.Web.Internal
{
    internal static class HttpHeaderCollectionValues
    {
#if ASP_NET_CORE
        internal static IEnumerable<KeyValuePair<string, string>> GetHeaderValues(IHeaderDictionary headers, List<string> itemNames, ISet<string> excludeNames)
        {
            bool checkForExclude = (itemNames == null || itemNames.Count == 0) && excludeNames?.Count > 0;
            var headerNames = itemNames?.Count > 0 ? itemNames : headers.Keys;
            foreach (var headerName in headerNames)
            {
                if (checkForExclude && excludeNames.Contains(headerName))
                    continue;

                if (!headers.TryGetValue(headerName, out var headerValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(headerName, headerValue);
            }
        }
#else
        internal static IEnumerable<KeyValuePair<string, string>> GetHeaderValues(NameValueCollection headers, List<string> itemNames, HashSet<string> excludeNames)
        {
            bool checkForExclude = (itemNames == null || itemNames.Count == 0) && excludeNames?.Count > 0;
            var headerNames = itemNames?.Count > 0 ? itemNames : headers.Keys.Cast<string>();
            foreach (var headerName in headerNames)
            {
                if (checkForExclude && excludeNames.Contains(headerName))
                    continue;

                var headerValue = headers[headerName];
                if (headerValue == null)
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(headerName, headerValue);
            }
        }
#endif
    }
}