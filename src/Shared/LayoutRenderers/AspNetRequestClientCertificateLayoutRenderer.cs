using System.Text;
using NLog.LayoutRenderers;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Security.Cryptography.X509Certificates;
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Client Certificate of the Connection
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-client-certificate}
    /// ${aspnet-request-client-certificate:Verbose=True}
    /// </remarks>
    [LayoutRenderer("aspnet-request-client-certificate")]
    public class AspNetRequestClientCertificateLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// This is passed to the X509Certificate2.ToString(bool) method
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Render Remote Port
        /// </summary>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
#if ASP_NET_CORE
            var connection = httpContext.Connection;
            if (connection == null)
            {
                return;
            }
            builder.Append(connection.ClientCertificate?.ToString(Verbose));
#else
            var certificate = httpContext.Request.ClientCertificate;
            if (certificate == null)
            {
                return;
            }
            // Convert to an X509Certificate2, which does have the proper overridden ToString() method.
            // HttpClientCertificate class only use object.ToString() which is useless.
            builder.Append(new X509Certificate2(certificate.Certificate)?.ToString(Verbose));
#endif
        }
    }
}