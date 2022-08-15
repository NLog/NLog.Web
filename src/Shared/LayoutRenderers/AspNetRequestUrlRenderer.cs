﻿using System;
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
        [Obsolete("Please use the Properties flags enumeration instead")]
        public bool IncludeQueryString 
        {
            get => GetFlagEnum(AspNetRequestUrlProperty.QueryString);
            set
            {
                SetFlagEnum(AspNetRequestUrlProperty.QueryString, value);
            }
        }

        /// <summary>
        /// To specify whether to include / exclude the Port. Default is false.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead")]
        public bool IncludePort
        {
            get => GetFlagEnum(AspNetRequestUrlProperty.Port);
            set
            {
                SetFlagEnum(AspNetRequestUrlProperty.Port, value);
            }
        }

        /// <summary>
        /// To specify whether to exclude / include the host. Default is true.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead")]
        public bool IncludeHost
        {
            get => GetFlagEnum(AspNetRequestUrlProperty.Host);
            set
            {
                SetFlagEnum(AspNetRequestUrlProperty.Host, value);
            }
        }

        /// <summary>
        /// To specify whether to exclude / include the scheme. Default is true.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead")]
        public bool IncludeScheme
        {
            get => GetFlagEnum(AspNetRequestUrlProperty.Scheme);
            set
            {
                SetFlagEnum(AspNetRequestUrlProperty.Scheme, value);
            }
        }

        /// <summary>
        /// To specify whether to exclude / include the url-path. Default is true.
        /// </summary>
        [Obsolete("Please use the Properties flags enumeration instead")]
        public bool IncludePath
        {
            get => GetFlagEnum(AspNetRequestUrlProperty.Path);
            set
            {
                SetFlagEnum(AspNetRequestUrlProperty.Path, value);
            }
        }

        /// <summary>
        /// Helper to set the bit in the flags enum properly
        /// </summary>
        /// <param name="bit"></param>
        /// <param name="flag"></param>
        private void SetFlagEnum(AspNetRequestUrlProperty bit, bool flag)
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

        /// <summary>
        /// helper to get the bit in the flags enum
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        private bool GetFlagEnum(AspNetRequestUrlProperty bit)
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
            if (Properties == AspNetRequestUrlProperty.None)
            {
                return;
            }

            var url = httpRequest.Url;
            if (url == null)
            {
                return;
            }

            if (GetFlagEnum(AspNetRequestUrlProperty.Scheme) && 
                !string.IsNullOrEmpty(url.Scheme))
            {
                builder.Append(url.Scheme);
                builder.Append("://");
            }
            if (GetFlagEnum(AspNetRequestUrlProperty.Host))
            {
                builder.Append(url.Host);
            }
            if (GetFlagEnum(AspNetRequestUrlProperty.Port) && url.Port > 0)
            {
                builder.Append(':');
                builder.Append(url.Port);
            }

            if (GetFlagEnum(AspNetRequestUrlProperty.Path))
            {
                var pathAndQuery = (Properties & AspNetRequestUrlProperty.QueryString) == AspNetRequestUrlProperty.QueryString ? 
                    url.PathAndQuery : url.AbsolutePath;
                builder.Append(pathAndQuery);
            }
            else if (GetFlagEnum(AspNetRequestUrlProperty.QueryString))
            {
                builder.Append(url.Query);
            }
        }
#else
        private void RenderUrl(HttpRequest httpRequest, StringBuilder builder)
        {
            if (Properties == AspNetRequestUrlProperty.None)
            {
                return;
            }

            if (GetFlagEnum(AspNetRequestUrlProperty.Scheme) && 
                !string.IsNullOrWhiteSpace(httpRequest.Scheme))
            {
                builder.Append(httpRequest.Scheme);
                builder.Append("://");
            }

            if (GetFlagEnum(AspNetRequestUrlProperty.Host))
            {
                builder.Append(httpRequest.Host.Host);
            }

            if (GetFlagEnum(AspNetRequestUrlProperty.Port) && 
                httpRequest.Host.Port > 0)
            {
                builder.Append(':');
                builder.Append(httpRequest.Host.Port.Value);
            }

            if (GetFlagEnum(AspNetRequestUrlProperty.Path))
            {
                IHttpRequestFeature httpRequestFeature;
                if (UseRawTarget && (httpRequestFeature = httpRequest.HttpContext.Features.Get<IHttpRequestFeature>()) != null)
                {
                    builder.Append(httpRequestFeature.RawTarget);
                }
                else
                {
                    builder.Append((httpRequest.PathBase + httpRequest.Path).ToUriComponent());

                    if (GetFlagEnum(AspNetRequestUrlProperty.QueryString))
                    {
                        builder.Append(httpRequest.QueryString.Value);
                    }
                }
            }
            else if (GetFlagEnum(AspNetRequestUrlProperty.QueryString))
            {
                builder.Append(httpRequest.QueryString.Value);
            }
        }
#endif
    }
}
