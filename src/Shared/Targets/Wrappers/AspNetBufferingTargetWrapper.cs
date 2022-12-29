using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
#if !ASP_NET_CORE
using System.Web;
#else
using Microsoft.AspNetCore.Http;
using NLog.Web.DependencyInjection;
#endif
using NLog.Common;
using NLog.Targets;
using NLog.Targets.Wrappers;


namespace NLog.Web.Targets.Wrappers
{
    /// <summary>
    /// Buffers log events for the duration of ASP.NET request and sends them down 
    /// to the wrapped target at the end of a request.
    /// </summary>
    /// <seealso href="https://github.com/nlog/nlog/wiki/AspNetBufferingWrapper-target">Documentation on NLog Wiki</seealso>
    /// <remarks>
    /// <p>
    /// Typically this target is used in cooperation with PostFilteringTargetWrapper
    /// to provide verbose logging for failing requests and normal or no logging for
    /// successful requests. We need to make the decision of the final filtering rule
    /// to apply after all logs for a page have been generated.
    /// </p>
    /// <p>
    /// To use this target, for classic ASP.NET you need to add an entry in the httpModules section of
    /// web.config:
    /// </p>
    /// <code lang="XML">
    /// <![CDATA[<?xml version="1.0" ?>
    /// <configuration>
    ///   <system.web>
    ///     <httpModules>
    ///       <add name="NLog" type="NLog.Web.NLogHttpModule, NLog.Web"/>
    ///     </httpModules>
    ///   </system.web>
    /// </configuration>
    /// ]]>
    /// </code>
    /// to use this target, for ASP.NET Core, you need to add a line fo code to involve the proper middleware
    /// <code>
    /// <![CDATA[
    ///    app.UseMiddleware<NLogBufferingTargetWrapperMiddleware>();
    /// ]]>
    /// </code>
    /// </remarks>
    /// <example>
    /// <p>To set up the ASP.NET Buffering target wrapper <a href="config.html">configuration file</a>, put
    /// the following in <c>web.nlog</c> file in your web application directory (this assumes
    /// that PostFilteringWrapper is used to provide the filtering and actual logs go to
    /// a file).
    /// </p>
    /// <code lang="XML" source="examples/targets/Configuration File/ASPNetBufferingWrapper/web.nlog" />
    /// <p>
    /// This assumes just one target and a single rule. More configuration
    /// options are described <a href="config.html">here</a>.
    /// </p>
    /// <p>
    /// To configure the target programmatically, put the following
    /// piece of code in your <c>Application_OnStart()</c> handler in Global.asax.cs 
    /// or some other place that gets executed at the very beginning of your code:
    /// </p>
    /// <code lang="C#" source="examples/targets/Configuration API/ASPNetBufferingWrapper/Global.asax.cs" />
    /// <p>
    /// Fully working C# project can be found in the <c>Examples/Targets/Configuration API/ASPNetBufferingWrapper</c>
    /// directory along with usage instructions.
    /// </p>
    /// </example>
    [Target("AspNetBufferingWrapper", IsWrapper = true)]
    public class AspNetBufferingTargetWrapper : WrapperTargetBase
    {
        private static readonly object dataSlot = new object();
        private int _bufferGrowLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetBufferingTargetWrapper" /> class.
        /// </summary>
        public AspNetBufferingTargetWrapper()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetBufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        public AspNetBufferingTargetWrapper(Target wrappedTarget)
            : this(wrappedTarget, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetBufferingTargetWrapper" /> class.
        /// </summary>
        /// <param name="wrappedTarget">The wrapped target.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public AspNetBufferingTargetWrapper(Target wrappedTarget, int bufferSize)
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
            get => _bufferGrowLimit;
            set
            {
                _bufferGrowLimit = value;
                GrowBufferAsNeeded = (value >= BufferSize);
            }
        }

        internal IHttpContextAccessor HttpContextAccessor
        {
            get => _httpContextAccessor ?? (_httpContextAccessor = RetrieveHttpContextAccessor());
            set => _httpContextAccessor = value;
        }
        private IHttpContextAccessor _httpContextAccessor;

        private IHttpContextAccessor RetrieveHttpContextAccessor()
        {
#if ASP_NET_CORE
            return ServiceLocator.ResolveService<IHttpContextAccessor>(ResolveService<IServiceProvider>(), LoggingConfiguration);
#else
            return new DefaultHttpContextAccessor();
#endif
        }

        /// <inheritdoc/>
        protected override void InitializeTarget()
        {
            var httpContextAccessor = HttpContextAccessor;  // Best to resolve HttpContext before starting logging
            if (httpContextAccessor is null)
                InternalLogger.Debug("{0}: HttpContextAccessor not available", this);

#if !ASP_NET_CORE
            // Prevent double subscribe
            NLogHttpModule.BeginRequest -= OnBeginRequestHandler;
            NLogHttpModule.EndRequest -= OnEndRequestHandler;

            NLogHttpModule.BeginRequest += OnBeginRequestHandler;
            NLogHttpModule.EndRequest += OnEndRequestHandler;
#endif
            
            base.InitializeTarget();
        }

