using NLog.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Web.Internal
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Convert the stream to a String for logging.
        /// If the stream is binary please do not utilize this middleware
        /// Arguably, logging a byte array in a sensible format is simply not possible.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>The contents of the Stream read fully from start to end as a String</returns>
        internal static async Task<string> GetString(this Stream stream)
        {
            string responseText = null;

            // If we cannot seek the stream we cannot capture the body
            if (!stream.CanSeek)
            {
                InternalLogger.Debug("NLogRequestPostedBodyMiddleware: HttpApplication.HttpContext Body stream is non-seekable");
                return responseText;
            }

            // Save away the original stream position
            var originalPosition = stream.Position;

            try
            {
                // This is required to reset the stream position to the beginning in order to properly read all of the stream.
                stream.Position = 0;

                // The last argument, leaveOpen, is set to true, so that the stream is not pre-maturely closed
                // therefore preventing the next reader from reading the stream.
                using (var streamReader = new StreamReader(
                           stream,
                           Encoding.UTF8,
                           true,
                           1024,
                           leaveOpen: true))
                {
                    // This is the most straight forward logic to read the entire body
                    responseText = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                }
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
    }
}
