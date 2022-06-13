using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET TSL Handshake
    /// </summary>
    /// <remarks>
    /// ${aspnet-tls-handshake}
    /// </remarks>
    [LayoutRenderer("aspnet-tls-handshake")]
    public class AspNetTlsHandshakeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Specifies which of the 7 properties of ITlsHandshakeFeature to emit
        /// Defaults to the protocol
        /// </summary>
        public TlsHandshakeProperty Property { get; set; } = TlsHandshakeProperty.Protocol;

        /// <summary>
        /// Render TLS Handshake Cipher Algorithm
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE3
            var tlsHandshake = HttpContextAccessor.HttpContext.TryGetTlsHandshake();
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
#endif
        }
    }
}
