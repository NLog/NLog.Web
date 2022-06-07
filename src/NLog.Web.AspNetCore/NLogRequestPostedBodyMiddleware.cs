using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Common;
using NLog.Web.Internal;
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

                var requestBody = await context.Request.Body.GetString().ConfigureAwait(false);

                if (!string.IsNullOrEmpty(requestBody))
                {
                    context.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = requestBody;
                }
            }

            if (context != null)
            {
                await _next(context).ConfigureAwait(false);
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
            if (!_options.ShouldCapture(context))
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: _configuration.ShouldCapture(HttpContext) predicate returned false");
                return false;
            }

            return true;
        }
    }
}
