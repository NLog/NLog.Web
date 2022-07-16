using NLog.Common;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System.ComponentModel;
#if !ASP_NET_CORE
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace NLog.Web.Targets.Wrappers
{
    /// <summary>
    /// Base class for both the ASP.NET and ASP.NET Core buffering target wrappers
    /// </summary>
    public abstract class AspNetBufferingTargetWrapperBase : WrapperTargetBase
    {
        /// <summary>
        /// The key into the HttpContext.Items collection
        /// </summary>
        protected readonly object DataSlot = new object();

        /// <summary>
        /// Limits the amount of slots that the buffer should grow
        /// </summary>
        protected int GrowLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetBufferingTargetWrapperBase" /> class.
        /// </summary>
        protected AspNetBufferingTargetWrapperBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetBufferingTargetWrapperBase" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        protected AspNetBufferingTargetWrapperBase(Target wrappedTarget)
            : this(wrappedTarget, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetBufferingTargetWrapperBase" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        protected AspNetBufferingTargetWrapperBase(Target wrappedTarget, int bufferSize)
        {
            WrappedTarget      = wrappedTarget;
            BufferSize         = bufferSize;
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
                return GrowLimit;
            }

            set
            {
                GrowLimit = value;
                GrowBufferAsNeeded = (value >= BufferSize);
            }
        }

        /// <summary>
        /// Accessor for the current HTTP Context
        /// </summary>
        protected IHttpContextAccessor ContextAccessor { get; set; } = new
#if ASP_NET_CORE
        HttpContextAccessor();
#else
        DefaultHttpContextAccessor();
#endif
        /// <summary>
        /// Initializes the target by hooking up the IHttpModule/IMiddleware BeginRequest and EndRequest events.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            // Prevent double subscribe
            AspNetBufferingTargetWrapperEventBase.BeginRequest -= OnBeginRequest;
            AspNetBufferingTargetWrapperEventBase.EndRequest   -= OnEndRequest;

            AspNetBufferingTargetWrapperEventBase.BeginRequest += OnBeginRequest;
            AspNetBufferingTargetWrapperEventBase.EndRequest   += OnEndRequest;

            HandleRequestAlreadyBegun();
        }

        /// <summary>
        /// Additional logic if necessary, if the target is created during an http request
        /// </summary>
        protected virtual void HandleRequestAlreadyBegun()
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Closes the target by flushing pending events in the buffer (if any).
        /// </summary>
        protected override void CloseTarget()
        {
            AspNetBufferingTargetWrapperEventBase.BeginRequest -= OnBeginRequest;
            AspNetBufferingTargetWrapperEventBase.EndRequest   -= OnEndRequest;

            base.CloseTarget();
        }

        /// <summary>
        /// Save the current HttpContext
        /// </summary>
        protected virtual void SaveHttpContext(HttpContextEventArgs httpContextEventArgs)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Adds the specified log event to the buffer.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            var buffer = GetRequestBuffer(ContextAccessor.HttpContext);
            if (buffer != null)
            {
                WrappedTarget?.PrecalculateVolatileLayouts(logEvent.LogEvent);
                InternalLogger.Trace("Appending log event {0} to ASP.NET request buffer.", logEvent.LogEvent.SequenceID);
                buffer.Append(logEvent);
            }
            else
            {
                InternalLogger.Trace("ASP.NET request buffer does not exist. Passing to wrapped target.");
                WrappedTarget?.WriteAsyncLogEvent(logEvent);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Internal.LogEventInfoBuffer GetRequestBuffer(
#if ASP_NET_CORE
            HttpContext context)
#else
            HttpContextBase context)
#endif
        {
            return context?.Items?[DataSlot] as Internal.LogEventInfoBuffer;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnBeginRequest(object sender, HttpContextEventArgs args)
        {
            InternalLogger.Trace("Setting up ASP.NET request buffer.");
            SaveHttpContext(args);
            var context = this.ContextAccessor.HttpContext;
            if (context != null)
            {
                context.Items[DataSlot] = new Internal.LogEventInfoBuffer(BufferSize, GrowBufferAsNeeded, BufferGrowLimit);
            }
            else
            {
                InternalLogger.Error("Unable to setup ASP.NET request buffer, HttpContext is null");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnEndRequest(object sender, HttpContextEventArgs args)
        {
            SaveHttpContext(args);
            var context = this.ContextAccessor.HttpContext;
            if (context != null)
            {
                var buffer = GetRequestBuffer(context);
                if (buffer != null)
                {
                    InternalLogger.Trace("Sending buffered events to wrapped target: {0}.", WrappedTarget);
                    WrappedTarget?.WriteAsyncLogEvents(buffer.GetEventsAndClear());
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
