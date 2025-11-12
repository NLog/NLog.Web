#if NETCOREAPP3_0_OR_GREATER

using System;

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
#if NET8_0_OR_GREATER
        [Obsolete("Use NegotiatedCipherSuite instead")]
#endif
        CipherAlgorithm,
        /// <summary>
        /// Gets the cipher strength
        /// </summary>
#if NET8_0_OR_GREATER
        [Obsolete("Use NegotiatedCipherSuite instead")]
#endif
        CipherStrength,
        /// <summary>
        /// Gets the HashAlgorithmType.
        /// </summary>
#if NET8_0_OR_GREATER
        [Obsolete("Use NegotiatedCipherSuite instead")]
#endif
        HashAlgorithm,
        /// <summary>
        /// Gets the hash strength.
        /// </summary>
#if NET8_0_OR_GREATER
        [Obsolete("Use NegotiatedCipherSuite instead")]
#endif
        HashStrength,
        /// <summary>
        /// Gets the KeyExchangeAlgorithm.
        /// </summary>
#if NET8_0_OR_GREATER
        [Obsolete("Use NegotiatedCipherSuite instead")]
#endif
        KeyExchangeAlgorithm,
        /// <summary>
        /// Gets the key exchange algorithm strength.
        /// </summary>
#if NET8_0_OR_GREATER
        [Obsolete("Use NegotiatedCipherSuite instead")]
#endif
        KeyExchangeStrength,
        /// <summary>
        /// Gets the SslProtocols.
        /// </summary>
        Protocol,
        /// <summary>
        /// Gets the HostName (Requires NET 8.0 or greater).
        /// </summary>
        HostName,
        /// <summary>
        /// Gets the <see cref="System.Net.Security.TlsCipherSuite"/> (Requires NET 8.0 or greater).
        /// </summary>
        NegotiatedCipherSuite,
    }
}

#endif
