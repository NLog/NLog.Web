using System.Text;
using System.Web;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User variable.
    /// </summary>
    [LayoutRenderer("aspnet-user-identity")]
    public class AspNetUserIdentityLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Renders the specified ASP.NET User.Identity.Name variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            if (context.User == null)
            {
                return;
            }

            if (context.User.Identity == null)
            {
                return;
            }

            builder.Append(context.User.Identity.Name);
        }
    }
}