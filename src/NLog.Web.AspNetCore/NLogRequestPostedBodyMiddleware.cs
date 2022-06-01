using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Common;
using NLog.Web.LayoutRenderers;

namespace NLog.Web
{
    /// <summary>
    /// This class is to intercept the HTTP pipeline and to allow additional logging of the following
    ///
    /// POST request body
    ///
    /// Usage: app.UseMiddleware&lt;NLogRequestPostBodyMiddleware&gt;(); where app is an IApplicationBuilder
    /// Register the NLogRequestPostBodyMiddlewareOption in the IoC so that the config gets passed to the constructor
    /// </summary>
    public class NLogRequestPostedBodyMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly NLogRequestPostedBodyMiddlewareOptions _options;

        /// <summary>
        /// Constructor that takes a configuration
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public NLogRequestPostedBodyMiddleware(RequestDelegate next, NLogRequestPostedBodyMiddlewareOptions options)
        {
            _next = next;
            _options = options;
        }

        /// <summary>
        /// This allows interception of the HTTP pipeline for logging purposes
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (ShouldCaptureRequestBody(context))
            {
                // This is required, otherwise reading the request will destructively read the request
                context.Request.EnableBuffering();

                // Save the POST request body in HttpContext.Items with a key of '__nlog-aspnet-request-posted-body'
                var requestBody = await GetString(context.Request.Body).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(requestBody))
                {
                    context.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = requestBody;
                }
            }

            // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
            await _next(context).ConfigureAwait(false);
        }

        private bool ShouldCaptureRequestBody(HttpContext context)
        {
            // Perform null checking
            if (context == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            if (context.Request == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // Perform null checking
            if (context.Request.Body == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // If we cannot read the stream we cannot capture the body
            if (!context.Request.Body.CanRead)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is non-readable");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            return (_options.ShouldCapture(context));
        }

        /// <summary>
        /// Convert the stream to a String for logging.
        /// If the stream is binary please do not utilize this middleware
        /// Arguably, logging a byte array in a sensible format is simply not possible.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>The contents of the Stream read fully from start to end as a String</returns>
        private async Task<string> GetString(Stream stream)
        {
            // Save away the original stream position
            var originalPosition = stream.Position;

            // This is required to reset the stream position to the beginning in order to properly read all of the stream.
            stream.Position = 0;

            string responseText = null;

            // The last argument, leaveOpen, is set to true, so that the stream is not pre-maturely closed
            // therefore preventing the next reader from reading the stream.
            // The middle three arguments are from the configuration instance
            // These default to UTF-8, true, and 1024.
            using (var streamReader = new StreamReader(
                       stream,
                       Encoding.UTF8,
                       true,
                       1024,
                       leaveOpen: true))
            {
                // This is the most straight forward logic to read the entire body
                responseText = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }

            // This is required to reset the stream position to the original, in order to
            // properly let the next reader process the stream from the original point
            stream.Position = originalPosition;

            // Return the string of the body
            return responseText;
        }
    }
}
