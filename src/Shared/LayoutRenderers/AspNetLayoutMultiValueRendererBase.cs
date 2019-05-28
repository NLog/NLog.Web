using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.Layouts;
using NLog.Web.Enums;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Layout renderers for ASP.NET rendering multiple key/value pairs.
    /// </summary>
    public abstract class AspNetLayoutMultiValueRendererBase : AspNetLayoutRendererBase
    {
        private string _itemSeparator = ",";
        private Layout _itemSeparatorLayout = ",";
        private string _valueSeparator = "=";
        private Layout _valueSeparatorLayout = "=";

        /// <summary>
        /// Separator between item. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat" />
        /// </summary>
        /// <remarks>Render with <see cref="GetRenderedItemSeparator" /></remarks>
        public string ItemSeparator
        {
            get => _itemSeparator;
            set
            {
                _itemSeparator = value;
                _itemSeparatorLayout = value;
            }
        }

        /// <summary>
        /// Separator between value and key. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat" />
        /// </summary>
        /// <remarks>Render with <see cref="GetRenderedValueSeparator" /></remarks>
        public string ValueSeparator
        {
            get => _valueSeparator;
            set
            {
                _valueSeparator = value;
                _valueSeparatorLayout = value;
            }
        }

        /// <summary>
        /// Single item in array? Only used for <see cref="AspNetRequestLayoutOutputFormat.Json" />
        /// Mutliple items are always in an array.
        /// </summary>
        public bool SingleAsArray { get; set; } = true;

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public AspNetRequestLayoutOutputFormat OutputFormat { get; set; } = AspNetRequestLayoutOutputFormat.Flat;

        /// <summary>
        /// Only render values if true, otherwise render key/value pairs.
        /// </summary>
        public bool ValuesOnly { get; set; }

        /// <summary>
        /// Serialize multiple key/value pairs
        /// </summary>
        /// <param name="pairs">The key/value pairs.</param>
        /// <param name="builder">Add to this builder.</param>
        [Obsolete("use SerializePairs with logEvent to support Layouts for Separator. This overload will be removed in NLog.Web(aspNetCore) 5")]
        protected void SerializePairs(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            SerializePairs(pairs, builder, null);
        }

        /// <summary>
        /// Serialize multiple key/value pairs
        /// </summary>
        /// <param name="pairs">The key/value pairs.</param>
        /// <param name="builder">Add to this builder.</param>
        /// <param name="logEvent">Log event for rendering separators.</param>
        protected void SerializePairs(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder, LogEventInfo logEvent)
        {
            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializePairsFlat(pairs, builder, logEvent);
                    break;
                case AspNetRequestLayoutOutputFormat.Json:
                    SerializePairsJson(pairs, builder);
                    break;
            }
        }

        private void SerializePairsJson(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            var firstItem = true;
            var pairsList = pairs.ToList();

            if (pairsList.Count > 0)
            {
                var addArray = pairsList.Count > (SingleAsArray | ValuesOnly ? 0 : 1);

                if (addArray)
                {
                    builder.Append('[');
                }

                foreach (var kpv in pairsList)
                {
                    var key = kpv.Key;
                    var value = kpv.Value;
                    if (!firstItem)
                    {
                        builder.Append(',');
                    }

                    firstItem = false;

                    if (!ValuesOnly)
                    {
                        // Quoted key
                        builder.Append('{');
                        AppendQuoted(builder, key);

                        builder.Append(':');
                    }

                    // Quoted value
                    AppendQuoted(builder, value);

                    if (!ValuesOnly)
                    {
                        builder.Append('}');
                    }
                }

                if (addArray)
                {
                    builder.Append(']');
                }
            }
        }

        private void SerializePairsFlat(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder,
            LogEventInfo logEvent)
        {
            var itemSeparator = GetRenderedItemSeparator(logEvent);
            var valueSeparator = GetRenderedValueSeparator(logEvent);

            var firstItem = true;
            foreach (var kpv in pairs)
            {
                var key = kpv.Key;
                var value = kpv.Value;

                if (!firstItem)
                {
                    builder.Append(itemSeparator);
                }

                firstItem = false;

                if (!ValuesOnly)
                {
                    builder.Append(key);

                    builder.Append(valueSeparator);
                }

                builder.Append(value);
            }
        }

        /// <summary>
        /// Get the rendered <see cref="ItemSeparator" />
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        protected string GetRenderedItemSeparator(LogEventInfo logEvent)
        {
            return logEvent != null ? _itemSeparatorLayout.Render(logEvent) : ItemSeparator;
        }

        /// <summary>
        /// Get the rendered <see cref="ValueSeparator" />
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        protected string GetRenderedValueSeparator(LogEventInfo logEvent)
        {
            return logEvent != null ? _valueSeparatorLayout.Render(logEvent) : ValueSeparator;
        }

        /// <summary>
        /// Append the value quoted, escape quotes when needed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        private static void AppendQuoted(StringBuilder builder, string value)
        {
            builder.Append('"');
            if (!string.IsNullOrEmpty(value) && value.Contains('"'))
            {
                builder.Append(value.Replace("\"", "\\\""));
            }
            else
            {
                builder.Append(value);
            }

            builder.Append('"');
        }
    }
}