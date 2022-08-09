using System;
using System.Collections.Generic;
using System.ComponentModel;
using NLog.Common;
using NLog.Targets;
using NLog.Targets.Wrappers;
#if !ASP_NET_CORE
using System.Web;
#else
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Http;
using NLog.Web.DependencyInjection;
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
        /// Limits the amount of slots that the buffer should grow
        /// </summary>
        private int _bufferGrowLimit;

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

        /// <summary>
        /// Context for DI
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        /// <returns>HttpContextAccessor or <c>null</c></returns>
        internal IHttpContextAccessor HttpContextAccessor
        {
            get => _httpContextAccessor ?? (_httpContextAccessor = RetrieveHttpContextAccessor());
            set => _httpContextAccessor = value;
        }

        private IHttpContextAccessor RetrieveHttpContextAccessor()
        {
#if ASP_NET_CORE
            return ServiceLocator.ResolveService<IHttpContextAccessor>(ResolveService<IServiceProvider>(), LoggingConfiguration);
#else
            return new DefaultHttpContextAccessor();
#endif
        }

        /// <summary>
        /// The Key for the buffer dictionary in the HttpContext.Items collection
        /// </summary>
        private static readonly object HttpContextItemsKey = new object();

        /// <summary>
        /// Must be called by the HttpModule/Middleware upon starting.
        /// This creates the dictionary in the HttpContext.Items with the proper key.
        /// </summary>
        /// <param name="context"></param>
        internal static void Initialize(HttpContextBase context)
        {
            if (context == null)
            {
                return;
            }

            var bufferDictionary = GetBufferDictionary(context);

            // If the dictionary is missing, create that first
            if (bufferDictionary == null)
            {
                SetBufferDictionary(context);
            }
        }

        /// <summary>
        /// Obtains a slot in the buffer dictionary for 'this' class instance
        /// If that does not exist, that is created first
        /// if the dictionary does not exist, that is created first
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Internal.LogEventInfoBuffer GetOrCreateRequestBuffer(HttpContextBase context) 
        {
            // If the context is missing, stop
            if (context == null)
            {
                return null;
            }

            var bufferDictionary = GetBufferDictionary(context);

            // If the dictionary is missing, stop
            if (bufferDictionary == null)
            {
                return null;
            }

            // if the slot for this class instance is missing, create that first
            if (bufferDictionary.ContainsKey(this))
            {
                return bufferDictionary[this];
            }

            lock (bufferDictionary)
            {
                if (!bufferDictionary.ContainsKey(this))
                {
                    bufferDictionary.Add(this,
                        new Internal.LogEventInfoBuffer(BufferSize, GrowBufferAsNeeded, BufferGrowLimit));
                }
            }

            return bufferDictionary[this];
        }

        /// <summary>
        /// Return the buffer dictionary from the HttpContext.Items using HttpContextItemsKey
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer> GetBufferDictionary(HttpContextBase context)
        {
            return context?.Items?[HttpContextItemsKey] as
                Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>;
        }

        /// <summary>
        /// Create the buffer dictionary in the HttpContext.Items using HttpContextItemsKey
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static void SetBufferDictionary(HttpContextBase context)
        {
            context.Items[HttpContextItemsKey] = new Dictionary<AspNetBufferingTargetWrapper, Internal.LogEventInfoBuffer>();
        }

        /// <summary>
        /// Adds the specified log event to the buffer.
        /// NOTE: if Write is never called, this instance will not be registered in the buffer dictionary in HttpContext.Items.
        /// That is expected normal behavior.
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
        /// Called from the HttpModule or Middleware upon the end of the HTTP pipeline
        /// Flushes all instances of this class registered in the HttpContext
        /// </summary>
        /// <param name="context"></param>
        internal static void Flush(HttpContextBase context)
        {
            var bufferDictionary = GetBufferDictionary(context);
            if (bufferDictionary == null)
            {
                return;
            }
            foreach(var bufferKeyValuePair in bufferDictionary)
            {
                bufferKeyValuePair.Key?.Flush(bufferKeyValuePair.Value);
            }
        }

        /// <summary>
        /// Called by the above static Flush method.
        /// </summary>
        /// <param name="buffer"></param>
        private void Flush(Internal.LogEventInfoBuffer buffer)
        {
            if (buffer == null)
            {
                return;
            }
            InternalLogger.Trace("Sending buffered events to wrapped target: {0}.", WrappedTarget);
            WrappedTarget?.WriteAsyncLogEvents(buffer.GetEventsAndClear());
        }
    }
}