        /// <inheritdoc/>
        protected override void CloseTarget()
        {
#if !ASP_NET_CORE
            NLogHttpModule.BeginRequest -= OnBeginRequestHandler;
            NLogHttpModule.EndRequest -= OnEndRequestHandler;
#endif

            base.CloseTarget();
        }

        /// <summary>
        /// Adds the specified log event to the buffer.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent)
        {
            var buffer = GetRequestBuffer();
            if (buffer != null)
            {
                InternalLogger.Trace("{0}: Append to active Request buffer", this);
                WrappedTarget?.PrecalculateVolatileLayouts(logEvent.LogEvent);
                buffer.Append(logEvent);
            }
            else
            {
                InternalLogger.Trace("{0}: Request buffer not active, writing to wrapped target.", this);
                WrappedTarget?.WriteAsyncLogEvent(logEvent);
            }
        }

        private NLog.Web.Internal.LogEventInfoBuffer GetRequestBuffer()
        {
            try
            {
#if ASP_NET_CORE
                var context = HttpContextAccessor?.HttpContext;
#else
                var context = HttpContext.Current;
#endif
                if (context == null)
                {
                    return null;
                }

                var targetBufferList = GetTargetBufferList(context);
                return targetBufferList?.GetRequestBuffer(this);
            }
            catch (Exception ex)
            {
                if (LogManager.ThrowExceptions)
                    throw;

                InternalLogger.Error(ex, "{0}: Failed to retrieve Request Buffer", this);
                return null;
            }
        }

        private static TargetBufferListNode GetTargetBufferList(HttpContext context)
        {
            return context?.Items?[dataSlot] as TargetBufferListNode;
        }

        private static void SetTargetBufferList(HttpContext context, TargetBufferListNode newEmptyList)
        {
            context.Items[dataSlot] = newEmptyList;
        }

        private sealed class TargetBufferListNode
        {
            public AspNetBufferingTargetWrapper Target => _target;
            public Internal.LogEventInfoBuffer RequestBuffer => _requestBuffer;
            public TargetBufferListNode NextNode => _nextNode;

            private AspNetBufferingTargetWrapper _target;
            private Internal.LogEventInfoBuffer _requestBuffer;
            private TargetBufferListNode _nextNode;

            public Internal.LogEventInfoBuffer GetRequestBuffer(AspNetBufferingTargetWrapper target)
            {
                if (NodeForTarget(target))
                {
                    var requestBuffer = _requestBuffer;
                    if (requestBuffer is null)
                    {
                        var buffer = new Internal.LogEventInfoBuffer(target.BufferSize, target.GrowBufferAsNeeded, target.BufferGrowLimit);
                        requestBuffer = Interlocked.CompareExchange(ref _requestBuffer, buffer, null) ?? buffer;
                    }
                    return requestBuffer;
                }
                else
                {
                    var nextNode = _nextNode;
                    if (nextNode is null)
                    {
                        nextNode = new TargetBufferListNode();
                        nextNode = Interlocked.CompareExchange(ref _nextNode, nextNode, null) ?? nextNode;
                    }
                    return nextNode.GetRequestBuffer(target);
                }
            }

            private bool NodeForTarget(AspNetBufferingTargetWrapper target)
            {
                if (_target is null && Interlocked.CompareExchange(ref _target, target, null) == null)
                    return true;

                return ReferenceEquals(_target, target);
            }
        }

        internal static void OnBeginRequest(HttpContext context)
        {
            try
            {
                if (context == null)
                {
                    return;
                }

                var targetBufferList = GetTargetBufferList(context);
                if (targetBufferList is null)
                {
                    InternalLogger.Trace("AspNetBufferingWrapper Request Buffer Initializing.");
                    SetTargetBufferList(context, new TargetBufferListNode());
                }
            }
            catch (Exception ex)
            {
                if (LogManager.ThrowExceptions)
                    throw;

                InternalLogger.Error(ex, "AspNetBufferingWrapper Failed to initialize Request Buffer");
            }
        }

        internal static void OnEndRequest(HttpContext context)
        {
            try
            {
                var targetBufferList = GetTargetBufferList(context);
                if (targetBufferList is null)
                    return;

                while (targetBufferList?.Target != null)
                {
                    var target = targetBufferList.Target;
                    var events = targetBufferList.RequestBuffer?.GetEventsAndClear();
                    if (events?.Length > 0)
                    {
                        InternalLogger.Trace("{0}: Request Buffer (Cnt={1}) flushed to wrapped target", target, events.Length);
                        target.WrappedTarget?.WriteAsyncLogEvents(events);
                    }

                    targetBufferList = targetBufferList.NextNode;
                }

                SetTargetBufferList(context, null); // Disable buffering
            }
            catch (Exception ex)
            {
                if (LogManager.ThrowExceptions)
                    throw;

                InternalLogger.Error(ex, "AspNetBufferingWrapper Failed to flush Request Buffer");
            }
        }

#if !ASP_NET_CORE
        private void OnBeginRequestHandler(object sender, EventArgs e)
        {
            OnBeginRequest(HttpContext.Current);
        }

        private void OnEndRequestHandler(object sender, EventArgs e)
        {
            OnEndRequest(HttpContext.Current);
        }
#endif
    }
}