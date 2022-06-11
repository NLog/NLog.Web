using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET TSL Handshake Key Exchange Algorithm
    /// </summary>
    /// <remarks>
    /// ${aspnet-tls-handshake-key-exchange-algorithm}
    /// </remarks>
    [LayoutRenderer("aspnet-tls-handshake-key-exchange-algorithm")]
    public class AspNetTlsHandshakeKeyExchangeAlgorithm : AspNetLayoutRendererBase
    {
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
            builder.Append(tlsHandshake.KeyExchangeAlgorithm);
#endif
        }
    }
}
