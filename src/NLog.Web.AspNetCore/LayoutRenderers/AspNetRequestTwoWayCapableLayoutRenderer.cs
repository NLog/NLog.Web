using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Indicates if the server can upgrade this request to an opaque, bidirectional stream.
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-two-way-capable}
    /// </remarks>
    [LayoutRenderer("aspnet-request-two-way-capable")]
    public class AspNetRequestTwoWayCapableLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render the indicator that the server can upgrade this request to an opaque, bidirectional stream.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if(features == null)
            {
                return;
            }
            var upgradeFeature = features.Get<IHttpUpgradeFeature>();
            if (upgradeFeature == null)
            {
                return;
            }
            builder.Append(upgradeFeature.IsUpgradableRequest ? '1': '0');
        }
    }
}
