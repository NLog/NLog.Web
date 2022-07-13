using Microsoft.AspNetCore.Http.Features;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Tracking Consent
    /// </summary>
    /// <remarks>
    /// ${aspnet-tracking-consent:Property=CanTrack}
    /// ${aspnet-tracking-consent:Property=HasConsent}
    /// ${aspnet-tracking-consent:Property=IsConsentNeeded}
    /// </remarks>
    [LayoutRenderer("aspnet-tracking-consent")]
    public class AspNetTrackingConsentLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Specifies which of the 3 properties of ITrackingConsentFeature to emit
        /// Defaults to CanTrack
        /// </summary>
        [DefaultParameter]
        public TrackingConsentProperty Property { get; set; } = TrackingConsentProperty.CanTrack;

        ///<inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if(features == null)
            {
                builder.Append('0');
                return;
            }
            var trackingConsent = features.Get<ITrackingConsentFeature>();
            if (trackingConsent == null)
            {
                builder.Append('0');
                return;
            }
            switch (Property)
            {
                case TrackingConsentProperty.CanTrack:
                    builder.Append(trackingConsent.CanTrack ? '1': '0');
                    break;
                case TrackingConsentProperty.HasConsent:
                    builder.Append(trackingConsent.HasConsent ? '1': '0');
                    break;
                case TrackingConsentProperty.IsConsentNeeded:
                    builder.Append(trackingConsent.IsConsentNeeded ? '1' : '0');
                    break;
            }
        }
    }
}
