using System;
using System.Collections.Generic;
using System.ComponentModel;
#if !ASP_NET_CORE
using System.Web;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
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
        private int growLimit;

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

        /// <summary>
        /// Adds the specified log event to the buffer.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            var buffer = GetRequestBuffer();
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

        private NLog.Web.Internal.LogEventInfoBuffer GetRequestBuffer()
        {
            var context = HttpContextAccessor?.HttpContext;
            if (context == null)
            {
                return null;
            }

            var bufferDictionary = GetBufferDictionary(context);
            if (bufferDictionary == null)
            {
                return null;
            }

            lock (bufferDictionary)
            {
                if (!bufferDictionary.TryGetValue(this, out var buffer))
                {
                    buffer = new Internal.LogEventInfoBuffer(BufferSize, GrowBufferAsNeeded, BufferGrowLimit);
                    bufferDictionary.Add(this, buffer);
                }

                return buffer;
            }
        }

        private static Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer> GetBufferDictionary(HttpContextBase context)
        {
            return context?.Items?[dataSlot] as
                Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>;
        }

        private static void SetBufferDictionary(HttpContextBase context)
        {
            context.Items[dataSlot] = new Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>();
        }

        internal static void OnBeginRequest(HttpContextBase context)
        {
            if (context == null)
            {
                return;
            }

            var bufferDictionary = GetBufferDictionary(context);
            if (bufferDictionary == null)
            {
                InternalLogger.Trace("Setting up ASP.NET request buffer.");
                SetBufferDictionary(context);
            }
        }

        internal static void OnEndRequest(HttpContextBase context)
        {
            var bufferDictionary = GetBufferDictionary(context);
            if (bufferDictionary == null)
            {
                return;
            }

            foreach (var bufferKeyValuePair in bufferDictionary)
            {
                var wrappedTarget = bufferKeyValuePair.Key.WrappedTarget;
                var buffer = bufferKeyValuePair.Value;
                if (buffer != null)
                {
                    InternalLogger.Trace("Sending buffered events to wrapped target: {0}.", wrappedTarget);
                    AsyncLogEventInfo[] events = buffer.GetEventsAndClear();
                    wrappedTarget.WriteAsyncLogEvents(events);
                }
            }
        }
    }
}