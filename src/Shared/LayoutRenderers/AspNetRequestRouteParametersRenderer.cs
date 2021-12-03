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
    /// <example>
    /// <para>Example usage of ${aspnet-request-routeparameters}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-routeparameters:OutputFormat=Flat}
    /// ${aspnet-request-routeparameters:OutputFormat=JsonArray}
    /// ${aspnet-request-routeparameters:OutputFormat=JsonDictionary}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-routeparameters")]
    public class AspNetRequestRouteParametersRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// List Route Parameter' Key to be rendered from Request.
        /// If empty, then render all parameters
        /// </summary>
        public List<string> RouteParameterKeys { get; set; }

        /// <summary>
        /// Renders the specified ASP.NET Route Parameters and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
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

            bool printAllRouteParameter = RouteParameterKeys == null || RouteParameterKeys.Count == 0;
            List<string> routeParameterKeys = RouteParameterKeys;
            if (routeParameters == null || routeParameters.Count == 0)
            {
                return;
            }

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

                string value = objValue.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }
    }
}