using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET request connection id
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-connection-id}
    /// </remarks>
    [LayoutRenderer("aspnet-request-connection-id")]
    public class AspNetRequestConnectionIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the ASP.NET connection ID
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var connection = HttpContextAccessor.HttpContext.TryGetConnection();
            if (connection == null)
            {
                return;
            }

            var id = connection.Id;
            if(!string.IsNullOrEmpty(id))
            {
                builder.Append(id);
            }
        }
    }
}
