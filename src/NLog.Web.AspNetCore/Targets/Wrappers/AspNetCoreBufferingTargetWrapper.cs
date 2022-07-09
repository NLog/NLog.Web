using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using NLog.Common;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Web.Internal;

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
    ///    app.UseMiddleware&lt;NLog.Web.NLogBufferingMiddleware&gt;();
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
    public class AspNetCoreBufferingTargetWrapper : WrapperTargetBase
    {
        private readonly object dataSlot = new object();
        private int growLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreBufferingTargetWrapper" /> class.
        /// </summary>
        public AspNetCoreBufferingTargetWrapper()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreBufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        public AspNetCoreBufferingTargetWrapper(Target wrappedTarget)
            : this(wrappedTarget, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreBufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public AspNetCoreBufferingTargetWrapper(Target wrappedTarget, int bufferSize)
        {
            WrappedTarget = wrappedTarget;
            BufferSize = bufferSize;
            GrowBufferAsNeeded = true;
        }

        /// <summary>
        /// Gets or sets the number of log events to be buffered.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(100)]
        public int BufferSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether buffer should grow as needed.
        /// </summary>
        /// <value>A value of <c>true</c> if buffer should grow as needed; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Value of <c>true</c> causes the buffer to expand until <see cref="BufferGrowLimit"/> is hit,
        /// <c>false</c> causes the buffer to never expand and lose the earliest entries in case of overflow.
        /// </remarks>
        /// <docgen category='Buffering Options' order='100' />
        [DefaultValue(false)]
        public bool GrowBufferAsNeeded { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of log events that the buffer can keep.
        /// </summary>
        /// <docgen category='Buffering Options' order='100' />
        public int BufferGrowLimit
        {
            get
            {
                return growLimit;
            }

            set
            {
                growLimit = value;
                GrowBufferAsNeeded = (value >= BufferSize);
            }
        }

        /// <summary>
        /// Initializes the target by hooking up the NLogHttpModule BeginRequest and EndRequest events.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            // Prevent double subscribe
            NLogBufferingMiddleware.BeginRequest -= OnBeginRequest;
            NLogBufferingMiddleware.EndRequest   -= OnEndRequest;

            NLogBufferingMiddleware.BeginRequest += OnBeginRequest;
            NLogBufferingMiddleware.EndRequest   += OnEndRequest;

            var context = new HttpContextAccessor().HttpContext;

            if (context != null)
            {
                // we are in the context already, it's too late for OnBeginRequest to be called, so let's
                // just call it ourselves
                OnBeginRequest(context);
            }
        }

        /// <summary>
        /// Closes the target by flushing pending events in the buffer (if any).
        /// </summary>
        protected override void CloseTarget()
        {
            NLogBufferingMiddleware.BeginRequest -= OnBeginRequest;
            NLogBufferingMiddleware.EndRequest   -= OnEndRequest;
            base.CloseTarget();
        }

        /// <summary>
        /// Adds the specified log event to the buffer.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            // HttpContext is null here, will need a fix
            var buffer = GetRequestBuffer(new HttpContextAccessor().HttpContext);
            if (buffer != null)
            {
                WrappedTarget.PrecalculateVolatileLayouts(logEvent.LogEvent);

                buffer.Append(logEvent);
                InternalLogger.Trace("Appending log event {0} to ASP.NET request buffer.", logEvent.LogEvent.SequenceID);
            }
            else
            {
                InternalLogger.Trace("ASP.NET request buffer does not exist. Passing to wrapped target.");
                WrappedTarget.WriteAsyncLogEvent(logEvent);
            }
        }

        private NLog.Web.Internal.LogEventInfoBuffer GetRequestBuffer(HttpContext context)
        {
            if (context == null)
            {
                return null;
            }

            return context.Items[dataSlot] as NLog.Web.Internal.LogEventInfoBuffer;
        }

        private void OnBeginRequest(object sender, EventArgs args)
        {
            InternalLogger.Trace("Setting up ASP.NET request buffer.");
            HttpContext context = (args as HttpContextEventArgs)?.HttpContext;
            if (context != null)
            {
                context.Items[dataSlot] = new NLog.Web.Internal.LogEventInfoBuffer(BufferSize, GrowBufferAsNeeded, BufferGrowLimit);
            }
            else
            {
                InternalLogger.Error("Unable to setup ASP.NET request buffer, HttpContext is null");
            }
        }

        private void OnBeginRequest(HttpContext context)
        {
            InternalLogger.Trace("Setting up ASP.NET request buffer.");
            if (context != null)
            {
                context.Items[dataSlot] = new NLog.Web.Internal.LogEventInfoBuffer(BufferSize, GrowBufferAsNeeded, BufferGrowLimit);
            }
            else
            {
                InternalLogger.Error("Unable to setup ASP.NET request buffer, HttpContext is null");
            }
        }

        private void OnEndRequest(object sender, EventArgs args)
        {
            HttpContext context = (args as HttpContextEventArgs)?.HttpContext;
            if (context != null)
            {
                var buffer = GetRequestBuffer(context);
                if (buffer != null)
                {
                    InternalLogger.Trace("Sending buffered events to wrapped target: {0}.", WrappedTarget);
                    AsyncLogEventInfo[] events = buffer.GetEventsAndClear();
                    WrappedTarget.WriteAsyncLogEvents(events);
                }
                else
                {
                    InternalLogger.Error("Unable to log buffered ASP.NET events, HttpContext.Items[] request buffer is null");
                }
            }
            else
            {
                InternalLogger.Error("Unable to log buffered ASP.NET events, HttpContext is null");
            }
        }
    }
}