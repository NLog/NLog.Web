using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Response Status Code.
    /// </summary>
    /// <para>Example usage of ${aspnet-response-statuscode}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-response-statuscode} - Produces - 200.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-response-statuscode")]
    [ThreadSafe]
    public class AspNetResponseStatusCodeRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// ASP.NET Http Response Status Code
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor.HttpContext.TryGetResponse();
            if (httpResponse == null)
            {
                return;
            }

            int statusCode;
#if !ASP_NET_CORE
            statusCode = httpResponse.StatusCode;
#else
            statusCode = httpResponse.StatusCode;
#endif
            if (statusCode >= 100 && statusCode <= 599)
            {
                builder.Append(statusCode);
            }
        }
    }
}