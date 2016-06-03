using NLog.LayoutRenderers;
using System.Text;
using System;
#if !NETSTANDARD_1plus
using NLog.Web.Internal;
using System.Web.Routing;
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif


namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base Class for ASP.NET MVC Renderer.
    /// </summary> 
    public abstract class AspNetMvcLayoutRendererBase : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>s        
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
                return;

            MvcDoAppend(builder, logEvent, context);
        }
        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        /// <param name="context">The current http context.</param>        
#if !NETSTANDARD_1plus
        protected abstract void MvcDoAppend(StringBuilder builder, LogEventInfo logEvent, HttpContextBase context);
#else
        protected abstract void MvcDoAppend(StringBuilder builder, LogEventInfo logEvent, HttpContext context);
#endif
    }
}