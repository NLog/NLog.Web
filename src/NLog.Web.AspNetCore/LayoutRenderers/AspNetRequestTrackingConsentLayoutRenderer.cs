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
    /// <code>
    /// ${aspnet-request-tracking-consent:Property=CanTrack}
    /// ${aspnet-request-tracking-consent:Property=HasConsent}
    /// ${aspnet-request-tracking-consent:Property=IsConsentNeeded}
    /// </code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-tracking-consent")]
    public class AspNetRequestTrackingConsentLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets what property to emit from ITrackingConsentFeature. Default = CanTrack
        /// </summary>
        [DefaultParameter]
        public TrackingConsentProperty Property { get; set; } = TrackingConsentProperty.CanTrack;

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var trackingConsent = features?.Get<ITrackingConsentFeature>();
            switch (Property)
            {
                case TrackingConsentProperty.CanTrack:
                    builder.Append(trackingConsent?.CanTrack == false ? '0' : '1');
                    break;
                case TrackingConsentProperty.HasConsent:
                    builder.Append(trackingConsent?.HasConsent == true ? '1' : '0');
                    break;
                case TrackingConsentProperty.IsConsentNeeded:
                    builder.Append(trackingConsent?.IsConsentNeeded == true ? '1' : '0');
                    break;
            }
        }
    }
}
