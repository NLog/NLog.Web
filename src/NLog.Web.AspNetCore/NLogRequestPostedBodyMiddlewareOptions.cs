using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using NLog.Common;
using NLog.Web.Internal;

namespace NLog.Web
{
    /// <summary>
    /// Contains the configuration for the NLogRequestPostedBodyMiddleware
    /// </summary>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-posted-body-layout-renderer">Documentation on NLog Wiki</seealso>
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
            AllowContentTypes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("application/", "json"),
                new KeyValuePair<string, string>("text/", ""),
                new KeyValuePair<string, string>("", "charset"),
                new KeyValuePair<string, string>("application/", "xml"),
                new KeyValuePair<string, string>("application/", "html")
            };
        }

        /// <summary>
        /// The maximum request posted body size that will be captured. Defaults to 30KB.
        /// </summary>
        /// <remarks>
        /// HttpRequest.EnableBuffer() writes the request to TEMP files on disk if the request ContentLength is > 30KB
        /// but uses memory otherwise if &lt;= 30KB, so we should protect against "very large" request post body payloads.
        /// </remarks>
        public int MaxContentLength { get; set; } = 30 * 1024;

        /// <summary>
        /// Prefix and suffix values to be accepted as ContentTypes. Ex. key-prefix = "application/" and value-suffix = "json"
        /// </summary>
        public IList<KeyValuePair<string,string>> AllowContentTypes { get; set; }

        /// <summary>
        /// If this returns true, the post request body will be captured
        /// Defaults to true if content length &lt;= 30KB
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpContext> ShouldCapture { get; set; }

        /// <summary>
        /// The default predicate for ShouldCapture. Returns true if content length &lt;= 30KB
        /// </summary>
        private bool DefaultCapture(HttpContext context)
        {
            var contentLength = context?.Request?.ContentLength ?? 0;
            if (contentLength <= 0 || contentLength > MaxContentLength)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.ContentLength={0}", contentLength);
                return false;
            }

            if (!context.HasAllowedContentType(AllowContentTypes))
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.ContentType={0}", context.Request.ContentType);
                return false;
            }

            return true;
        }
    }
}
