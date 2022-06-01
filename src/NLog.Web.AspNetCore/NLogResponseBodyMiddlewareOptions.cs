using System;
using Microsoft.AspNetCore.Http;

namespace NLog.Web
{
    /// <summary>
    /// Contains the configuration for the NLogRequestPostedBodyMiddleware
    /// </summary>
    public class NLogResponseBodyMiddlewareOptions
    {
        /// <summary>
        /// The default configuration
        /// </summary>
        internal static readonly NLogResponseBodyMiddlewareOptions Default = new NLogResponseBodyMiddlewareOptions();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NLogResponseBodyMiddlewareOptions()
        {
            ShouldRetainCapture = DefaultRetainCapture;
            ShouldCapture = DefaultShouldCapture;
        }

        /// <summary>
        /// The maximum response size that will be captured
        /// Defaults to 30KB
        /// </summary>
        public int MaximumResponseSize { get; set; } = 30 * 1024;

        /// <summary>
        /// If this returns true, the response body will be captured
        /// Defaults to true
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpContext> ShouldCapture { get; set; }

        /// <summary>
        /// The default predicate for ShouldCapture
        /// Returns true
        /// </summary>
        private bool DefaultShouldCapture(HttpContext context)
        {
            return true;
        }

        /// <summary>
        /// Defaults to true if content length &lt;= 30KB
        /// </summary>
        public Predicate<HttpContext> ShouldRetainCapture { get; set; }

        /// <summary>
        /// The default predicate for ShouldCapture
        /// Returns true if content length &lt;= 30KB
        /// </summary>
        private bool DefaultRetainCapture(HttpContext context)
        {
            return context?.Response?.ContentLength != null && context?.Response?.ContentLength <= MaximumResponseSize;
        }
    }
}
