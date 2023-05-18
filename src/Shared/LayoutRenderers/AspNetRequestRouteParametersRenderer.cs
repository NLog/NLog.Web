using System;
using System.Text;
using System.Collections.Generic;
#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Route Parameters
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-routeparameters:OutputFormat=Flat}
    /// ${aspnet-request-routeparameters:OutputFormat=JsonArray}
    /// ${aspnet-request-routeparameters:OutputFormat=JsonDictionary}
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-RouteParameters-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-routeparameters")]
    public class AspNetRequestRouteParametersRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Route Parameter' Key to be rendered from Request.
        /// If empty, then render all parameters
        /// </summary>
        [DefaultParameter]
        public List<string> Items { get; set; }

        /// <summary>
        /// List Route Parameter' Key to be rendered from Request.
        /// If empty, then render all parameters
        /// </summary>
        [Obsolete("Instead use Items-property. Marked obsolete with NLog.Web 5.3")]
        public List<string> RouteParameterKeys { get => Items; set => Items = value; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
            {
                return;
            }

            var pairs = GetPairs(context, Items);
            if (pairs != null)
            {
                SerializePairs(pairs, builder, logEvent);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetPairs(HttpContextBase httpContext, List<string> routeParameterKeys)
        {
            if (routeParameterKeys?.Count == 1 && !string.IsNullOrEmpty(routeParameterKeys[0]))
            {
#if !ASP_NET_CORE
                object routeValue = null;
                RouteTable.Routes?.GetRouteData(httpContext)?.Values?.TryGetValue(routeParameterKeys[0], out routeValue);
#else
                var routeValue = httpContext?.GetRouteValue(routeParameterKeys[0]);
#endif
                var routeStringValue = routeValue?.ToString();
                if (!string.IsNullOrEmpty(routeStringValue))
                    return new[] { new KeyValuePair<string, string>(routeParameterKeys[0], routeStringValue) };
            }
            else
            {
#if !ASP_NET_CORE
                RouteValueDictionary routeValues = RouteTable.Routes?.GetRouteData(httpContext)?.Values;
#else
                RouteValueDictionary routeValues = httpContext.GetRouteData()?.Values;
#endif
                if (routeValues?.Count > 0)
                    return routeParameterKeys?.Count > 0 ? GetManyPairs(routeValues, routeParameterKeys) : GetAllPairs(routeValues);
            }

            return null;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetManyPairs(RouteValueDictionary routeValues, List<string> routeParameterKeys)
        {
            foreach (var routeKey in routeParameterKeys)
            {
                if (routeValues.TryGetValue(routeKey, out var routeValue))
                {
                    string value = routeValue?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        yield return new KeyValuePair<string, string>(routeKey, value);
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetAllPairs(RouteValueDictionary routeValues)
        {
            foreach (var routeItem in routeValues)
            {
                string routeValue = routeItem.Value?.ToString();
                if (!string.IsNullOrEmpty(routeValue))
                {
                    yield return new KeyValuePair<string, string>(routeItem.Key, routeValue);
                }
            }
        }
    }
}