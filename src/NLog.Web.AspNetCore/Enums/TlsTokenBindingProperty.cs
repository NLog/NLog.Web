#if ASP_NET_CORE3
namespace NLog.Web.Enums
{
    /// <summary>
    /// Tls Token Binding Enumeration
    /// </summary>
    public enum TlsTokenBindingProperty
    {
        /// <summary>
        /// Tls Token Binding for Provider
        /// </summary>
        Provider,
        /// <summary>
        /// Tls Token Binding for Referrer
        /// </summary>
        Referrer
    }
}
#endif
