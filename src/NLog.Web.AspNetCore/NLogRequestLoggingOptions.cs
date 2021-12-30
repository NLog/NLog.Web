using System.Collections.Generic;

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

        /// <summary>
        /// Get or set duration time in milliseconds, before a HttpRequest is seen as slow (Logged as warning)
        /// </summary>
        public int DurationThresholdMs { get; set; } = 300;

        /// <summary>
        /// Gets or sets request-paths where LogLevel should be reduced (Logged as debug)
        /// </summary>
        /// <remarks>
        /// Example '/healthcheck'
        /// </remarks>
        public ISet<string> ExcludeRequestPaths { get; } = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
    }
}
