using System;
using System.IO;
using System.Text;
using System.Web;
using NLog.Common;
using NLog.Web.LayoutRenderers;

namespace NLog.Web
{
    /// <summary>
    /// HttpModule that enables ${aspnet-request-posted-body}
    /// </summary>
    public class NLogRequestPostedBodyModule : IHttpModule
    {
        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += (sender, args) => OnBeginRequest((sender as HttpApplication)?.Context);
        }

        internal void OnBeginRequest(HttpContext context)
        {
            if (ShouldCaptureRequestBody(context))
            {
                var requestBody = GetString(context.Request.InputStream);

                if (!string.IsNullOrEmpty(requestBody))
                {
                    context.Items[AspNetRequestPostedBodyLayoutRenderer.NLogPostedRequestBodyKey] = requestBody;
                }
            }
        }

        private bool ShouldCaptureRequestBody(HttpContext context)
        {
            // Perform null checking
            if (context == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyModule: HttpContext is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // Perform null checking
            if (context.Request == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyModule: HttpContext.Request stream is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            var stream = context.Request.InputStream;
            if (stream == null)
            {
                InternalLogger.Debug("NLogRequestPostedBodyModule: HttpContext.Request.Body stream is null");
                // Execute the next class in the HTTP pipeline, this can be the next middleware or the actual handler
                return false;
            }

            // If we cannot read the stream we cannot capture the body
            if (!stream.CanRead)
            {
                InternalLogger.Debug("NLogRequestPostedBodyModule: HttpContext.Request.Body stream is non-readable");
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
        /// Reads the posted body stream into a string
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private string GetString(Stream stream)
        {
            string responseText = null;

            if (stream.Length == 0)
                return responseText;

            // Save away the original stream position
            var originalPosition = stream.Position;

            try
            {
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
                           true,
                           bufferSize: 1024,
                           leaveOpen: true))
                {
                    // This is the most straight forward logic to read the entire body
                    responseText = streamReader.ReadToEnd();
                }

#else
                byte[] byteArray = new byte[Math.Min(stream.Length, 1024)];

                using (var ms = new MemoryStream())
                {
                    int read = 0;

                    while ((read = stream.Read(byteArray, 0, byteArray.Length)) > 0)
                    {
                        ms.Write(byteArray, 0, read);
                    }

                    responseText = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                }
#endif
            }
            finally
            {
                // This is required to reset the stream position to the original, in order to
                // properly let the next reader process the stream from the original point
                stream.Position = originalPosition;
            }

            // Return the string of the body
            return responseText;
        }

        void IHttpModule.Dispose()
        {
            // Nothing here to do
        }
    }
}
