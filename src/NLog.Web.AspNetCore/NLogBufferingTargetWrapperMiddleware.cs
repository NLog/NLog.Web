using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Web.Targets.Wrappers;

namespace NLog.Web
{
    /// <summary>
    /// This class is to intercept the HTTP pipeline and to allow the AspNetCoreBufferingTargetWrapper to function properly
    ///
    /// Usage: app.UseMiddleware&lt;NLogBufferingTargetWrapperMiddleware&gt;(); where app is an IApplicationBuilder
    /// </summary>
    public class NLogBufferingTargetWrapperMiddleware : AspNetBufferingTargetWrapperEventBase
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes new instance of the <see cref="NLogBufferingTargetWrapperMiddleware"/> class
        /// </summary>
        /// <remarks>
        /// Use the following in Startup.cs:
        /// <code>
        /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        /// {
        ///    app.UseMiddleware&lt;NLog.Web.NLogBufferingTargetWrapperMiddleware&gt;();
        /// }
        /// </code>
        /// </remarks>
        public NLogBufferingTargetWrapperMiddleware(RequestDelegate next)
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
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await _next(context).ConfigureAwait(false);
            }
            finally
            {
                InvokeEndRequestHandler();
            }
        }
    }
}
