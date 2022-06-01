using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using NLog.Common;
using NLog.Web.LayoutRenderers;

namespace NLog.Web
{
    /// <summary>
    /// This class is to intercept the HTTP pipeline and to allow additional logging of the following
    ///
    /// Response body
    ///
    /// The following are saved in the HttpContext.Items collection
    ///
    /// __nlog-aspnet-response-body
    ///
    /// Usage: app.UseMiddleware&lt;NLogResponseBodyMiddleware&gt;(); where app is an IApplicationBuilder
    /// Register the NLogBodyMiddlewareOptions in the IoC so that the config gets passed to the constructor
    /// Please use with caution, this will temporarily use 2x the memory for the response, so this may be
    /// suitable only for responses &lt; 64 KB
    /// </summary>
    public class NLogResponseBodyMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly NLogResponseBodyMiddlewareOptions _options;

        // Using this instead of new MemoryStream() is important to the performance.
        // According to the articles, this should be used as a static and not as an instance.
        // This will manage a pool of MemoryStream instead of creating a new MemoryStream every response.
        private static readonly RecyclableMemoryStreamManager Manager = new RecyclableMemoryStreamManager();

        /// <summary>
        /// Constructor that takes a configuration
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public NLogResponseBodyMiddleware(RequestDelegate next, NLogResponseBodyMiddlewareOptions options)
        {
            _next = next;
            _options = options;
        }

        /// <summary>
        /// This allows interception of the HTTP pipeline for logging purposes
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (ShouldCaptureResponseBody(context))
            {
                using (var memoryStream = Manager.GetStream())
                {
                    // Save away the true response stream
                    var originalStream = context.Response.Body;

                    // Make the Http Context Response Body refer to the Memory Stream
                    context.Response.Body = memoryStream;

                    // The Http Context Response then writes to the Memory Stream
                    await _next(context).ConfigureAwait(false);

                    var responseBody = await GetString(memoryStream).ConfigureAwait(false);

                    // Copy the contents of the memory stream back to the true response stream
                    await memoryStream.CopyToAsync(originalStream).ConfigureAwait(false);

                    // This next line enables NLog to log the response
                    if (!string.IsNullOrEmpty(responseBody) && _options.ShouldRetainCapture(context))
                    {
                        context.Items[AspNetResponseBodyLayoutRenderer.NLogResponseBodyKey] = responseBody;
                    }
                }
            }
            else
            {
                if (context != null)
                {
                    await _next(context).ConfigureAwait(false);
                }
            }
        }

        private bool ShouldCaptureResponseBody(HttpContext context)
        {
            // Perform null checking
            if (context == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // Perform null checking
            if (context.Response == null)
            {
                InternalLogger.Debug("NLogResponseBodyMiddleware: HttpContext.Response is null");
                return false;
            }

            // Perform null checking
            if (context.Response.Body == null)
            {
                InternalLogger.Debug("NLogResponseBodyMiddleware: HttpContext.Response.Body stream is null");
                return false;
            }

            // If we cannot write the response stream we cannot capture the body
            if (!context.Response.Body.CanWrite)
            {
                InternalLogger.Debug("NLogResponseBodyMiddleware: HttpContext.Response.Body stream is non-writeable");
                return false;
            }

            // Use the predicate in the configuration instance that takes the HttpContext as an argument
            if (!_options.ShouldCapture(context))
            {
                InternalLogger.Debug("NLogResponseBodyMiddleware: _configuration.ShouldCapture(HttpContext) predicate returned false");
                return false;
            }

            return true;
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
