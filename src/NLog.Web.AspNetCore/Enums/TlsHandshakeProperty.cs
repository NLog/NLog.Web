namespace NLog.Web.Enums
{
    /// <summary>
    /// Specifies which of the 7 properties of ITlsHandshakeFeature to emit
    /// </summary>
    public enum TlsHandshakeProperty
    {
        /// <summary>
        /// Gets the CipherAlgorithmType.
        /// </summary>
        CipherAlgorithm,
        /// <summary>
        /// Gets the cipher strength
        /// </summary>
        CipherStrength,
        /// <summary>
        /// Gets the HashAlgorithmType.
        /// </summary>
        HashAlgorithm,
        /// <summary>
        /// Gets the hash strength.
        /// </summary>
        HashStrength,
        /// <summary>
        /// Gets the KeyExchangeAlgorithm.
        /// </summary>
        KeyExchangeAlgorithm,
        /// <summary>
        /// Gets the key exchange algorithm strength.
        /// </summary>
        KeyExchangeStrength,
        /// <summary>
        /// Gets the SslProtocols.
        /// </summary>
        Protocol
    }
}
