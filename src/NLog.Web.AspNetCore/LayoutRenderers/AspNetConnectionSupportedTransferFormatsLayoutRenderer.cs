﻿#if ASP_NET_CORE3
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Supported Transfer Formats of the connection
    /// </summary>
    [LayoutRenderer("aspnet-connection-supported-transfer-formats")]
    public class AspNetConnectionSupportedTransferFormatsLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// ASP.NET Active Transfer Format of the connection
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
            var transferFormatFeature = features.Get<ITransferFormatFeature>();
            if (transferFormatFeature == null)
            {
                return;
            }
            builder.Append(transferFormatFeature.SupportedFormats);

        }
    }
}
#endif
