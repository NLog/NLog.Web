using System;
using System.Text;
#if !ASP_NET_CORE
using System.Web;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base Class for ASP.NET MVC Renderer.
    /// </summary>
    [Obsolete("Inherit from AspNetLayoutRendererBase instead. Marked obsolete with NLog.Web v5.1")]
    public abstract class AspNetMvcLayoutRendererBase : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null)
            {
                return;
            }

            MvcDoAppend(builder, logEvent, context);
        }

        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        /// <param name="context">The current http context.</param>
        protected abstract void MvcDoAppend(StringBuilder builder, LogEventInfo logEvent, HttpContextBase context);
    }
}