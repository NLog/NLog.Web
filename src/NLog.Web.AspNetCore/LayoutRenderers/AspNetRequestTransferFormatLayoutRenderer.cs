#if ASP_NET_CORE3
using System.Text;
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Gets the transfer format of the protocol.
    /// </summary>
    [LayoutRenderer("aspnet-request-transfer-format")]
    public class AspNetRequestTransferFormatLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// ActiveFormat or SupportedFormats
        /// </summary>
        public TransferFormatProperty Property { get; set; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var transferFormatFeature = features?.Get<ITransferFormatFeature>();
            switch (Property)
            {
                case TransferFormatProperty.ActiveFormat:
                    builder.Append(transferFormatFeature?.ActiveFormat);
                    break;
                case TransferFormatProperty.SupportedFormats:
                    builder.Append(transferFormatFeature?.SupportedFormats);
                    break;
            }
        }
    }
}
#endif
