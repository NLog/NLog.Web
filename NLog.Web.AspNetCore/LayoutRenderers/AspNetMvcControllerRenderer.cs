using NLog.LayoutRenderers;
using System.Text;
#if !ASP_NET_CORE
using System.Web.Routing;
using System.Web;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
#endif


namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET MVC Controller Name.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer to render the controller name.
    /// </remarks>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-mvc-controller}    
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-mvc-controller")]
    public class AspNetMvcControllerRenderer : AspNetMvcLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        /// <param name="context">The current http context.</param>
        protected override void MvcDoAppend(StringBuilder builder, LogEventInfo logEvent, HttpContextBase context)
        {
            string key = "controller";
            string controller;

#if !ASP_NET_CORE
            controller = RouteTable.Routes?.GetRouteData(context)?.Values[key]?.ToString();
#else
            controller = context?.GetRouteData()?.Values?[key]?.ToString();
#endif
            if (!string.IsNullOrEmpty(controller))
                builder.Append(controller);
        }
    }
}