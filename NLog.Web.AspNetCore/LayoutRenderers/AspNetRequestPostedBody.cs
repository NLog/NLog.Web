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

            if (!TryGetBody(httpRequest, out var body))
            {
                return; // No Body to read
            }

            // reset if possible
            if (!TryResetStream(httpRequest, body, out var oldPosition))
            {
                return;
            }

            var content = BodyToString(body);

            //restore
            body.Position = oldPosition;

            builder.Append(content);
        }

        private static string BodyToString(Stream body)
        {
            // Note: don't dispose the StreamReader, it will close the stream and that's unwanted. You could pass that that
            // to the StreamReader in some platforms, but then the dispose will be a NOOP, so for platform compat just don't dispose
            var bodyReader = new StreamReader(body);
            var content = bodyReader.ReadToEnd();
            return content;
        }

        private bool TryResetStream(HttpRequest httpRequest, Stream body, out long oldPosition)
        {
            long? contentLength = httpRequest.ContentLength;
            oldPosition = body.Position;
            if (!body.CanSeek)
            {

#if !ASP_NET_CORE
                InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek");
                return false;
#endif


                if (oldPosition > 0 && oldPosition >= contentLength)
                {
                    InternalLogger.Debug("AspNetRequestPostedBody: body stream cannot seek and already read. StreamPosition={0}", oldPosition);
                    return false;
                }

                oldPosition = 0;
                if (!TryEnableRewind(httpRequest))
                {
                    return false;
                }

                // can seek after buffering?
                if (!body.CanSeek)
                {
                    return false;
                }
            }
            else
            {
                if (MaxContentLength > 0 && !contentLength.HasValue && body.Length > MaxContentLength)
                {
                    InternalLogger.Debug("AspNetRequestPostedBody: body stream too big. Body.Length={0}", body.Length);
                    return false;
                }
            }

            body.Position = 0;
            return true;
        }

        private bool TryGetBody(HttpRequest httpRequest, out Stream body)
        {
            long? contentLength = httpRequest.ContentLength;
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

        private bool TryEnableRewind(HttpRequest httpRequest)
        {
            long? contentLength = httpRequest.ContentLength;
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

            EnableRewind(httpRequest, bufferThreshold);
            return true;
        }

        private static void EnableRewind(HttpRequest httpRequest, int bufferThreshold)
        {
#if ASP_NET_CORE2
            Microsoft.AspNetCore.Http.HttpRequestRewindExtensions.EnableBuffering(httpRequest, bufferThreshold);
#elif ASP_NET_CORE1
            Microsoft.AspNetCore.Http.Internal.BufferingHelper.EnableRewind(httpRequest, bufferThreshold);
#endif
        }
    }
}
