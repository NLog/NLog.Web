using System;
using System.IO;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

#if !ASP_NET_CORE
using System.Web;
using HttpRequest = System.Web.HttpRequestBase;
#else
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET posted body, e.g. FORM or Ajax POST
    /// </summary>
    /// <para>Example usage of ${aspnet-request-posted-body}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-posted-body} - Produces - {username:xyz,password:xyz}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-posted-body")]
    [ThreadSafe]
    public class AspNetRequestPostedBody : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Skip logging of HttpRequest.Body when ContentLength is larger than this (0 = No limit, -1 = No Buffer Limit)
        /// </summary>
        public int MaxContentLength { get; set; } = 30 * 1024;

        /// <summary>
        /// Renders the ASP.NET posted body
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            long? contentLength = httpRequest.ContentLength;
            if (contentLength <= 0)
            {
                return; // No Body to read
            }

            if (MaxContentLength > 0 && contentLength > MaxContentLength)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream is too big. ContentLength={0}", contentLength);
                return; // Body too large
            }

#if !ASP_NET_CORE
            var body = httpRequest.InputStream;
#else
            var body = httpRequest.Body;
#endif

            if (body == null)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream was null");
                return;
            }

            if (!body.CanRead)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream has been closed");
                return;
            }

            // reset if possible
            long oldPosition = body.Position;
            if (!body.CanSeek)
            {
                if (oldPosition > 0 && oldPosition >= contentLength)
                {
                    InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek and already read. StreamPosition={0}", oldPosition);
                    return;
                }

                oldPosition = 0;
                if (!TryEnableBuffering(httpRequest, contentLength, out body))
                {
                    return; // Body cannot be buffered
                }
            }
            else
            {
                if (MaxContentLength > 0 && !contentLength.HasValue && body.Length > MaxContentLength)
                {
                    InternalLogger.Debug("AspNetRequestPostedBody: body stream too big. Body.Length={0}", body.Length);
                    return; // Body too large
                }
            }

            body.Position = 0;

            //note: dispose of StreamReader isn't doing things besides closing the stream (which can be turn off, and then it's a NOOP)
            var bodyReader = new StreamReader(body);
            var content = bodyReader.ReadToEnd();

            //restore
            body.Position = oldPosition;

            builder.Append(content);
        }

        bool TryEnableBuffering(HttpRequest httpRequest, long? contentLength, out Stream bodyStream)
        {
            bodyStream = null;

            if (MaxContentLength >= 0 && !contentLength.HasValue)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek with unknown ContentLength");
                return false;
            }

            int bufferThreshold = MaxContentLength <= 0 ? 64 * 1024 : MaxContentLength;
            if (MaxContentLength == 0 && contentLength > bufferThreshold)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek and stream is too big. ContentLength={0}", contentLength);
                return false;
            }

#if ASP_NET_CORE2
            Microsoft.AspNetCore.Http.HttpRequestRewindExtensions.EnableBuffering(httpRequest, bufferThreshold);
            bodyStream = httpRequest.Body;
            return bodyStream?.CanSeek == true;
#elif ASP_NET_CORE1
            Microsoft.AspNetCore.Http.Internal.BufferingHelper.EnableRewind(httpRequest, bufferThreshold);
            bodyStream = httpRequest.Body;
            return bodyStream?.CanSeek == true;
#else
            InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek");
            return false;
#endif
        }
    }
}