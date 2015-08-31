using System;
using System.Globalization;
using System.Text;
using System.Web;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Application variable.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer to insert the value of the specified variable stored 
    /// in the ASP.NET Application dictionary.
    /// </remarks>
    /// <example>
    /// <para>You can set the value of an ASP.NET Application variable by using the following code:</para>
    /// <code lang="C#">
    /// <![CDATA[
    /// HttpContext.Current.Application["myvariable"] = 123;
    /// HttpContext.Current.Application["stringvariable"] = "aaa BBB";
    /// HttpContext.Current.Application["anothervariable"] = DateTime.Now;
    /// ]]>
    /// </code>
    /// <para>Example usage of ${aspnet-application}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-application:variable=myvariable} - produces "123"
    /// ${aspnet-application:variable=anothervariable} - produces "01/01/2006 00:00:00"
    /// ${aspnet-application:variable=anothervariable:culture=pl-PL} - produces "2006-01-01 00:00:00"
    /// ${aspnet-application:variable=myvariable:padding=5} - produces "  123"
    /// ${aspnet-application:variable=myvariable:padding=-5} - produces "123  "
    /// ${aspnet-application:variable=stringvariable:upperCase=true} - produces "AAA BBB"
    /// </code>
    /// </example>
    [LayoutRenderer("HDC")]
    public class HttpDiagnosticsContextLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Gets or sets the variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Variable { get; set; }

        /// <summary>
        /// Format string for conversion from object to string.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the culture used for rendering. 
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Renders the specified log event context item and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            

            var context = HttpDiagnosticsContext.Current;

            if (context.Contains(Variable))
            {
                builder.Append(context[Variable].ToStringWithOptionalFormat(Format, Culture));
            }
        }
    }
}