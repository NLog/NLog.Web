#if ASP_NET_CORE3
namespace NLog.Web.Enums
{
    /// <summary>
    /// Determines which property of IHttpMaxRequestBodySizeFeature to render
    /// </summary>
    public enum MaxBodyRequestSizeProperty
    {
        /// <summary>
        /// Render the IsReadOnly bool
        /// </summary>
        IsReadOnly,
        /// <summary>
        /// Render the MaxBodyRequestSize Int64
        /// </summary>
        MaxBodyRequestSize
    }
}
#endif
