using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NLog.Common;
using NLog.Web.Internal;
using NLog.Web.LayoutRenderers;

namespace NLog.Web
{
    /// <summary>
    /// ASP.NET Core Middleware that enables ${aspnet-request-posted-body}
    ///
    /// Inject the NLogRequestPostBodyMiddlewareOption in the IoC if wanting to override default values for constructor
    /// </summary>
    /// <remarks>
    /// Usage:
    /// 
    /// app.UseMiddleware&lt;NLogRequestPostBodyMiddleware&gt;(); where app is an IApplicationBuilder
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-posted-body-layout-renderer">Documentation on NLog Wiki</seealso>
    public class NLogRequestPostedBodyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly NLogRequestPostedBodyMiddlewareOptions _options;

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
            try
            {
                MiddlewareIsInstalled(context);

                if (ShouldCaptureRequestBody(context))
                {
                    // This is required, otherwise reading the request will destructively read the request
                    context.Request.EnableBuffering();

                    var requestBody = await ReadPostedBodyFromStream(context.Request.Body).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(requestBody))
                    {
                        context.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = requestBody;
                    }
                }
            }
            catch (Exception ex)
            {
                InternalLogger.Error(ex, "NLogRequestPostedBodyMiddleware: Failed to read HttpContext.Request.Body");
                if (LogManager.ThrowExceptions)
                    throw;
            }
            finally
            {
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await _next(context);   // NOSONAR
            }
        }

        /// <summary>
        /// Signal to the AspNetRequestPostedBodyLayoutRenderer class that the Middleware is installed.
        /// </summary>
        /// <param name="context"></param>
        private static void MiddlewareIsInstalled(HttpContext context)
        {
            if (context != null)
            {
                context.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyMiddlewareInstalled] = true;
            }
        }

        private bool ShouldCaptureRequestBody(HttpContext context)
        {
            if (context is null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext is null");
                return false;
            }

#if NET5_0_OR_GREATER
            if (context.TryGetFeature<IHttpRequestBodyDetectionFeature>()?.CanHaveBody == false)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Feature.CanHaveBody = false");
                return false;
            }
#endif

            var postedBody = context.TryGetRequest()?.Body;
            if (postedBody is null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is null");
                return false;
            }

            // If we cannot read the stream we cannot capture the body
            if (!postedBody.CanRead)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is non-readable");
                return false;
            }

            return _options.ShouldCapture(context);
        }

        /// <summary>
        /// Convert the stream to a String for logging.
        /// If the stream is binary please do not utilize this middleware
        /// Arguably, logging a byte array in a sensible format is simply not possible.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>The contents of the Stream read fully from start to end as a String</returns>
        private static async Task<string> ReadPostedBodyFromStream(Stream stream)
        {
            // If we cannot seek the stream we cannot capture the body
            if (!stream.CanSeek)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.Body stream is non-seekable");
                return string.Empty;
            }

            string responseText = null;

            // Save away the original stream position
            var originalPosition = stream.Position;

            try
            {
                // This is required to reset the stream position to the beginning in order to properly read all of the stream.
                stream.Position = 0;

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
