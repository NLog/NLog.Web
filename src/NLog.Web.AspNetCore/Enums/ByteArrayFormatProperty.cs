#if ASP_NET_CORE3
namespace NLog.Web.Enums
{
    /// <summary>
    /// Controls how a byte array should be logged
    /// </summary>
    public enum ByteArrayFormatProperty
    {
        /// <summary>
        /// Emit the byte array using Convert.ToBase64()
        /// </summary>
        Base64,
        /// <summary>
        /// Emit the byte array using COnvert.ToHexString()
        /// </summary>
        Hex
    }
}
#endif
