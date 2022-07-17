using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpContext Application Dictionary Variable.
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-application:variable=myvariable} - produces "123"
    /// ${aspnet-application:variable=anothervariable} - produces "01/01/2006 00:00:00"
    /// ${aspnet-application:variable=anothervariable:culture=pl-PL} - produces "2006-01-01 00:00:00"
    /// ${aspnet-application:variable=myvariable:padding=5} - produces "  123"
    /// ${aspnet-application:variable=myvariable:padding=-5} - produces "123  "
    /// ${aspnet-application:variable=stringvariable:upperCase=true} - produces "AAA BBB"
    /// </code>
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
    /// </example>
    [LayoutRenderer("aspnet-application")]
    public class AspNetApplicationValueLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets the item variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        /// Gets or sets the variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Variable { get => Item; set => Item = value; }

        /// <summary>
        /// Format string for conversion from object to string.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the culture used for rendering.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var item = Item;
            if (item == null)
            {
                return;
            }

            var application = HttpContextAccessor.HttpContext.Application;
            if (application == null)
            {
                return;
            }

            var formatProvider = GetFormatProvider(logEvent, Culture);
            builder.AppendFormattedValue(application[item], Format, formatProvider, ValueFormatter);
        }
    }
}