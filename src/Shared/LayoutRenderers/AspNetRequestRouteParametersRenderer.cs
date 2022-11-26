using System.Text;
using System.Collections.Generic;
#if !ASP_NET_CORE
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif
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
        public List<string> RouteParameterKeys { get => Items; set => Items = value; }

        /// <summary>
        /// List Route Parameter' Key to be rendered from Request.
        /// If empty, then render all parameters
        /// </summary>
        public List<string> Items { get; set; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
            {
                return;
            }

            var routeParameterKeys = Items;

            if (routeParameterKeys?.Count == 1 && !string.IsNullOrEmpty(routeParameterKeys[0]))
            {
#if !ASP_NET_CORE
                object routeValue = null;
                RouteTable.Routes?.GetRouteData(context)?.Values?.TryGetValue(routeParameterKeys[0], out routeValue);
#else
                var routeValue = context?.GetRouteValue(routeParameterKeys[0]);
#endif
                var routeStringValue = routeValue?.ToString();
                if (!string.IsNullOrEmpty(routeStringValue))
                {

                    var pair = new[] { new KeyValuePair<string, string>(routeParameterKeys[0], routeStringValue) };
                    SerializePairs(pair, builder, logEvent);
                }
            }
            else
            {
#if !ASP_NET_CORE
                RouteValueDictionary routeParameters = RouteTable.Routes?.GetRouteData(context)?.Values;
#else
                RouteValueDictionary routeParameters = context.GetRouteData()?.Values;
#endif
                if (routeParameters?.Count > 0)
                {
                    var pairs = GetPairs(routeParameters, routeParameterKeys);
                    SerializePairs(pairs, builder, logEvent);
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetPairs(RouteValueDictionary routeParameters, List<string> routeParameterKeys)
        {
            if (routeParameterKeys?.Count > 0)
            {
                foreach (var routeKey in routeParameterKeys)
                {
                    if (routeParameters.TryGetValue(routeKey, out var routeValue))
                    {
                        string value = routeValue?.ToString();
                        if (!string.IsNullOrEmpty(value))
                            yield return new KeyValuePair<string, string>(routeKey, value);
                    }
                }
            }
            else
            {
                // Output all route-values
                foreach (var routeItem in routeParameters)
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
}