using NLog.Targets;

namespace NLog.Web.Targets.Wrappers
{
    /// <summary>
    /// Buffers log events for the duration of ASP.NET request and sends them down
    /// to the wrapped target at the end of a request.
    /// </summary>
    /// <seealso href="https://github.com/nlog/nlog/wiki/AspNetCoreBufferingWrapper-target">Documentation on NLog Wiki</seealso>
    /// <remarks>
    /// <p>
    /// Typically this target is used in cooperation with PostFilteringTargetWrapper
    /// to provide verbose logging for failing requests and normal or no logging for
    /// successful requests. We need to make the decision of the final filtering rule
    /// to apply after all logs for a page have been generated.
    /// </p>
    /// <p>
    /// To use this target, you need to add the middleware NLogBufferingMiddleware
    /// </p>
    /// Use the following in Startup.cs:
    /// <code>
    /// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    /// {
    ///    app.UseMiddleware&lt;NLog.Web.NLogBufferingTargetWrapperMiddleware&gt;();
    /// }
    /// </code>
    /// </remarks>
    /// <example>
    /// <p>To set up the ASP.NET Buffering target wrapper <a href="config.html">configuration file</a>, put
    /// the following in <c>web.nlog</c> file in your web application directory (this assumes
    /// that PostFilteringWrapper is used to provide the filtering and actual logs go to
    /// a file).
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/ASPNetCoreBufferingWrapper/web.nlog" />
    /// <p>
    /// This assumes just one target and a single rule. More configuration
    /// options are described <a href="config.html">here</a>.
    /// </p>
    /// </example>
    [Target("AspNetCoreBufferingWrapper", IsWrapper = true)]
    public class AspNetCoreBufferingTargetWrapper : AspNetBufferingTargetWrapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreBufferingTargetWrapper" /> class.
        /// </summary>
        public AspNetCoreBufferingTargetWrapper()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreBufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        public AspNetCoreBufferingTargetWrapper(Target wrappedTarget) :
            base(wrappedTarget)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreBufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public AspNetCoreBufferingTargetWrapper(Target wrappedTarget, int bufferSize) :
            base(wrappedTarget, bufferSize)
        {
        }

        /// <summary>
        /// Returns the HttpContext from the EventArgs
        /// </summary>
        /// <param name="httpContextEventArgs"></param>
        /// <returns></returns>
        protected override void SaveHttpContext(HttpContextEventArgs httpContextEventArgs)
        {
            ContextAccessor.HttpContext = httpContextEventArgs?.HttpContext;
        }
    }
}
