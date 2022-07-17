using System.Text;
using NLog.LayoutRenderers;
using System.Collections.Generic;
using System.Linq;
#if !ASP_NET_CORE
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

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
    [LayoutRenderer("aspnet-request-routeparameters")]
    public class AspNetRequestRouteParametersRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Route Parameter' Key to be rendered from Request.
        /// If empty, then render all parameters
        /// </summary>
        public List<string> RouteParameterKeys { get; set; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
            {
                return;
            }

#if !ASP_NET_CORE
            RouteValueDictionary routeParameters = RouteTable.Routes?.GetRouteData(context)?.Values;
#else
            RouteValueDictionary routeParameters = context.GetRouteData()?.Values;
#endif
            if (routeParameters == null || routeParameters.Count == 0)
            {
                return;
            }

            var routeParameterKeys = RouteParameterKeys;
            bool printAllRouteParameter = routeParameterKeys == null || routeParameterKeys.Count == 0;
            if (printAllRouteParameter)
            {
                routeParameterKeys = routeParameters.Keys.ToList();
            }

            IEnumerable<KeyValuePair<string, string>> pairs = GetPairs(routeParameters, routeParameterKeys);
            SerializePairs(pairs, builder, logEvent);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetPairs(RouteValueDictionary routeParameters, List<string> routeParameterKeys)
        {
            foreach (string key in routeParameterKeys)
            {
                // This platform specific code is to prevent an unncessary .ToString call otherwise. 
                if (!routeParameters.TryGetValue(key, out object objValue))
                {
                    continue;
                }

                string value = objValue?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }
    }
}