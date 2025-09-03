using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpContext Items Dictionary Value.
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-httpcontext-item:myKey} - produces "123"
    /// ${aspnet-httpcontext-item:anotherKey} - produces "01/01/2006 00:00:00"
    /// ${aspnet-httpcontext-item:anotherKey:culture=pl-PL} - produces "2006-01-01 00:00:00"
    /// ${aspnet-httpcontext-item:myKey:padding=5} - produces "  123"
    /// ${aspnet-httpcontext-item:myKey:padding=-5} - produces "123  "
    /// ${aspnet-httpcontext-item:stringKey:upperCase=true} - produces "AAA BBB"
    /// </code>
    /// </remarks>
    /// <example>
    /// <para>You can set the value of an ASP.NET Item variable by using the following code:</para>
    /// <code lang="C#">
    /// <![CDATA[
    /// HttpContext.Current.Items["myKey"] = 123;
    /// HttpContext.Current.Items["stringKey"] = "aaa BBB";
    /// HttpContext.Current.Items["anotherKey"] = DateTime.Now;
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-HttpContext-Item-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-httpcontext-item")]
    [LayoutRenderer("aspnet-item")]
    public class AspNetHttpContextItemLayoutRenderer : AspNetLayoutRendererBase
    {
        private readonly NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper _objectPathRenderer = new NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper();

        /// <summary>
        /// Gets or sets the item variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Item { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the object-property-navigation-path for lookup of nested property.
        /// In this case the Item should have have any dot notation, as the nested properties path is in this variable
        /// Example:
        /// Item="person";
        /// ObjectPath="Name.First"
        /// This will emit the First Name property of the object in HttpContext.Items woith the key of 'person' in the collection
        /// </summary>
        /// <docgen category='Layout Options' order='20' />
        public string ObjectPath { get => _objectPathRenderer.ObjectPath; set => _objectPathRenderer.ObjectPath = value; }

        /// <summary>
        /// Gets or sets the item variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [Obsolete("Instead use Item-property. Marked obsolete with NLog.Web 5.3")]
        public string Variable { get => Item; set => Item = value; }

        /// <summary>
        /// Format string for conversion from object to string.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string? Format { get; set; }

        /// <summary>
        /// Gets or sets the culture used for rendering.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public CultureInfo? Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            if (string.IsNullOrEmpty(Item))
                throw new NLogConfigurationException("AspNetItemValue-LayoutRenderer Item-property must be assigned. Lookup blank value not supported.");
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var item = Item;
            if (string.IsNullOrEmpty(item))
                return;

            var httpContext = HttpContextAccessor?.HttpContext;
            if (httpContext is null)
                return;

            var value = LookupItemValue(httpContext.Items, item);
            if (value is null)
                return;

            if (!string.IsNullOrEmpty(ObjectPath))
            {
                if (!_objectPathRenderer.TryGetPropertyValue(value, out value))
                    return;
            }

            var formatProvider = GetFormatProvider(logEvent, Culture);
            builder.AppendFormattedValue(value, Format, formatProvider, ValueFormatter);
        }

#if !ASP_NET_CORE
        private static object? LookupItemValue(System.Collections.IDictionary items, string key)
        {
            return items?.Count > 0 && items.Contains(key) ? items[key] : null;
        }
#else
        private static object? LookupItemValue(IDictionary<object, object?> items, string key)
        {
            return items != null && items.TryGetValue(key, out var itemValue) ? itemValue : null;
        }
#endif
    }
}