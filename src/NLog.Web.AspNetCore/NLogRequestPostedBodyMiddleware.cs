using System.IO;
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
    /// The following are saved in the HttpContext.Items collection
    ///
    /// __nlog-aspnet-request-posted-body
    ///
    /// Usage: app.UseMiddleware&lt;NLogRequestPostBodyMiddleware&gt;(); where app is an IApplicationBuilder
    /// Register the NLogRequestPostBodyMiddlewareConfiguration in the IoC so that the config gets passed to the constructor
    /// </summary>
    public class NLogRequestPostedBodyMiddleware : IMiddleware
    {
        private NLogRequestPostedBodyMiddlewareConfiguration _configuration { get; }

        /// <summary>
        /// Constructor that takes a configuration
        /// </summary>
        /// <param name="configuration"></param>
        public NLogRequestPostedBodyMiddleware(NLogRequestPostedBodyMiddlewareConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This allows interception of the HTTP pipeline for logging purposes
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <param name="next">The RequestDelegate that is to be executed next in the HTTP pipeline</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Perform null checking
            if (context.Request == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request stream is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await next(context).ConfigureAwait(false);
                return;
            }

            // Perform null checking
            if (context.Request.Body == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await next(context).ConfigureAwait(false);
                return;
            }

            // If we cannot read the stream we cannot capture the body
            if (!context.Request.Body.CanRead)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is non-readable");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await next(context).ConfigureAwait(false);
                return;
            }

            // This is required, otherwise reading the request will destructively read the request
            context.Request.EnableBuffering();

            // If we cannot reset the stream position to zero, and then back to the original position
            // we cannot capture the body
            if (!context.Request.Body.CanSeek)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is non-seekable");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await next(context).ConfigureAwait(false);
                return;
            }

            // Use the predicate in the configuration instance that takes the HttpContext as an argument
            if (_configuration.ShouldCapture(context))
            {
                // Save the POST request body in HttpContext.Items with a key of '__nlog-aspnet-request-posted-body'
                context.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] =
                    await GetString(context?.Request.Body).ConfigureAwait(false);
            }
            else
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: _configuration.ShouldCapture(HttpContext) predicate returned false");
            }

            // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
            await next(context).ConfigureAwait(false);
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
                       _configuration.Encoding,
                       _configuration.DetectEncodingFromByteOrderMark,
                       _configuration.BufferSize,
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
