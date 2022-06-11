using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET TSL Handshake Hash Algorithm
    /// </summary>
    /// <remarks>
    /// ${aspnet-tls-handshake-hash-algorithm}
    /// </remarks>
    [LayoutRenderer("aspnet-tls-handshake-hash-algorithm")]
    public class AspNetTlsHandshakeHashAlgorithm : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render TLS Handshake Hash Algorithm
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
            builder.Append(tlsHandshake.HashAlgorithm);
#endif
        }
    }
}
