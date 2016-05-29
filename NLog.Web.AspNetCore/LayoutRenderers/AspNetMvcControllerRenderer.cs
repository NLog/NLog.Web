using NLog.LayoutRenderers;
using System.Text;
#if !NETSTANDARD_1plus
using NLog.Web.Internal;
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Routing;
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
    public class AspNetMvcControllerRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
                return;

            string key = "controller";
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