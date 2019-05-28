using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;

#else
using System.Collections.Specialized;
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request URL
    /// </summary>
    /// <para>Example usage of ${aspnet-request-url}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-url:IncludeQueryString=true} - produces http://www.exmaple.com/?t=1
    /// ${aspnet-request-url:IncludeQueryString=false} - produces http://www.exmaple.com/
    /// ${aspnet-request-url:IncludePort=true} - produces http://www.exmaple.com:80/
    /// ${aspnet-request-url:IncludePort=false} - produces http://www.exmaple.com/
    /// ${aspnet-request-url:IncludeScheme=false} - produces www.exmaple.com/
    /// ${aspnet-request-url:IncludePort=true:IncludeQueryString=true} - produces http://www.exmaple.com:80/?t=1    
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-url")]
    [ThreadSafe]
    public class AspNetRequestUrlRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// To specify whether to include / exclude the Query string. Default is false.
        /// </summary>
        public bool IncludeQueryString { get; set; } = false;

        /// <summary>
        /// To specify whether to  include /exclude the Port. Default is false.
        /// </summary>
        public bool IncludePort { get; set; } = false;

        /// <summary>
        /// To specify whether to exclude / include the host. Default is true.
        /// </summary>
        public bool IncludeHost { get; set; } = true;

        /// <summary>
        /// To specify whether to exclude / include the scheme. Default is true.
        /// </summary>
        public bool IncludeScheme { get; set; } = true;

        /// <summary>
        /// Renders the Request URL from the HttpRequest
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

            RenderUrl(httpRequest, builder);
        }

#if !ASP_NET_CORE
        private void RenderUrl(HttpRequestBase httpRequest, StringBuilder builder)
        {
            var url = httpRequest.Url;
            if (url == null)
                return;

            if (IncludeScheme && !string.IsNullOrEmpty(url.Scheme))
            {
                builder.Append(url.Scheme);
                builder.Append("://");
            }
            if (IncludeHost)
            {
                builder.Append(url.Host);
            }
            if (IncludePort && url.Port > 0)
            {
                builder.Append(':');
                builder.Append(url.Port);
            }

            var pathAndQuery = IncludeQueryString ? url.PathAndQuery : url.AbsolutePath;
            builder.Append(pathAndQuery);
        }
#else
        private void RenderUrl(HttpRequest httpRequest, StringBuilder builder)
        {
            if (IncludeScheme && !string.IsNullOrWhiteSpace(httpRequest.Scheme))
            {
                builder.Append(httpRequest.Scheme);
                builder.Append("://");
            }

            if (IncludeHost)
            {
                builder.Append(httpRequest.Host.Host);
            }

            if (IncludePort && httpRequest.Host.Port > 0)
            {
                builder.Append(':');
                builder.Append(httpRequest.Host.Port.Value);
            }

            builder.Append(httpRequest.PathBase.ToUriComponent());
            builder.Append(httpRequest.Path.ToUriComponent());
            if (IncludeQueryString)
            {
                builder.Append(httpRequest.QueryString.Value);
            }
        }
#endif
    }
}