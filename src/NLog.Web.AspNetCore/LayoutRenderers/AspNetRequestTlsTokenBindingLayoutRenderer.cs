#if ASP_NET_CORE3
using System;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET TLS Token Bindings for Provider and Referrer
    ///
    /// TLS token bindings help mitigate the risk of impersonation by an attacker in the event
    /// an authenticated client's bearer tokens are somehow exfiltrated from the client's machine.
    /// See https://datatracker.ietf.org/doc/draft-popov-token-binding/ for more information.
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-tls-token-binding:Property=Provider:Format=Base64}
    /// ${aspnet-request-tls-token-binding:Property=Referrer:Format=Hex}
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-TLS-Token-Bindng-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-tls-token-binding")]
    public class AspNetRequestTlsTokenBindingLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Tls Token Binding Type Enumeration, Provider or Referrer
        /// </summary>
        [DefaultParameter]
        public TlsTokenBindingProperty Property { get; set; }

        /// <summary>
        /// Tls Token Binding Format Enumeration, Hex or Base64
        /// </summary>
        public ByteArrayFormatProperty Format { get; set; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if (features == null)
            {
                return;
            }

            var tlsTokenBinding = features.Get<ITlsTokenBindingFeature>();
            if (tlsTokenBinding == null)
            {
                return;
            }

            switch(Property)
            {
                case TlsTokenBindingProperty.Referrer:
                    builder.Append(ToFormattedString(tlsTokenBinding.GetReferredTokenBindingId()));
                    break;

                case TlsTokenBindingProperty.Provider:
                    builder.Append(ToFormattedString(tlsTokenBinding.GetProvidedTokenBindingId()));
                    break;
            }
        }

        private string ToFormattedString(byte[] bytes)
        {
            if(bytes == null || bytes.Length == 0)
            {
                return null;
            }

            switch (Format)
            {
                case ByteArrayFormatProperty.Base64:
                    return Convert.ToBase64String(bytes);

                case ByteArrayFormatProperty.Hex:
                    return BitConverter.ToString(bytes);
            }

            return null;
        }
    }
}
#endif
