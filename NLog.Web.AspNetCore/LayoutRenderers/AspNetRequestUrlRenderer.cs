﻿using System;
using System.Text;
#if !ASP_NET_CORE
using System.Collections.Specialized;
using System.Web;
#else
#endif
using NLog.LayoutRenderers;
using NLog.Web.Internal;

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
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
                return;

            var url = CreateUrl(httpRequest);
            builder.Append(url);
        }

#if !ASP_NET_CORE

        private string CreateUrl(HttpRequestBase httpRequest)
        {
            string port = null, host = null, scheme = null;

            if (httpRequest.Url == null)
            {
                return null;
            }

            if (IncludePort && httpRequest.Url.Port > 0)
            {
                port = ":" + httpRequest.Url.Port;
            }

            var pathAndQuery = IncludeQueryString ? httpRequest.Url.PathAndQuery : httpRequest.Url.AbsolutePath;

            if (IncludeHost)
            {
                host = httpRequest.Url?.Host;
            }

            if (IncludeScheme)
            {
                scheme = httpRequest.Url.Scheme + "://";
            }

            var url = $"{scheme}{host}{port}{pathAndQuery}";
            return url;
        }

#else
        private string CreateUrl(Microsoft.AspNetCore.Http.HttpRequest httpRequest)
        {
            string pathAndQuery = null, port = null, host = null, scheme = null;
        
            if (IncludeQueryString)
            {
                pathAndQuery = httpRequest.QueryString.Value;
            }

            if (IncludePort && httpRequest.Host.Port > 0)
            {
                port = ":" + httpRequest.Host.Port.ToString();
            }

            if (IncludeHost)
            {
                host = httpRequest.Host.Host;
            }

            if (IncludeScheme && !String.IsNullOrWhiteSpace(httpRequest.Scheme))
            {
                scheme = httpRequest.Scheme + "://";
            }

            var url = $"{scheme}{host}{port}{httpRequest.PathBase.ToUriComponent()}{httpRequest.Path.ToUriComponent()}{pathAndQuery}";
            return url;
        }

#endif
    }
}
