using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Specifies which of the 7 properties of ITlsHandshakeFeature to emit
    /// </summary>
    public enum AspNetTlsHandshakeField
    {
        /// <summary>
        /// Gets the CipherAlgorithmType.
        /// </summary>
        CipherAlgorithm,
        /// <summary>
        /// Gets the cipher strength
        /// </summary>
        CipherStrength,
        /// <summary>
        /// Gets the HashAlgorithmType.
        /// </summary>
        HashAlgorithm,
        /// <summary>
        /// Gets the hash strength.
        /// </summary>
        HashStrength,
        /// <summary>
        /// Gets the KeyExchangeAlgorithm.
        /// </summary>
        KeyExchangeAlgorithm,
        /// <summary>
        /// Gets the key exchange algorithm strength.
        /// </summary>
        KeyExchangeStrength,
        /// <summary>
        /// Gets the key exchange algorithm strength.
        /// </summary>
        Protocol
    }

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
        /// </summary>
        public AspNetTlsHandshakeField AspNetTlsHandshakeField { get; set; }

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

            switch (AspNetTlsHandshakeField)
            {
                case AspNetTlsHandshakeField.CipherAlgorithm:
                    builder.Append(tlsHandshake.CipherAlgorithm);
                    break;
                case AspNetTlsHandshakeField.CipherStrength:
                    builder.Append(tlsHandshake.CipherStrength);
                    break;
                case AspNetTlsHandshakeField.HashAlgorithm:
                    builder.Append(tlsHandshake.HashAlgorithm);
                    break;
                case AspNetTlsHandshakeField.HashStrength:
                    builder.Append(tlsHandshake.HashStrength);
                    break;
                case AspNetTlsHandshakeField.KeyExchangeAlgorithm:
                    builder.Append(tlsHandshake.KeyExchangeAlgorithm);
                    break;
                case AspNetTlsHandshakeField.KeyExchangeStrength:
                    builder.Append(tlsHandshake.KeyExchangeStrength);
                    break;
                case AspNetTlsHandshakeField.Protocol:
                    builder.Append(tlsHandshake.Protocol);
                    break;
            }
#endif
        }
    }
}
