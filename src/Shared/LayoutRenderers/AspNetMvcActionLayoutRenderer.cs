using System.Text;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET RouteData MVC Action Name.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-mvc-action}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-MVC-Action-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-mvc-action")]
    public class AspNetMvcActionLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var key = "action";

            var httpContext = HttpContextAccessor?.HttpContext;
            if (httpContext is null)
                return;

#if !ASP_NET_CORE
            object? actionValue = null;
            RouteTable.Routes?.GetRouteData(httpContext)?.Values?.TryGetValue(key, out actionValue);
#else
            var actionValue = httpContext.GetRouteValue(key);
#endif
            builder.Append(actionValue?.ToString());
        }
    }
}