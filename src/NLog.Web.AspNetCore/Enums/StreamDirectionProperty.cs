#if NET5_0_OR_GREATER
namespace NLog.Web.Enums
{
    /// <summary>
    /// Determines which property of IStreamDirectionFeature to render
    /// </summary>
    public enum StreamDirectionProperty
    {
        /// <summary>
        /// Render the IStreamDirectionFeature.CanRead property
        /// </summary>
        CanRead,
        /// <summary>
        /// Render the IStreamDirectionFeature.CanWrite property
        /// </summary>
        CanWrite
    }
}
#endif
