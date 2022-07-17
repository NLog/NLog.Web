using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Indicates if the server can upgrade this request to an opaque, bidirectional stream.
    /// 1 if Capable
    /// 0 if Incapable
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-bidirectional-stream}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-bidirectional-stream")]
    public class AspNetRequestBidirectionalStreamLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var upgradeFeature = features?.Get<IHttpUpgradeFeature>();
            builder.Append(upgradeFeature?.IsUpgradableRequest == true ? '1': '0');
        }
    }
}
