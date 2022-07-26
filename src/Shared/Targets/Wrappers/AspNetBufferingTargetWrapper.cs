using System;
using NLog.Common;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System.ComponentModel;
using NLog.Config;
#if !ASP_NET_CORE
using System.Web;
#else
using NLog.Web.DependencyInjection;
using Microsoft.AspNetCore.Http;
#endif

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
    ///       <add name="NLogBufferingTargetWrapperModule" type="NLog.Web.NLogBufferingTargetWrapperModule, NLog.Web"/>
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
        /// <summary>
        /// The key into the HttpContext.Items collection
        /// </summary>
        protected readonly object DataSlot = new object();

        /// <summary>
        /// Limits the amount of slots that the buffer should grow
        /// </summary>
        protected int GrowLimit;

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
        /// Context for DI
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        /// <returns>HttpContextAccessor or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        public IHttpContextAccessor HttpContextAccessor
        {
            get => _httpContextAccessor ?? (_httpContextAccessor = RetrieveHttpContextAccessor(ResolveService<IServiceProvider>(), LoggingConfiguration));
            set => _httpContextAccessor = value;
        }

#if !ASP_NET_CORE
        internal static IHttpContextAccessor DefaultHttpContextAccessor { get; set; } = new DefaultHttpContextAccessor();
        internal static IHttpContextAccessor RetrieveHttpContextAccessor(IServiceProvider serviceProvider, LoggingConfiguration loggingConfiguration) => DefaultHttpContextAccessor;
#else

        internal static IHttpContextAccessor RetrieveHttpContextAccessor(IServiceProvider serviceProvider, LoggingConfiguration loggingConfiguration)
        {
            return ServiceLocator.ResolveService<IHttpContextAccessor>(serviceProvider, loggingConfiguration);
        }
#endif

        private readonly object _lock = new object();

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Internal.LogEventInfoBuffer GetOrCreateRequestBuffer(
#if ASP_NET_CORE
            HttpContext context)
#else
            HttpContextBase context)
#endif
        {
            // Make sure to create the LogEventInfoBuffer only once in multi-threaded situation.
            if (context != null && context.Items[DataSlot] == null)
            {
                lock (_lock)
                {
                    if (context.Items[DataSlot] == null)
                    {
                        context.Items[DataSlot] =
                            new Internal.LogEventInfoBuffer(BufferSize, GrowBufferAsNeeded, BufferGrowLimit);
                    }
                }
            }
            return context?.Items?[DataSlot] as Internal.LogEventInfoBuffer;
        }

        /// <summary>
        /// Adds the specified log event to the buffer.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            var buffer = GetOrCreateRequestBuffer(HttpContextAccessor.HttpContext);
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
        /// Executed by at the end of the HTTP handling by the HttpModule and Middleware
        /// </summary>
        internal void FlushBufferedLogEvents()
        {
            var context = HttpContextAccessor.HttpContext;
            if (context != null)
            {
                var buffer = GetOrCreateRequestBuffer(context);
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
