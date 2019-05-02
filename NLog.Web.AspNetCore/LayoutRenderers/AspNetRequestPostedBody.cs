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
        private const int Size64KiloBytes = 64 * 1024;
        private const int Size30Kilobytes = 30 * 1024;

        /// <summary>
        /// Max size in bytes of the body. Skip logging of the body when larger.
        /// Default 30720 Bytes = 30 KiB 
        /// (0 = No limit, -1 = No Buffer Limit)
        /// </summary>
        public int MaxContentLength { get; set; } = Size30Kilobytes;

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
            if (!TryGetBody(httpRequest, contentLength, out var body))
            {
                return; // No Body to read
            }

            var content = BodyToString(body);
            builder.Append(content);
        }

        private static string BodyToString(Stream body)
        {
            var oldPosition = body.Position;
            body.Position = 0;
            try
            {
                // Note: don't dispose the StreamReader, it will close the stream and that's unwanted. You could pass that that
                // to the StreamReader in some platforms, but then the dispose will be a NOOP, so for platform compat just don't dispose
                var bodyReader = new StreamReader(body);
                var content = bodyReader.ReadToEnd();
                return content;
            }
            finally
            {
                //restore
                body.Position = oldPosition;
            }
        }

        private bool TryGetBody(HttpRequest httpRequest, long? contentLength, out Stream body)
        {
            body = null;
            if (contentLength <= 0)
            {
                return false;
            }

            if (MaxContentLength > 0 && contentLength > MaxContentLength)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream is too big. ContentLength={0}", contentLength);
                return false;
            }

            body = GetBodyStream(httpRequest);

            if (body == null)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream was null");
                return false;
            }

            if (!body.CanRead)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream has been closed");
                return false;
            }

            if (!body.CanSeek)
            {
                var oldPosition = body.Position;
                if (oldPosition > 0 && oldPosition >= contentLength)
                {
                    InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek and already read. StreamPosition={0}", oldPosition);
                    return false;
                }

                if (!TryEnableBuffering(httpRequest, contentLength, out body))
                    return false;
            }
            else
            {
                if (MaxContentLength > 0 && !contentLength.HasValue && body.Length > MaxContentLength)
                {
                    InternalLogger.Debug("AspNetRequestPostedBody: body stream too big. Body.Length={0}", body.Length);
                    body = null;
                    return false;
                }
            }

            return true;
        }

        private static Stream GetBodyStream(HttpRequest httpRequest)
        {
#if !ASP_NET_CORE
            var body = httpRequest.InputStream;
#else
            var body = httpRequest.Body;
#endif
            return body;
        }

        ///<returns>Can seek now?</returns>
        private bool TryEnableBuffering(HttpRequest httpRequest, long? contentLength, out Stream bodyStream)
        {
            bodyStream = null;

            if (MaxContentLength >= 0 && !contentLength.HasValue)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek with unknown ContentLength");
                return false;
            }

            int bufferThreshold = MaxContentLength <= 0 ? Size64KiloBytes : MaxContentLength;
            if (MaxContentLength == 0 && contentLength > bufferThreshold)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek and stream is too big. ContentLength={0}", contentLength);
                return false;
            }

            bodyStream = EnableRewind(httpRequest, bufferThreshold);
            if (bodyStream?.CanSeek != true)
            {
                InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek");
                return false;
            }

            return true;
        }

        private static Stream EnableRewind(HttpRequest httpRequest, int bufferThreshold)
        {
#if ASP_NET_CORE2
            Microsoft.AspNetCore.Http.HttpRequestRewindExtensions.EnableBuffering(httpRequest, bufferThreshold);
            return httpRequest.Body;
#elif ASP_NET_CORE1
            Microsoft.AspNetCore.Http.Internal.BufferingHelper.EnableRewind(httpRequest, bufferThreshold);
            return httpRequest.Body;
#else
            return null;
#endif
        }
    }
}
