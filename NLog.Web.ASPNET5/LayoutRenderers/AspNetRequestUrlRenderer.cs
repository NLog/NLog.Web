using System.Text;
#if !DNX
using System.Web;
using System.Collections.Specialized;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Primitives;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using System;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    [LayoutRenderer("aspnet-request-url")]
    public class AspNetRequestUrlRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// To specify whether to exclude / include the Query string.
        /// </summary>
        public bool IncludeQueryString { get; set; } = false;

#if !DNX
        /// <summary>
        /// To specify whether to exclude / include the Port. Only support non ASP.NET CORE, we can support this after RC2 Release.
        /// </summary>
        public bool IncludePort { get; set; } = false;
#endif
        /// <summary>
        /// Renders the Referrer URL from the HttpRequest
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.Request;

            if (httpRequest == null)
            {
                return;
            }
#if !DNX
            if (httpRequest.Url != null)
            {
                string port = String.Empty;
                string pathAndQuery = String.Empty;

                if (IncludePort && httpRequest.Url?.Port > 0)
                {
                    port = ":" + httpRequest.Url.Port.ToString();
                }

                if (IncludeQueryString && httpRequest.Url?.PathAndQuery != null)
                {
                    pathAndQuery = httpRequest.Url.PathAndQuery;
                }
                else
                {
                    pathAndQuery = httpRequest.Url.AbsolutePath;
                }

                var url = string.Concat($"{httpRequest.Url.Scheme}://{httpRequest.Url.Host}{port}{pathAndQuery}");
                builder.Append(url);
            }

#else
            var url = string.Concat(httpRequest.Scheme, "://", httpRequest.Host.ToUriComponent(), httpRequest.PathBase.ToUriComponent(), httpRequest.Path.ToUriComponent());

            if (IncludeQueryString)
                url = url + httpRequest.QueryString.ToUriComponent();

            builder.Append(url);
#endif

        }
    }
}
