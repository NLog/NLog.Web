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
    /// <code>${aspnet-response-statuscode:Format=d} also emits the http status code as an int</code>
    /// <code>${aspnet-response-statuscode:Format=[anything other than d]} emits the http status code as the enum HttpStatusCode.ToString()</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetResponse-StatusCode-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-statuscode")]
    public class AspNetResponseStatusCodeRenderer : AspNetLayoutRendererBase
    {
        private const string IntegerFormat = "d";

        /// <summary>
        /// If this is 'd', output the int of the http status code
        /// Otherwise, output the Enum.ToString() of the HttpStatusCode.
        /// Defaults to 'd'
        /// </summary>
        [DefaultParameter]
        public string Format { get; set; } = IntegerFormat;

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

            // .NET format strings are case-insensitive
            // See https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
            if (string.Equals(Format, IntegerFormat, StringComparison.InvariantCultureIgnoreCase))
            {
                builder.Append(statusCode);
            }
            else
            {
                builder.Append(((HttpStatusCode)statusCode).ToString());
            }
        }
    }
}
