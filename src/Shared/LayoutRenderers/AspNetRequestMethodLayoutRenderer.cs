using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

#if !ASP_NET_CORE
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Http Request Method (POST / GET)
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-method}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-Method-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-method")]
    public class AspNetRequestMethodLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext.TryGetRequest();

#if !ASP_NET_CORE
            var httpMethod = httpRequest?.HttpMethod;
#else
            var httpMethod = httpRequest?.Method;
#endif
            builder.Append(httpMethod);
        }
    }
}