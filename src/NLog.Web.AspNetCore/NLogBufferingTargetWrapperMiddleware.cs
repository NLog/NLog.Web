using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog.Web.Targets.Wrappers;

namespace NLog.Web
{
    /// <summary>
    /// This class is to intercept the HTTP pipeline and to allow the AspNetBufferingTargetWrapper to function properly
    ///
    /// Usage: app.UseMiddleware&lt;NLogBufferingTargetWrapperMiddleware&gt;(); where app is an IApplicationBuilder
    /// </summary>
    /// <seealso href="https://github.com/nlog/nlog/wiki/AspNetBufferingWrapper-target">Documentation on NLog Wiki</seealso>
    public class NLogBufferingTargetWrapperMiddleware
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
                MiddlewareIsInstalled(context);
                AspNetBufferingTargetWrapper.OnBeginRequest(context);

                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                await _next(context).ConfigureAwait(false);
            }
            finally
            {
                AspNetBufferingTargetWrapper.OnEndRequest(context);
            }
        }

        /// <summary>
        /// Signal to the AspNetBufferingTargetWrapper class that the Middleware is installed.
        /// </summary>
        /// <param name="context"></param>
        private static void MiddlewareIsInstalled(HttpContext context)
        {
            if (context != null)
            {
                context.Items[AspNetBufferingTargetWrapper.AspNetBufferingTargetWrapperMiddlewareInstalled] = true;
            }
        }
    }
}
