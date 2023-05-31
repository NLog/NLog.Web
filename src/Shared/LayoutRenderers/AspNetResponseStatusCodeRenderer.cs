using System;
using System.Linq;
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
    /// <code>
    /// ${aspnet-response-statuscode} - Render http status code as integer
    /// ${aspnet-response-statuscode:Format=D} - Render http status code as integer
    /// ${aspnet-response-statuscode:Format=F} - Render http status code as enum-string-value
    /// ${aspnet-response-statuscode:Format=G} - Render http status code as enum-string-value
    /// ${aspnet-response-statuscode:Format=X} - Render http status code as hexadecimal
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetResponse-StatusCode-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-statuscode")]
    public class AspNetResponseStatusCodeRenderer : AspNetLayoutRendererBase
    {
        const int HttpStatusCodeLow = 100;
        const int HttpStatusCodeHigh = 999;

        private string[] FormatMapper => _formatMapper ?? (_formatMapper = Enumerable.Range(0, HttpStatusCodeHigh + 1).Select(s => ((HttpStatusCode)s).ToString(Format)).ToArray());
        private string[] _formatMapper;

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
        public string Format
        {
            get => _format;
            set
            {
                if (_format != value)
                {
                    _format = value;
                    _formatMapper = null;
                }
            }
        }
        private string _format = "d";

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor.HttpContext.TryGetResponse();
            if (httpResponse == null)
            {
                return;
            }

            builder.Append(ConvertToString(httpResponse.StatusCode));
        }

        private string ConvertToString(int httpStatusCode)
        {
            try
            {
                if (httpStatusCode < HttpStatusCodeLow || httpStatusCode > HttpStatusCodeHigh)
                {
                    return string.Empty;    // Only output valid HTTP status codes
                }

                return FormatMapper[httpStatusCode];
            }
            catch (Exception ex)
            {
                NLog.Common.InternalLogger.Warn(ex, "aspnet-response-statuscode - Failed to Format HttpStatusCode={0}", httpStatusCode);
                return null;
            }
        }
    }
}
