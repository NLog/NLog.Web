using System.Text;
#if !DNX
using System.Web;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
#endif
using NLog.LayoutRenderers;

#if !DNX
namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Session ID.
    /// </summary>
    [LayoutRenderer("aspnet-sessionid")]
    public class AspNetSessionIDLayoutRenderer : AspNetLayoutRendererBase
    {
#if DNX
        /// <summary>
        /// Initializes the <see cref="AspNetSessionIDLayoutRenderer"/> with the <see cref="IHttpContextAccessor"/>.
        /// </summary>
        public AspNetSessionIDLayoutRenderer(IHttpContextAccessor accessor) : base(accessor)
        {
        }
#endif
        /// <summary>
        /// Renders the ASP.NET Session ID appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;

            if (context.Session == null)
            {
                return;
            }

            builder.Append(context.Session.SessionID);
        }
    }
}
#endif