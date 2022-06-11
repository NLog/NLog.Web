using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET TSL Handshake Protocol
    /// </summary>
    /// <remarks>
    /// ${aspnet-tls-handshake-protocol}
    /// </remarks>
    [LayoutRenderer("aspnet-tls-handshake-protocol")]
    public class AspNetTlsHandshakeProtocol : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render TLS Handshake Protocol
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
            builder.Append(tlsHandshake.Protocol);
#endif
        }
    }
}
