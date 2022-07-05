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
    /// ASP.NET TLS Token Binding
    ///
    /// TLS token bindings help mitigate the risk of impersonation by an attacker in the event
    /// an authenticated client's bearer tokens are somehow exfiltrated from the client's machine.
    /// See https://datatracker.ietf.org/doc/draft-popov-token-binding/ for more information.
    /// </summary>
    /// <remarks>
    /// ${aspnet-tls-token-binding:Property=Provider:Format-Base64}
    /// ${aspnet-tls-token-binding:Property=Referrer:Format=Hex} for hexadecimal separated by the dash character
    /// </remarks>
    [LayoutRenderer("aspnet-tls-token-binding")]
    public class AspNetTlsTokenBindingLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Tls Token Binding Enumeration
        /// Defaults to Provider
        /// </summary>
        [DefaultParameter]
        public TlsTokenBindingProperty Property { get; set; } = TlsTokenBindingProperty.Provider;

        /// <summary>
        /// Tls Token Binding Format, Hex or Base64
        /// Defaults to Hex
        /// </summary>
        [DefaultParameter]
        public ByteArrayFormat Format { get; set; } = ByteArrayFormat.Hex;

        /// <summary>
        /// Renders the ASP.NET TLS Token Binding
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

            var tlsTokenBinding= features.Get<ITlsTokenBindingFeature>();
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
                case ByteArrayFormat.Base64:
                    return Convert.ToBase64String(bytes);

                case ByteArrayFormat.Hex:
                    return BitConverter.ToString(bytes);
            }
            return null;
        }
    }
}
#endif
