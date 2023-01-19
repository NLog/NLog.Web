using System;
using System.Net;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Response Status Code.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-response-statuscode}          emits the http status code as integer</code>
    /// <code>${aspnet-response-statuscode:Format=D} emits the http status code as integer</code>
    /// <code>${aspnet-response-statuscode:Format=F} emits the http status code as enum-string-value</code>
    /// <code>${aspnet-response-statuscode:Format=G} emits the http status code as enum-string-value</code>
    /// <code>${aspnet-response-statuscode:Format=X} emits the http status code as hexadecimal</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetResponse-StatusCode-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-statuscode")]
    public class AspNetResponseStatusCodeRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// A valid enumeration format string, defaults to integer format
        /// </summary>
        /// <remarks>
        /// Supported Values, Case Insensitive
        /// D: outputs the HttpStatusCode enum as a integer
        /// F: outputs the HttpStatusCode enum as a string if possible, otherwise an integer
        /// G: outputs the HttpStatusCode enum as a string if possible, otherwise an integer
        /// X: outputs the HttpStatusCode enum as a hexadecimal
        /// </remarks>
        /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/enumeration-format-strings">Documentation on Enum Format Strings</seealso>

        [DefaultParameter]
        public string Format { get; set; } = "d";

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor.HttpContext.TryGetResponse();
            if (httpResponse == null)
            {
                return;
            }

            var statusCode = httpResponse.StatusCode;
            if (statusCode < 100 || statusCode > 599)
            {
                // Only output valid HTTP status codes
                return;
            }

            try
            {
                builder.Append(((HttpStatusCode)statusCode).ToString(Format));
            }
            catch (Exception ex)
            {
                NLog.Common.InternalLogger.Error(ex, 
                    "Error occurred outputting HttpStatusCode enum ToString() with format specifier of {0} and value of {1}",
                    Format, statusCode);
            }
        }
    }
}
