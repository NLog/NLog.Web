using System;
using System.Text;
using NLog.Config;
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
    public class AspNetRequestHttpMethodRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();

            string httpMethod;
#if !ASP_NET_CORE
            httpMethod = httpRequest?.HttpMethod;
#else
            httpMethod = httpRequest?.Method;
#endif
            builder.Append(httpMethod);
        }
    }
}