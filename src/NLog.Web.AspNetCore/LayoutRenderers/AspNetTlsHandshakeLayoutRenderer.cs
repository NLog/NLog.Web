#if ASP_NET_CORE3
using Microsoft.AspNetCore.Connections.Features;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET TLS Handshake
    /// </summary>
    /// <remarks>
    /// ${aspnet-tls-handshake:Property=CipherAlgorithm}
    /// ${aspnet-tls-handshake:Property=CipherStrength}
    /// ${aspnet-tls-handshake:Property=HashAlgorithm}
    /// ${aspnet-tls-handshake:Property=HashStrength}
    /// ${aspnet-tls-handshake:Property=KeyExchangeAlgorithm}
    /// ${aspnet-tls-handshake:Property=KeyExchangeStrength}
    /// ${aspnet-tls-handshake:Property=Protocol}
    /// </remarks>
    [LayoutRenderer("aspnet-tls-handshake")]
    public class AspNetTlsHandshakeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Specifies which of the 7 properties of ITlsHandshakeFeature to emit
        /// Defaults to the protocol
        /// </summary>
        [DefaultParameter]
        public TlsHandshakeProperty Property { get; set; } = TlsHandshakeProperty.Protocol;

        /// <summary>
        /// Render TLS Handshake Information
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if(features == null)
            {
                return;
            }
            var tlsHandshake = features.Get<ITlsHandshakeFeature>();
            if (tlsHandshake == null)
            {
                return;
            }

            switch (Property)
            {
                case TlsHandshakeProperty.CipherAlgorithm:
                    builder.Append(tlsHandshake.CipherAlgorithm);
                    break;
                case TlsHandshakeProperty.CipherStrength:
                    builder.Append(tlsHandshake.CipherStrength);
                    break;
                case TlsHandshakeProperty.HashAlgorithm:
                    builder.Append(tlsHandshake.HashAlgorithm);
                    break;
                case TlsHandshakeProperty.HashStrength:
                    builder.Append(tlsHandshake.HashStrength);
                    break;
                case TlsHandshakeProperty.KeyExchangeAlgorithm:
                    builder.Append(tlsHandshake.KeyExchangeAlgorithm);
                    break;
                case TlsHandshakeProperty.KeyExchangeStrength:
                    builder.Append(tlsHandshake.KeyExchangeStrength);
                    break;
                case TlsHandshakeProperty.Protocol:
                    builder.Append(tlsHandshake.Protocol);
                    break;
            }

        }
    }
}
#endif