using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpContext Application Dictionary Item Value.
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-application:item=myvariable} - produces "123"
    /// ${aspnet-application:item=anothervariable} - produces "01/01/2006 00:00:00"
    /// ${aspnet-application:item=anothervariable:culture=pl-PL} - produces "2006-01-01 00:00:00"
    /// ${aspnet-application:item=myvariable:padding=5} - produces "  123"
    /// ${aspnet-application:item=myvariable:padding=-5} - produces "123  "
    /// ${aspnet-application:item=stringvariable:upperCase=true} - produces "AAA BBB"
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
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetApplication-layout-renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-application")]
    public class AspNetApplicationValueLayoutRenderer : AspNetLayoutRendererBase
    {
        private readonly NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper _objectPathRenderer = new NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper();

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
        [Obsolete("Instead use Item-property. Marked obsolete with NLog.Web 5.3")]
        public string Variable { get => Item; set => Item = value; }

        /// <summary>
        /// Gets or sets the object-property-navigation-path for lookup of nested property
        /// </summary>
        public string ObjectPath { get => _objectPathRenderer.ObjectPath; set => _objectPathRenderer.ObjectPath = value; }

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
            if (item is null)
            {
                return;
            }

            var application = HttpContextAccessor.HttpContext.Application;
            if (application is null)
            {
                return;
            }

            var value = application[item];
            if (value is null)
            {
                return;
            }

            if (!(ObjectPath is null))
            {
                if (!_objectPathRenderer.TryGetPropertyValue(value, out value))
                    return;

                if (value is null)
                    return;
            }

            var formatProvider = GetFormatProvider(logEvent, Culture);
            builder.AppendFormattedValue(value, Format, formatProvider, ValueFormatter);
        }
    }
}