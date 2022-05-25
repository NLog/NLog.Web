using System;
using System.Text;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// Contains the configuration for the NLogRequestPostedBodyMiddleware
    /// </summary>
    public class NLogRequestPostedBodyMiddlewareConfiguration
    {
        /// <summary>
        /// The default configuration
        /// </summary>
        public static readonly NLogRequestPostedBodyMiddlewareConfiguration Default = new NLogRequestPostedBodyMiddlewareConfiguration();

        /// <summary>
        /// Defaults to UTF-8
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Defaults to 1024
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// Defaults to true
        /// </summary>
        public bool DetectEncodingFromByteOrderMark { get; set; } = true;

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
            return app?.Context?.Request?.ContentLength != null && app?.Context?.Request?.ContentLength <= 30 * 1024;
        }
    }
}
