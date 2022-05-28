using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
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
    ///
    /// To use, subclass this class and set your own Configuration
    /// </summary>
    public class NLogRequestPostedBodyHttpModule : IHttpModule
    {
        /// <summary>
        /// The name of the HttpModule
        /// You may override in the subclass with your own name
        /// </summary>
        public string ModuleName { get; set; } = "NLog Request Posted Body Module";

        /// <summary>
        /// The configuration for the HttpModule
        /// </summary>
        public NLogRequestPostedBodyHttpModuleConfiguration Configuration { get; set; } =
            NLogRequestPostedBodyHttpModuleConfiguration.Default;


        /// <summary>
        /// Hook in to the BeginRequest event to capture the request posted body
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
        }

        /// <summary>
        /// This will forward the necessary arguments to the capture request body method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BeginRequest(object sender, EventArgs args)
        {
            HttpApplication app = sender as HttpApplication;

            if (ShouldCaptureRequestBody(app))
            {
                TryCaptureRequestPostedBody(app?.Request?.InputStream, app?.Context?.Items);
            }
        }

        /// <summary>
        /// The method to decide if we should capture the post request body or not.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private bool ShouldCaptureRequestBody(HttpApplication app)
        {
            // Perform null checking
            if (app == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // Perform null checking
            if (app.Context == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // Perform null checking
            if (app.Context.Request == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext.Request stream is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // Perform request stream specific checks
            if (!ShouldCaptureStream(app.Context.Request.InputStream))
            {
                return false;
            }

            return (Configuration.ShouldCapture(app.Context));
        }

        /// <summary>
        /// Stream specific verifications
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private bool ShouldCaptureStream(Stream stream)
        {
            // Perform null checking
            if (stream == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext.Request.Body stream is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // If we cannot read the stream we cannot capture the body
            if (!stream.CanRead)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext.Request.Body stream is non-readable");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // If we cannot seek the stream we cannot capture the body
            if (!stream.CanSeek)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext.Request.Body stream is non-seekable");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perform the actual capture of the request body
        /// </summary>
        /// <param name="bodyStream"></param>
        /// <param name="items"></param>
        private void TryCaptureRequestPostedBody(Stream bodyStream,IDictionary items)
        {
            var requestBody = GetString(bodyStream);
            if (!string.IsNullOrEmpty(requestBody))
            {
                items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = requestBody;
            }
        }

        /// <summary>
        /// Reads the posted body stream into a string
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string GetString(Stream stream)
        {
            string responseText = null;

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
                       detectEncodingFromByteOrderMarks: Configuration.DetectEncodingFromByteOrderMark,
                       bufferSize: 1024,
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

                responseText = Encoding.UTF8.GetString(ms.ToArray());
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
