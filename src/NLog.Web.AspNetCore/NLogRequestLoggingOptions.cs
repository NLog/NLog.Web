namespace NLog.Web
{
    /// <summary>
    /// Options configuration for <see cref="NLogRequestLoggingMiddleware"/>
    /// </summary>
    public sealed class NLogRequestLoggingOptions
    {
        internal static readonly NLogRequestLoggingOptions Default = new NLogRequestLoggingOptions();

        /// <summary>
        /// Logger-name used for logging http-requests
        /// </summary>
        public string LoggerName { get; set; } = "NLogRequestLogging";
    }
}
