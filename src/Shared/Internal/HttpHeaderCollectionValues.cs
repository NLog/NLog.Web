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
        internal static IEnumerable<KeyValuePair<string, string?>> GetHeaderValues(IHeaderDictionary headers, List<string>? itemNames, ISet<string> excludeNames)
        {
            var checkForExclude = (excludeNames?.Count > 0 && (itemNames is null || itemNames.Count == 0)) ? excludeNames : null;
            var headerNames = itemNames?.Count > 0 ? itemNames : headers.Keys;
            foreach (var headerName in headerNames)
            {
                if (checkForExclude?.Contains(headerName) == true)
                    continue;

                if (!headers.TryGetValue(headerName, out var headerValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string?>(headerName, headerValue);
            }
        }
#else
        internal static IEnumerable<KeyValuePair<string, string?>> GetHeaderValues(NameValueCollection headers, List<string>? itemNames, HashSet<string> excludeNames)
        {
            var checkForExclude = (excludeNames?.Count > 0 && (itemNames is null || itemNames.Count == 0)) ? excludeNames : null;

            var headerNames = itemNames?.Count > 0 ? itemNames : headers.Keys.Cast<string>();
            foreach (var headerName in headerNames)
            {
                if (checkForExclude?.Contains(headerName) == true)
                    continue;

                var headerValue = headers[headerName];
                if (headerValue is null)
                    continue;

                yield return new KeyValuePair<string, string?>(headerName, headerValue);
            }
        }
#endif
    }
}