#if ASP_NET_CORE3
namespace NLog.Web.Enums
{
    /// <summary>
    /// Determines which property of ITransferFormatFeature to render
    /// </summary>
    public enum TransferFormatProperty
    {
        /// <summary>
        /// Render the ActiveFormat enum
        /// </summary>
        ActiveFormat,
        /// <summary>
        /// Render the SupportedFormats enum
        /// </summary>
        SupportedFormats
    }
}
#endif
