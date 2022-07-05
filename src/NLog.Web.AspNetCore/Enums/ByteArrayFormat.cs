namespace NLog.Web.Enums
{
    /// <summary>
    /// Controls how a byte array should be logged
    /// </summary>
    public enum ByteArrayFormat
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
