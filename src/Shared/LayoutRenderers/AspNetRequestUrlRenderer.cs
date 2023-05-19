using System;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using NLog.Common;
using NLog.Web.Enums;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
#else
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request URL
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-url:IncludeQueryString=true} - produces http://www.example.com/?t=1
    /// ${aspnet-request-url:IncludeQueryString=false} - produces http://www.example.com/
    /// ${aspnet-request-url:IncludePort=true} - produces http://www.example.com:80/
    /// ${aspnet-request-url:IncludePort=false} - produces http://www.example.com/
    /// ${aspnet-request-url:IncludeScheme=false} - produces www.example.com/
    /// ${aspnet-request-url:IncludePort=true:IncludeQueryString=true} - produces http://www.example.com:80/?t=1
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-Url-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-url")]
    public class AspNetRequestUrlRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// A flags enumeration that controls which of the five portions of the URL are logged.
        /// Defaults to scheme://host/path, port and query string are by default not logged.
        /// </summary>
        public AspNetRequestUrlProperty Properties { get; set; } = AspNetRequestUrlProperty.Default;

        /// <summary>
        /// To specify whether to include / exclude the Query string. Default is false.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public bool IncludeQueryString 
        {
            get => HasPropertiesFlag(AspNetRequestUrlProperty.Query);
            set => SetPropertiesFlag(AspNetRequestUrlProperty.Query, value);
        }

        /// <summary>
        /// To specify whether to include / exclude the Port. Default is false.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public bool IncludePort
        {
            get => HasPropertiesFlag(AspNetRequestUrlProperty.Port);
            set => SetPropertiesFlag(AspNetRequestUrlProperty.Port, value);
        }

        /// <summary>
        /// To specify whether to exclude / include the host. Default is true.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public bool IncludeHost
        {
            get => HasPropertiesFlag(AspNetRequestUrlProperty.Host);
            set => SetPropertiesFlag(AspNetRequestUrlProperty.Host, value);
        }

        /// <summary>
        /// To specify whether to exclude / include the scheme. Ex. 'http' or 'https'.  Default is true.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public bool IncludeScheme
        {
            get => HasPropertiesFlag(AspNetRequestUrlProperty.Scheme);
            set => SetPropertiesFlag(AspNetRequestUrlProperty.Scheme, value);
        }

        /// <summary>
        /// To specify whether to exclude / include the url-path. Default is true.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead. Marked obsolete on NLog.Web 5.1")]
        public bool IncludePath
        {
            get => HasPropertiesFlag(AspNetRequestUrlProperty.Path);
            set => SetPropertiesFlag(AspNetRequestUrlProperty.Path, value);
        }

        private void SetPropertiesFlag(AspNetRequestUrlProperty bit, bool flag)
        {
            if (flag)
            {
                Properties |= bit;
            }
            else
            {
                Properties &= ~bit;
            }
        }

        private bool HasPropertiesFlag(AspNetRequestUrlProperty bit)
        {
            return (Properties & bit) == bit;
        }

#if ASP_NET_CORE

        /// <summary>
        /// To specify whether to use raw path and full query. Default is false.
        /// See https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.features.ihttprequestfeature.rawtarget
        /// </summary>
        public bool UseRawTarget { get; set; }
        
#endif

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            try
            {
                RenderUrl(httpRequest, builder);
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-request-url - HttpContext has been disposed");
            }
        }

#if !ASP_NET_CORE

        private void RenderUrl(HttpRequestBase httpRequest, StringBuilder builder)
        {
            var url = httpRequest.Url;
            if (url == null)
            {
                return;
            }

            if (HasPropertiesFlag(AspNetRequestUrlProperty.Scheme) && 
                !string.IsNullOrEmpty(url.Scheme))
            {
                builder.Append(url.Scheme);
                builder.Append("://");
            }
            if (HasPropertiesFlag(AspNetRequestUrlProperty.Host))
            {
                builder.Append(url.Host);
            }
            if (HasPropertiesFlag(AspNetRequestUrlProperty.Port) && url.Port > 0)
            {
                builder.Append(':');
                builder.Append(url.Port);
            }

            if (HasPropertiesFlag(AspNetRequestUrlProperty.Path))
            {
                var pathAndQuery = HasPropertiesFlag(AspNetRequestUrlProperty.Query) ? 
                    url.PathAndQuery : url.AbsolutePath;
                builder.Append(pathAndQuery);
            }
            else if (HasPropertiesFlag(AspNetRequestUrlProperty.Query))
            {
                builder.Append(url.Query);
            }
        }
#else
        private void RenderUrl(HttpRequest httpRequest, StringBuilder builder)
        {
            if (HasPropertiesFlag(AspNetRequestUrlProperty.Scheme) && 
                !string.IsNullOrWhiteSpace(httpRequest.Scheme))
            {
                builder.Append(httpRequest.Scheme);
                builder.Append("://");
            }

            if (HasPropertiesFlag(AspNetRequestUrlProperty.Host))
            {
                builder.Append(httpRequest.Host.Host);
            }

            if (HasPropertiesFlag(AspNetRequestUrlProperty.Port) && 
                httpRequest.Host.Port > 0)
            {
                builder.Append(':');
                builder.Append(httpRequest.Host.Port.Value);
            }

            if (HasPropertiesFlag(AspNetRequestUrlProperty.Path))
            {
                if (UseRawTarget && httpRequest.HttpContext.TryGetFeature<IHttpRequestFeature>() is IHttpRequestFeature httpRequestFeature)
                {
                    builder.Append(httpRequestFeature.RawTarget);
                }
                else
                {
                    builder.Append((httpRequest.PathBase + httpRequest.Path).ToUriComponent());

                    if (HasPropertiesFlag(AspNetRequestUrlProperty.Query))
                    {
                        builder.Append(httpRequest.QueryString.Value);
                    }
                }
            }
            else if (HasPropertiesFlag(AspNetRequestUrlProperty.Query))
            {
                builder.Append(httpRequest.QueryString.Value);
            }
        }
#endif
    }
}
