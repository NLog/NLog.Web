using System.Text;
using System.Web;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Session ID.
    /// </summary>
    [LayoutRenderer("aspnet-sessionid")]
    public class AspNetSessionIDLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Renders the ASP.NET Session ID appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            if (context.Session == null)
            {
                return;
            }

            builder.Append(context.Session.SessionID);
        }
    }
}