using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NLog.Web
{
    /// <summary>
    /// Middleware that writes all requests to Logger named "RequestLogging"
    /// </summary>
    /// <remarks>
    /// - LogLevel.Error - Request failed with exception<br/>
    /// - LogLevel.Warn  - Request completed with unsucessful StatusCode<br/>
    /// - LogLevel.Info  - Request completed standard StatusCode<br/>
    /// </remarks>
    public class NLogRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly NLogRequestLoggingOptions _options;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        /// <summary>
        /// Initializes new instance of the <see cref="NLogRequestLoggingMiddleware"/> class
        /// </summary>
        /// <remarks>
        /// Use the following in Startup.cs:
        /// <code>
        /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        /// {
        ///    app.UseMiddleware&lt;NLog.Web.RequestLoggingMiddleware&gt;();
        /// }
        /// </code>
        /// </remarks>
        public NLogRequestLoggingMiddleware(RequestDelegate next, NLogRequestLoggingOptions options = default, ILoggerFactory loggerFactory = default)
        {
            _next = next;
            _options = options ?? NLogRequestLoggingOptions.Default;
            _logger = loggerFactory?.CreateLogger(_options.LoggerName ?? NLogRequestLoggingOptions.Default.LoggerName) ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                LogHttpRequest(httpContext, null);
            }
            catch (Exception exception) when (LogHttpRequest(httpContext, exception))
            {
                // Logging complete
            }
        }

        /// <summary>
        /// Exception Filter for better capture of thread-execution-context (Ex. AsyncLocal-state)
        /// </summary>
        private bool LogHttpRequest(HttpContext httpContext, Exception exception)
        {
            if (exception != null)
            {
                _logger.LogError(exception, "HttpRequest Exception");
            }
            else
            {
                var statusCode = httpContext.Response?.StatusCode ?? 0;
                if (statusCode < 100 || statusCode >= 400)
                {
                    _logger.LogWarning("HttpRequest Failed");
                }
                else
                {
                    _logger.LogInformation("HttpRequest Completed");
                }
            }

            return false;   // Exception Filter should not suppress the Exception
        }
    }
}
