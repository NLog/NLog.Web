using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET TSL Handshake Cipher Strength
    /// </summary>
    /// <remarks>
    /// ${aspnet-tls-handshake-cipher-strength}
    /// </remarks>
    [LayoutRenderer("aspnet-tls-handshake-cipher-strength")]
    public class AspNetTlsHandshakeCipherStrength : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render TLS Handshake Cipher Strength
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
            builder.Append(tlsHandshake.CipherStrength);
#endif
        }
    }
}
