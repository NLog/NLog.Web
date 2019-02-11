using System;
using System.IO;
using System.Text;
using NLog.Common;
#if !ASP_NET_CORE
using System.Web;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET posted value, e.g. FORM or Ajax POST
    /// </summary>
    /// <para>Example usage of ${aspnet-request-posted-value}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-posted-value} - Produces - {username:xyz,password:xyz}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-posted-value")]
    [ThreadSafe]
    public class AspNetRequestPostedValue : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the ASP.NET posted value
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
                return;

#if !ASP_NET_CORE

            var body = HttpContext.Current.Request.InputStream;
#else
            var body = httpRequest.Body;
#endif

            if (body == null)
            {
                InternalLogger.Debug("AspNetRequestPostedvalue: body stream was null");
                return;
            }

            long oldPosition = -1;

            // reset if possible
            if (body.CanSeek)
            {
                oldPosition = body.Position;
                body.Position = 0;
            }

            //note: dispose of StreamReader isn't doing things besides closing the stream (which can be turn off, and then it's a NOOP)
            var bodyReader = new StreamReader(body);
            var content = bodyReader.ReadToEnd();

            //restore
            if (body.CanSeek)
            {
                body.Position = oldPosition;
            }

            builder.Append(content);
        }
    }
}
