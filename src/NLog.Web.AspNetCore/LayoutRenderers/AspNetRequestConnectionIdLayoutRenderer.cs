using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET request connection id
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-connection-id}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-connection-id")]
    public class AspNetRequestConnectionIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
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
