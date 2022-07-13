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
    /// ${aspnet-request-is-upgradable}
    /// </remarks>
    [LayoutRenderer("aspnet-request-two-way-capable")]
    public class AspNetRequestIsUpgradableLayoutRenderer : AspNetLayoutRendererBase
    {
        ///<inheritdoc/>
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
