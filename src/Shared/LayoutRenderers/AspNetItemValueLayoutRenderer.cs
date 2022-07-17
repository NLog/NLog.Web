using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

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
    [LayoutRenderer("aspnet-httpcontext-item")]
    [LayoutRenderer("aspnet-item")]
    public class AspNetItemValueLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets the item variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        /// Gets or sets the item variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Variable { get => Item; set => Item = value; }

        /// <summary>
        /// Gets or sets whether items with a dot are evaluated as properties or not
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public bool EvaluateAsNestedProperties { get; set; }

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
                return;

            var context = HttpContextAccessor.HttpContext;
            var value = PropertyReader.GetValue(item, context?.Items, (items, key) => LookupItemValue(items, key), EvaluateAsNestedProperties);
            var formatProvider = GetFormatProvider(logEvent, Culture);
            builder.AppendFormattedValue(value, Format, formatProvider, ValueFormatter);
        }

#if !ASP_NET_CORE
        private static object LookupItemValue(System.Collections.IDictionary items, string key)
        {
            return items?.Count > 0 && items.Contains(key) ? items[key] : null;
        }
#else
        private static object LookupItemValue(IDictionary<object, object> items, string key)
        {
            return items != null && items.TryGetValue(key, out var itemValue) ? itemValue : null;
        }
#endif
    }
}