#if NETCOREAPP3_0_OR_GREATER

namespace NLog.Web.Enums
{
    /// <summary>
    /// Controls how a byte array should be logged
    /// </summary>
    public enum ByteArrayFormatProperty
    {
        /// <summary>
        /// Emit the byte array using Convert.ToBase64(byte[])
        /// </summary>
        Base64,
        /// <summary>
        /// Emit the byte array using BitConverter.ToString(byte[])
        /// </summary>
        Hex
    }
}

#endif
