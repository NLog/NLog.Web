using System;
using System.Collections;
using System.IO;
#if NET46_OR_GREATER
using System.Text;
#endif
using System.Web;
using NLog.Web.LayoutRenderers;

namespace NLog.Web
{
    /// <summary>
    /// This class is to intercept the HTTP pipeline and to allow additional logging of the following
    ///
    /// POST request body
    ///
    /// The following are saved in the HttpContext.Items collection
    ///
    /// __nlog-aspnet-request-posted-body
    /// </summary>
    public class NLogRequestPostedBodyHttpModule : IHttpModule
    {
        /// <summary>
        /// The name of the HttpModule
        /// </summary>
        public string ModuleName => "NLog Request Posted Body Module";

        /// <summary>
        /// Hook in to the BeginRequest event to capture the request posted body
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += InterceptRequest;
        }

        /// <summary>
        /// This will forward the necessary arguments to the capture request body method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void InterceptRequest(object sender, EventArgs args)
        {
            HttpApplication app = sender as HttpApplication;

            CaptureRequestPostedBody(app?.Request?.InputStream, app?.Request?.ContentLength, app?.Context?.Items);
        }

        /// <summary>
        /// This will capture the requested posted body if &lt;= 8192 bytes
        /// Public to be unit testable, HttpContext and HttpRequest are un-mockable
        /// unless you are using ASP.NET Core.  HttpContext and HttpRequest are sealed
        /// and no not have an interface so NSubstitute throws an Exception mocking them.
        /// </summary>
        /// <param name="bodyStream"></param>
        /// <param name="contentLength"></param>
        /// <param name="items"></param>
        public void CaptureRequestPostedBody(Stream bodyStream, int? contentLength, IDictionary items)
        {
            if (bodyStream == null)
            {
                return;
            }

            if (!bodyStream.CanRead)
            {
                return;
            }

            if (!bodyStream.CanSeek)
            {
                return;
            }

            if (contentLength == null || contentLength > 30 * 1024)
            {
                return;
            }

            items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = GetString(bodyStream);
        }

        /// <summary>
        /// Reads the posted body stream into a string
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected string GetString(Stream stream)
        {
            string responseText = string.Empty;

            // Save away the original stream position
            var originalPosition = stream.Position;

            // This is required to reset the stream position to the beginning in order to properly read all of the stream.
            stream.Position = 0;

#if NET46_OR_GREATER

            //This required 5 argument constructor with leaveOpen available was added in 4.5,
            //but the project and its unit test project are built for 4.6
            //and we should not change the csproj file just for this single class

            //If the 4 argument constructor with leaveOpen missing is used, the stream is closed after the
            //ReadToEnd() operation completes and the request stream is no longer open for the actual consumer
            //This causes the unit test to fail and should cause a failure during actual usage.

            using (var streamReader = new StreamReader(
                       stream,
                       Encoding.UTF8,
                       detectEncodingFromByteOrderMarks: true,
                       bufferSize: 1024,
                       leaveOpen: true))
            {
                // This is the most straight forward logic to read the entire body
                responseText = streamReader.ReadToEnd();
            }
#endif
            // This is required to reset the stream position to the original, in order to
            // properly let the next reader process the stream from the original point
            stream.Position = originalPosition;

            // Return the string of the body
            return responseText;
        }

        /// <summary>
        /// This is a no-op
        /// </summary>
        public void Dispose()
        {
            // Nothing to be disposed of
        }
    }
}
