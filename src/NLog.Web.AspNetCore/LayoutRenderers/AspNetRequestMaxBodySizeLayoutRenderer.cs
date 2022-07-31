﻿#if ASP_NET_CORE3
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// The maximum request body size for a single request
    /// Uses IHttpMaxRequestBodySizeFeature
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-max-body-size:Property=IsReadOnly}</code>
    /// <code>${aspnet-request-max-body-size:Property=MaxBodyRequestSize}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-max-body-size")]
    public class AspNetRequestMaxBodySizeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// IsReadOnly or MaxBodyRequestSize
        /// </summary>
        public MaxBodyRequestSizeProperty Property { get; set; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var maxBodyRequestSizeFeature = features?.Get<IHttpMaxRequestBodySizeFeature>();
            switch (Property)
            {
                case MaxBodyRequestSizeProperty.IsReadOnly:
                    builder.Append(maxBodyRequestSizeFeature?.IsReadOnly == true ? '1' : '0');
                    break;
                case MaxBodyRequestSizeProperty.MaxBodyRequestSize:
                    builder.Append(maxBodyRequestSizeFeature?.MaxRequestBodySize);
                    break;
            }
        }
    }
}
#endif
