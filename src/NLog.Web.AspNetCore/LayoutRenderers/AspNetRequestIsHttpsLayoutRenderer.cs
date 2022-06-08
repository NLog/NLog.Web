using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Is Request HTTPS
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-is-https}
    /// </remarks>
    [LayoutRenderer("aspnet-request-is-https")]
    public class AspNetRequestIsHttpsLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET Core HttpContext.Request.IsHttps variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = HttpContextAccessor.HttpContext?.TryGetRequest();
            builder.Append(request?.IsHttps == true ? '1' : '0');
        }
    }
}
