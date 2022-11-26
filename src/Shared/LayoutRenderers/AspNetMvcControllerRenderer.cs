using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Routing;
using System.Web;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
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
    public class AspNetMvcControllerRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var key = "controller";

            var context = HttpContextAccessor.HttpContext;


#if !ASP_NET_CORE
            object controllerValue = null;
            RouteTable.Routes?.GetRouteData(context)?.Values?.TryGetValue(key, out controllerValue);
#else
            var controllerValue = context?.GetRouteValue(key);
#endif
            builder.Append(controllerValue?.ToString());
        }
    }
}