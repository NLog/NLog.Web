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
    /// POST request body
    ///
    /// Usage: app.UseMiddleware&lt;NLogRequestPostBodyMiddleware&gt;(); where app is an IApplicationBuilder
    ///
    /// Inject the NLogRequestPostBodyMiddlewareOption in the IoC if wanting to override default values for constructor
    /// </summary>
    public class NLogRequestPostedBodyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly NLogRequestPostedBodyMiddlewareOptions _options;

        // Using this instead of new MemoryStream() is important to the performance.
        // According to the articles, this should be used as a static and not as an instance.
        // This will manage a pool of MemoryStream instead of creating a new MemoryStream every response.
        // Otherwise we will end up doing new MemoryStream 1000s of time a minute
        private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new RecyclableMemoryStreamManager();

        /// <summary>
        /// Initializes new instance of the <see cref="NLogRequestPostedBodyMiddleware"/> class
        /// </summary>
        /// <remarks>
        /// Use the following in Startup.cs:
        /// <code>
        /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        /// {
        ///    app.UseMiddleware&lt;NLog.Web.NLogRequestPostedBodyMiddleware&gt;();
        /// }
        /// </code>
        /// </remarks>
        public NLogRequestPostedBodyMiddleware(RequestDelegate next, NLogRequestPostedBodyMiddlewareOptions options = default)
        {
            _next = next;
            _options = options ?? NLogRequestPostedBodyMiddlewareOptions.Default;
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

                var requestBody = await GetString(context.Request.Body).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(requestBody))
                {
                    context.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = requestBody;
                }
            }

            if (ShouldCaptureResponseBody(context))
            {
                using (var memoryStream = MemoryStreamManager.GetStream())
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
                    if (!string.IsNullOrEmpty(responseBody) && _options.ShouldRetainResponse(context))
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

        private bool ShouldCaptureRequestBody(HttpContext context)
        {
            // Perform null checking
            if (context == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext is null");
                return false;
            }

            if (context.Request == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request is null");
                return false;
            }

            // Perform null checking
            if (context.Request.Body == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is null");
                return false;
            }

            // If we cannot read the stream we cannot capture the body
            if (!context.Request.Body.CanRead)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is non-readable");
                return false;
            }

            // Use the ShouldCaptureRequest predicate in the configuration instance that takes the HttpContext as an argument
            if (!_options.ShouldCaptureRequest(context))
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: _configuration.ShouldCaptureRequest(HttpContext) predicate returned false");
                return false;
            }

            return true;
        }

        private bool ShouldCaptureResponseBody(HttpContext context)
        {
            // Perform null checking
            if (context == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext is null");
                return false;
            }

            // Perform null checking
            if (context.Response == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Response is null");
                return false;
            }

            // Perform null checking
            if (context.Response.Body == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Response.Body stream is null");
                return false;
            }

            // If we cannot write the response stream we cannot capture the body
            if (!context.Response.Body.CanWrite)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Response.Body stream is non-writeable");
                return false;
            }

            // Use the ShouldCaptureResponse predicate in the configuration instance that takes the HttpContext as an argument
            if (!_options.ShouldCaptureResponse(context))
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: _configuration.ShouldCaptureResponse(HttpContext) predicate returned false");
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
            string responseText = null;

            // If we cannot seek the stream we cannot capture the body
            if (!stream.CanSeek)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext Body stream is non-seekable");
                return responseText;
            }

            // Save away the original stream position
            var originalPosition = stream.Position;

            try
            {
                // This is required to reset the stream position to the beginning in order to properly read all of the stream.
                stream.Position = 0;

                // The last argument, leaveOpen, is set to true, so that the stream is not pre-maturely closed
                // therefore preventing the next reader from reading the stream.
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
            }
            finally
            {
                // This is required to reset the stream position to the original, in order to
                // properly let the next reader process the stream from the original point
                stream.Position = originalPosition;
            }

            // Return the string of the body
            return responseText;
        }
    }
}
