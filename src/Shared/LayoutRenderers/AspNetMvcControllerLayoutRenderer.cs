using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET RouteData MVC Controller Name.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-mvc-controller}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-MVC-Controller-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-mvc-controller")]
    public class AspNetMvcControllerLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var key = "controller";

            var httpContext = HttpContextAccessor?.HttpContext;
            if (httpContext is null)
                return;

#if !ASP_NET_CORE
            object? controllerValue = null;
            RouteTable.Routes?.GetRouteData(httpContext)?.Values?.TryGetValue(key, out controllerValue);
#else
            var controllerValue = httpContext.GetRouteValue(key);
#endif
            builder.Append(controllerValue?.ToString());
        }
    }
}