using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
                if (statusCode < 100 || (statusCode >= 400 && statusCode < 600))
                {
                    _logger.LogWarning("HttpRequest Failed");
                }
                else if (IsExcludedHttpRequest(httpContext))
                {
                    _logger.LogDebug("HttpRequest Completed");
                }
                else if (IsSlowHttpRequest())
                {
                    _logger.LogWarning("HttpRequest Slow");
                }
                else
                {
                    _logger.LogInformation("HttpRequest Completed");
                }
            }

            return false;   // Exception Filter should not suppress the Exception
        }

        private bool IsSlowHttpRequest()
        {
#if !ASP_NET_CORE2
            var currentActivity = System.Diagnostics.Activity.Current;
            var activityStartTime = DateTime.MinValue;
            while (currentActivity != null)
            {
                if (currentActivity.StartTimeUtc > DateTime.MinValue)
                    activityStartTime = currentActivity.StartTimeUtc;
                currentActivity = currentActivity.Parent;
            }
            if (activityStartTime > DateTime.MinValue)
            {
                var currentDuration = DateTime.UtcNow - activityStartTime;
                if (currentDuration > TimeSpan.FromMilliseconds(_options.DurationThresholdMs))
                {
                    return true;
                }
            }
#endif

            return false;
        }

        private bool IsExcludedHttpRequest(HttpContext httpContext)
        {
            if (_options.ExcludeRequestPaths.Count > 0)
            {
                var requestPath = httpContext.Features.Get<IHttpRequestFeature>()?.Path;
                if (string.IsNullOrEmpty(requestPath))
                {
                    requestPath = httpContext.Request?.Path;
                    if (string.IsNullOrEmpty(requestPath))
                    {
                        return false;
                    }
                }

                return _options.ExcludeRequestPaths.Contains(requestPath);
            }

            return false;
        }
    }
}
