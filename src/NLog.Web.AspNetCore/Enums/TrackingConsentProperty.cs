
namespace NLog.Web.Enums
{
    /// <summary>
    /// Used to query, grant, and withdraw user consent regarding the storage of user information related to site activity and functionality.
    /// </summary>
    public enum TrackingConsentProperty
    {
        /// <summary>
        /// Indicates either if consent has been given or if consent is not required.
        /// </summary>
        CanTrack,
        /// <summary>
        /// Indicates if consent was given.
        /// </summary>
        HasConsent,
        /// <summary>
        /// Indicates if consent is required for the given request.
        /// </summary>
        IsConsentNeeded
    }
}
