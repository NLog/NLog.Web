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
    /// <code>${aspnet-response-statuscode} emits the http status code as an int</code>
    /// <code>${aspnet-response-statuscode:Format=value} where value is a valid enumeration format string emits the specified format</code>
    /// 'value' is case-insensitive
    /// Supported formats for 'value'
    /// f or g: outputs the HttpStatusCode enum as a string if possible, otherwise an int
    /// d: outputs the HttpStatusCode enum as a int
    /// x: outputs the HttpStatusCode enum as a hexdecimal
    /// See https://learn.microsoft.com/en-us/dotnet/standard/base-types/enumeration-format-strings for more information
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetResponse-StatusCode-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-statuscode")]
    public class AspNetResponseStatusCodeRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// A valid enumeration format string
        /// </summary>
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
