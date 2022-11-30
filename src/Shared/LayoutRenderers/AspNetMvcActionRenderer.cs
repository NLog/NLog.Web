using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Routing;
using System.Web;
#else
using Microsoft.AspNetCore.Routing;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
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
    public class AspNetMvcActionRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var key = "action";

            var context = HttpContextAccessor.HttpContext;

#if !ASP_NET_CORE
            object actionValue = null;
            RouteTable.Routes?.GetRouteData(context)?.Values?.TryGetValue(key, out actionValue);
#else
            var actionValue = context?.GetRouteValue(key);
#endif
            builder.Append(actionValue?.ToString());
        }
    }
}