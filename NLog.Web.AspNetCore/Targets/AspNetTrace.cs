
#if !ASP_NET_CORE
using System.Web;
using NLog.Targets;

namespace NLog.Web.Targets
{
    /// <summary>
    /// Writes log messages to the ASP.NET trace.
    /// </summary>
    /// <seealso href="https://github.com/nlog/nlog/wiki/AspNetTrace-target">Documentation on NLog Wiki</seealso>
    /// <remarks>
    /// Log entries can then be viewed by navigating to http://server/path/Trace.axd.
    /// </remarks>
    [Target("AspNetTrace")]
    public class AspNetTraceTarget : TargetWithLayout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NLog.Targets.TargetWithLayout"/> class.
        /// </summary>
        /// <remarks>
        /// The default value of the layout is: 
        /// <code>
        /// ${longdate}|${level:uppercase=true}|${logger}|${message}
        /// </code>
        /// </remarks>
        public AspNetTraceTarget()
        {
        }

        /// <summary>
        /// Writes the specified logging event to the ASP.NET Trace facility. 
        /// If the log level is greater than or equal to <see cref="LogLevel.Warn"/> it uses the
        /// System.Web.TraceContext.Warn method, otherwise it uses
        /// System.Web.TraceContext.Write method.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
            {
                return;
            }

            if (logEvent.Level >= LogLevel.Warn)
            {
                context.Trace.Warn(logEvent.LoggerName, Layout.Render(logEvent));
            }
            else
            {
                context.Trace.Write(logEvent.LoggerName, Layout.Render(logEvent));
            }
        }
    }
}
#endif