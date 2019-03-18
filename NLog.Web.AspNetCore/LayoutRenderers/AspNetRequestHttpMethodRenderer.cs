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
    /// ASP.NET Http Request Method.
    /// </summary>
    /// <para>Example usage of ${aspnet-request-method}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-method} - Produces - Post.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-method")]
    [ThreadSafe]
    public class AspNetRequestHttpMethodRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// ASP.NET Http Request Method
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            string httpMethod;
#if !ASP_NET_CORE
            httpMethod = httpRequest.HttpMethod;
#else
            httpMethod = httpRequest.Method;
#endif
            builder.Append(httpMethod);
        }
    }
}