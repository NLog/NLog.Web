using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using NLog.Common;
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
        /// Defaults to UTF-8
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Defaults to 1024
        /// </summary>
        public int BufferSize { get; set; } = 1024;

        /// <summary>
        /// If this returns true, the post request body will be captured
        /// Defaults to true if content length &lt;= 30KB
        /// This can be used to capture only certain content types,
        /// only certain hosts, only below a certain request body size, and so forth
        /// </summary>
        /// <returns></returns>
        public Predicate<HttpApplication> ShouldCapture { get; set; } = DefaultCapture;

        /// <summary>
        /// The default predicate for ShouldCapture
        /// Returns true if content length &lt;= 30KB
        /// </summary>
        public static bool DefaultCapture(HttpApplication app)
        {
            return app?.Context?.Request?.ContentLength != null && app?.Context?.Request?.ContentLength <= 30 * 1024;
        }

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

            CaptureRequestPostedBody(
                app?.Request?.InputStream,
                app?.Context?.Items,
                ShouldCapture(app));
        }

        /// <summary>
        /// Public to be unit testable, HttpContext and HttpRequest are un-mockable
        /// unless you are using ASP.NET Core.  HttpContext and HttpRequest are sealed
        /// and no not have an interface so NSubstitute throws an Exception mocking them.
        /// </summary>
        /// <param name="bodyStream"></param>
        /// <param name="items"></param>
        /// <param name="shouldCapture"></param>
        public void CaptureRequestPostedBody(
            Stream bodyStream,
            IDictionary items,
            bool shouldCapture)
        {
            if (bodyStream == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.InputStream stream is null");
                return;
            }

            if (!bodyStream.CanRead)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.InputStream stream is non-readable");
                return;
            }

            if (!bodyStream.CanSeek)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpContext.Request.InputStream stream is non-seekable");
                return;
            }

            if (shouldCapture)
            {
                items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = GetString(bodyStream);
            }
            else
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: ShouldCapture(HttpContext) predicate returned false");
            }
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
                       bufferSize: BufferSize,
                       leaveOpen: true))
            {
                // This is the most straight forward logic to read the entire body
                responseText = streamReader.ReadToEnd();
            }

#else
            byte[] byteArray = new byte[stream.Length];

            using (var ms = new MemoryStream())
            {
                int read = 0;

                while ((read = stream.Read(byteArray, 0, byteArray.Length)) > 0)
                {
                    ms.Write(byteArray, 0, read);
                }

                responseText = Encoding.GetString(ms.ToArray());
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
