using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// The maximum allowed size of the current request body in bytes.
    /// When set to null, the maximum request body size is unlimited.
    /// This cannot be modified after the reading the request body has started.
    /// This limit does not affect upgraded connections which are always unlimited.
    /// </summary>
    [LayoutRenderer("aspnet-request-max-body-size")]
    public class AspNetRequestMaxBodySizeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// The maximum allowed size of the current request body in bytes.
        /// When set to null, the maximum request body size is unlimited.
        /// This cannot be modified after the reading the request body has started.
        /// This limit does not affect upgraded connections which are always unlimited.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if (features == null)
            {
                return;
            }
            var maxRequestBodySizeFeature = features.Get<IHttpMaxRequestBodySizeFeature>();
            if (maxRequestBodySizeFeature == null)
            {
                return;
            }
            builder.Append(maxRequestBodySizeFeature.MaxRequestBodySize);
        }
    }
}
