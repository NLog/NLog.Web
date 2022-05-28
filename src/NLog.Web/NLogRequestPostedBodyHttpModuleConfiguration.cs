using System;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// Contains the configuration for the NLogRequestPostedBodyMiddleware
    /// </summary>
    public class NLogRequestPostedBodyHttpModuleConfiguration
    {
        /// <summary>
        /// The default configuration
        /// </summary>
        public static readonly NLogRequestPostedBodyHttpModuleConfiguration Default = new NLogRequestPostedBodyHttpModuleConfiguration();

        /// <summary>
        /// Defaults to true
        /// </summary>
        public bool DetectEncodingFromByteOrderMark { get; set; } = true;

        /// <summary>
        /// The maximum request size that will be captured
        /// Defaults to 30KB
        /// </summary>
        public int MaximumRequestSize { get; set; } = 30 * 1024;

        /// <summary>
        /// If this returns true, the post request body will be captured
        /// Defaults to true if content length &lt;= 30KB
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpApplication> ShouldCapture { get; set; } = DefaultCapture;

        /// <summary>
        /// The default predicate for ShouldCapture
        /// Returns true if content length &lt;= 30KB
        /// </summary>
        public static bool DefaultCapture(HttpApplication app)
        {
            return app?.Context?.Request?.ContentLength != null && app?.Context?.Request?.ContentLength <=
                new NLogRequestPostedBodyHttpModuleConfiguration().MaximumRequestSize;
        }
    }
}
