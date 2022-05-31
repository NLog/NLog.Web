using System;
using Microsoft.AspNetCore.Http;

namespace NLog.Web
{
    /// <summary>
    /// Contains the configuration for the NLogRequestPostedBodyMiddleware
    /// </summary>
    public class NLogRequestPostedBodyMiddlewareOptions
    {
        /// <summary>
        /// The default configuration
        /// </summary>
        internal static readonly NLogRequestPostedBodyMiddlewareOptions Default = new NLogRequestPostedBodyMiddlewareOptions();

        /// <summary>
        /// The default constructor
        /// </summary>
        public NLogRequestPostedBodyMiddlewareOptions()
        {
            ShouldCapture = DefaultCapture;
        }

        /// <summary>
        /// The maximum request size that will be captured
        /// Defaults to 30KB.  This checks against the ContentLength.
        /// HttpRequest.EnableBuffer() writes the request to TEMP files on disk if the request ContentLength is > 30KB
        /// but uses memory otherwise if &lt;= 30KB, so we should protect against "very large"
        /// request post body payloads.
        /// </summary>
        public int MaximumRequestSize { get; set; } = 30 * 1024;

        /// <summary>
        /// If this returns true, the post request body will be captured
        /// Defaults to true if content length &lt;= 30KB
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpContext> ShouldCapture { get; set; }

        /// <summary>
        /// The default predicate for ShouldCapture
        /// Returns true if content length &lt;= 30KB
        /// </summary>
        private bool DefaultCapture(HttpContext context)
        {
            return context?.Request?.ContentLength != null && context?.Request?.ContentLength <= MaximumRequestSize;
        }
    }
}
