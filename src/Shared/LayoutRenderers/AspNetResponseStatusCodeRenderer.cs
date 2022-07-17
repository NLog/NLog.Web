using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Response Status Code.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-response-statuscode}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetResponse-StatusCode-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-statuscode")]
    public class AspNetResponseStatusCodeRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor.HttpContext.TryGetResponse();
            if (httpResponse == null)
            {
                return;
            }

            int statusCode = httpResponse.StatusCode;

            if (statusCode >= 100 && statusCode <= 599)
            {
                builder.Append(statusCode);
            }
        }
    }
}
