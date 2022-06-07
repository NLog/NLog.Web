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
            ShouldCaptureRequest = DefaultCaptureRequest;
            ShouldCaptureResponse = DefaultCaptureResponse;
            ShouldRetainResponse = DefaultRetainResponse;

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
        public int MaxRequestContentLength { get; set; } = 30 * 1024;

        /// <summary>
        /// The maximum response body size that will be captured. Defaults to 30KB.
        /// </summary>
        /// <remarks>
        /// Since we must capture the response body on a MemoryStream first, this will use 2x the amount
        /// of memory that we would ordinarily use for the response.
        /// </remarks>
        public int MaxResponseContentLength { get; set; } = 30 * 1024;

        /// <summary>
        /// Prefix and suffix values to be accepted as ContentTypes. Ex. key-prefix = "application/" and value-suffix = "json"
        /// The defaults are:
        ///
        /// text/*
        /// */charset
        /// application/json
        /// application/xml
        /// application/html
        /// </summary>
        public IList<KeyValuePair<string,string>> AllowContentTypes { get; set; }

        /// <summary>
        /// If this returns true, the post request body will be captured
        /// Defaults to true if content length &lt;= 30KB and request conmtent type is one of the allowed content types
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpContext> ShouldCaptureRequest { get; set; }

        /// <summary>
        /// If this returns true, the response body will be captured
        /// Defaults to true if content length &lt;= 30KB
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpContext> ShouldCaptureResponse { get; set; }

        /// <summary>
        /// If this returns true, the response body will be captured
        /// Defaults to true if content length &lt;= 30KB
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpContext> ShouldRetainResponse { get; set; }

        /// <summary>
        /// The default predicate for ShouldCaptureRequest. Returns true if content length &lt;= 30KB
        /// and if the content type is allowed
        /// </summary>
        private bool DefaultCaptureRequest(HttpContext context)
        {
            var contentLength = context?.Request?.ContentLength ?? 0;
            if (contentLength <= 0 || contentLength > MaxRequestContentLength)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.ContentLength={0}", contentLength);
                return false;
            }

            if (!context.HasAllowedRequestContentType(AllowContentTypes))
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.ContentType={0}", context?.Request?.ContentType);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The default predicate for ShouldCaptureResponse. Returns true
        /// Since we know nothing about the response before the response is created
        /// </summary>
        private bool DefaultCaptureResponse(HttpContext context)
        {
            return true;
        }

        /// <summary>
        /// The default predicate for ShouldRetainCapture.  Returns true if content length &lt;= 30KB
        /// and if the content type is allowed
        /// </summary>
        private bool DefaultRetainResponse(HttpContext context)
        {
            var contentLength = context?.Response?.ContentLength ?? 0;
            if (contentLength <= 0 || contentLength > MaxResponseContentLength)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Response.ContentLength={0}", contentLength);
                return false;
            }

            if (!context.HasAllowedResponseContentType(AllowContentTypes))
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.ContentType={0}", context?.Request?.ContentType);
                return false;
            }

            return true;
        }
    }
}
