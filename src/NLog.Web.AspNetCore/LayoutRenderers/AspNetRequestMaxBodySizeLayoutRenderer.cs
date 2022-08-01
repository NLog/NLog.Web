#if ASP_NET_CORE3
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
    /// 
    /// CanRead indicates whether MaxRequestBodySize is read-only.
    /// If true, this could mean that the request body has already been read from or that UpgradeAsync() was called.
    /// 
    /// MaxBodyRequestSize is the maximum allowed size of the current request body in bytes.
    /// When set to null, the maximum request body size is unlimited.
    /// This cannot be modified after the reading the request body has started.
    /// This limit does not affect upgraded connections which are always unlimited.
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
