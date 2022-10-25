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
            var logLevel = _options.ShouldLogRequest?.Invoke(httpContext, exception) ?? Microsoft.Extensions.Logging.LogLevel.None;
            if (logLevel != Microsoft.Extensions.Logging.LogLevel.None)
            {
                if (exception != null)
                {
                    _logger.Log(logLevel, 0, exception, "HttpRequest Exception");
                }
                else
                {
                    switch (logLevel)
                    {
                        case Microsoft.Extensions.Logging.LogLevel.Trace:
                        case Microsoft.Extensions.Logging.LogLevel.Debug:
                        case Microsoft.Extensions.Logging.LogLevel.Information:
                            _logger.Log(logLevel, 0, null, "HttpRequest Completed");
                            break;
                        case Microsoft.Extensions.Logging.LogLevel.Warning:
                        case Microsoft.Extensions.Logging.LogLevel.Error:
                        case Microsoft.Extensions.Logging.LogLevel.Critical:
                            _logger.Log(logLevel, 0, null, "HttpRequest Failure");
                            break;
                    }
                }
            }

            return false;   // Exception Filter should not suppress the Exception
        }
    }
}
