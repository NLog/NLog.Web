using NLog.LayoutRenderers;
using System.Text;
#if !NETSTANDARD_1plus
using System.Web.Routing;
using System.Web;
#else
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
    /// ${aspnet-mvc-action}    
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-mvc-action")]
    public class AspNetMvcActionRenderer : AspNetMvcLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        /// <param name="context">The current http context.</param>
#if !NETSTANDARD_1plus
        protected override void MvcDoAppend(StringBuilder builder, LogEventInfo logEvent, HttpContextBase context)
#else
        protected override void MvcDoAppend(StringBuilder builder, LogEventInfo logEvent, HttpContext context)
#endif
        {
            string key = "action";
            string controller;

#if !NETSTANDARD_1plus
            controller = RouteTable.Routes?.GetRouteData(context)?.Values[key]?.ToString();
#else
            controller = context.GetRouteValue(key)?.ToString();
#endif
            if (!string.IsNullOrEmpty(controller))
                builder.Append(controller);
        }
    }
}