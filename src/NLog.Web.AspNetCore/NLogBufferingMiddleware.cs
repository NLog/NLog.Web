using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Web.Targets.Wrappers;

namespace NLog.Web
{
    /// <summary>
    /// This class is to intercept the HTTP pipeline and to allow additional logging of the following
    ///
    /// POST request body
    ///
    /// Usage: app.UseMiddleware&lt;NLogBufferingMiddleware&gt;(); where app is an IApplicationBuilder
    /// </summary>
    public class NLogBufferingMiddleware
    {
        /// <summary>
        /// Event to be raised at the end of each HTTP Request.
        /// </summary>
        public static event EventHandler EndRequest;

        /// <summary>
        /// Event to be raised at the beginning of each HTTP Request.
        /// </summary>
        public static event EventHandler BeginRequest;

        private readonly RequestDelegate _next;

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
        public NLogBufferingMiddleware(RequestDelegate next)
        {
            _next = next;
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
                BeginRequest(null, new HttpContextEventArgs(context));
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await _next(context).ConfigureAwait(false);
            }
            finally
            {
                EndRequest(null, new HttpContextEventArgs(context));
            }
        }
    }
}
