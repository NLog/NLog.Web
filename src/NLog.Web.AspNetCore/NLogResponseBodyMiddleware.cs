using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using NLog.Common;
using NLog.Web.Internal;
using NLog.Web.LayoutRenderers;

namespace NLog.Web
{
    /// <summary>
    /// This class is to intercept the HTTP pipeline and to allow additional logging of the following
    ///
    /// Response Body
    ///
    /// Usage: app.UseMiddleware&lt;NLogResponseBodyMiddleware&gt;(); where app is an IApplicationBuilder
    ///
    /// Inject the NLogResponseBodyMiddlewareOption in the IoC if wanting to override default values for constructor
    /// </summary>
    public class NLogResponseBodyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly NLogResponseBodyMiddlewareOptions _options;

        // Using this instead of new MemoryStream() is important to the performance.
        // According to the articles, this should be used as a static and not as an instance.
        // This will manage a pool of MemoryStream instead of creating a new MemoryStream every response.
        // Otherwise we will end up doing new MemoryStream 1000s of time a minute
        private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new RecyclableMemoryStreamManager();

        /// <summary>
        /// Initializes new instance of the <see cref="NLogResponseBodyMiddleware"/> class
        /// </summary>
        /// <remarks>
        /// Use the following in Startup.cs:
        /// <code>
        /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        /// {
        ///    app.UseMiddleware&lt;NLog.Web.NLogResponseBodyMiddleware&gt;();
        /// }
        /// </code>
        /// </remarks>
        public NLogResponseBodyMiddleware(RequestDelegate next, NLogResponseBodyMiddlewareOptions options = default)
        {
            _next = next;
            _options = options ?? NLogResponseBodyMiddlewareOptions.Default;
        }

        /// <summary>
        /// This allows interception of the HTTP pipeline for logging purposes
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
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

                    var responseBody = await memoryStream.GetString().ConfigureAwait(false);

                    // Copy the contents of the memory stream back to the true response stream
                    await memoryStream.CopyToAsync(originalStream).ConfigureAwait(false);

                    // This next line enables NLog to log the response
                    if (!string.IsNullOrEmpty(responseBody) && _options.ShouldRetain(context))
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
            if (!_options.ShouldCapture(context))
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: _configuration.ShouldCapture(HttpContext) predicate returned false");
                return false;
            }

            return true;
        }
    }
}
