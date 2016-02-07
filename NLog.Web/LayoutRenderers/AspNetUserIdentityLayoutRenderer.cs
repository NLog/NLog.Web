using System.Text;
#if NET451
using System.Web;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
#endif
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User variable.
    /// </summary>
    [LayoutRenderer("aspnet-user-identity")]
    public class AspNetUserIdentityLayoutRenderer : AspNetLayoutRendererBase
    {
#if DOTNET5_4
        /// <summary>
        /// Initializes the <see cref="AspNetUserIdentityLayoutRenderer"/> with the <see cref="IHttpContextAccessor"/>.
        /// </summary>
        public AspNetUserIdentityLayoutRenderer(IHttpContextAccessor accessor) : base(accessor)
        {
        }
#endif
        /// <summary>
        /// Renders the specified ASP.NET User.Identity.Name variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            
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