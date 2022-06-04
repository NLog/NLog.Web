using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Layouts;
using NLog.Web.Enums;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Layout renderers for ASP.NET rendering multiple key/value pairs.
    /// </summary>
    public abstract class AspNetLayoutMultiValueRendererBase : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Separator between item. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat" />
        /// </summary>
        /// <remarks>Render with <see cref="GetRenderedItemSeparator" /></remarks>
        public string ItemSeparator { get => _itemSeparatorLayout?.OriginalText; set => _itemSeparatorLayout = new SimpleLayout(value ?? ""); }
        private SimpleLayout _itemSeparatorLayout = new SimpleLayout(",");

        /// <summary>
        /// Separator between value and key. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat" />
        /// </summary>
        /// <remarks>Render with <see cref="GetRenderedValueSeparator" /></remarks>
        public string ValueSeparator { get => _valueSeparatorLayout?.OriginalText; set => _valueSeparatorLayout = new SimpleLayout(value ?? ""); }
        private SimpleLayout _valueSeparatorLayout = new SimpleLayout("=");

        /// <summary>
        /// Get or set whether single key/value-pair be rendered as Json-Array.
        /// </summary>
        [Obsolete("Replaced by OutputFormat = JsonArray / JsonDictionary. Marked obsolete with NLog.Web ver. 5.0")]
        public bool SingleAsArray
        {
            get => OutputFormat != AspNetRequestLayoutOutputFormat.JsonDictionary;
            set
            {
                if (!value)
                    OutputFormat = AspNetRequestLayoutOutputFormat.JsonDictionary;
                else if (OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                    OutputFormat = AspNetRequestLayoutOutputFormat.JsonArray;
            }
        }

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
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
        /// <param name="logEvent">Log event for rendering separators.</param>
        protected void SerializePairs(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder, LogEventInfo logEvent)
        {
            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializePairsFlat(pairs, builder, logEvent);
                    break;
                case AspNetRequestLayoutOutputFormat.JsonArray:
                case AspNetRequestLayoutOutputFormat.JsonDictionary:
                    SerializePairsJson(pairs, builder);
                    break;
            }
        }

        private void SerializePairsJson(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            var firstItem = true;

            foreach (var item in pairs)
            {
                if (firstItem)
                {
                    if (!ValuesOnly && OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                    {
                        builder.Append('{');
                    }
                    else
                    {
                        builder.Append('[');
                    }
                }
                else
                {
                    builder.Append(',');
                }

                SerializePairJson(builder, item);

                firstItem = false;
            }

            if (!firstItem)
            {
                if (!ValuesOnly && OutputFormat == AspNetRequestLayoutOutputFormat.JsonDictionary)
                {
                    builder.Append('}');
                }
                else
                {
                    builder.Append(']');
                }
            }
        }

        private void SerializePairJson(StringBuilder builder, KeyValuePair<string, string> kpv)
        {
            var key = kpv.Key;
            var value = kpv.Value;

            if (!ValuesOnly)
            {
                // Quoted key
                if (OutputFormat != AspNetRequestLayoutOutputFormat.JsonDictionary)
                {
                    builder.Append('{');
                }
                AppendQuoted(builder, key);
                builder.Append(':');
            }

            // Quoted value
            AppendQuoted(builder, value);

            if (!ValuesOnly && OutputFormat != AspNetRequestLayoutOutputFormat.JsonDictionary)
            {
                builder.Append('}');
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
        /// Serialize multiple values
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="builder">Add to this builder.</param>
        /// <param name="logEvent">Log event for rendering separators.</param>
        protected void SerializeValues(IEnumerable<string> values, StringBuilder builder, LogEventInfo logEvent)
        {
            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializeValuesFlat(values, builder, logEvent);
                    break;
                case AspNetRequestLayoutOutputFormat.JsonArray:
                case AspNetRequestLayoutOutputFormat.JsonDictionary:
                    SerializeValuesJson(values, builder);
                    break;
            }
        }

        private static void SerializeValuesJson(IEnumerable<string> values, StringBuilder builder)
        {
            var firstItem = true;
            foreach (var item in values)
            {
                if (firstItem)
                {
                    builder.Append('[');
                }
                else
                {
                    builder.Append(',');
                }
                SerializeValueJson(builder, item);
                firstItem = false;
            }
            if (!firstItem)
            {
                builder.Append(']');
            }
        }

        private static void SerializeValueJson(StringBuilder builder, string value)
        {
            // Quoted value
            AppendQuoted(builder, value);
        }

        private void SerializeValuesFlat(IEnumerable<string> values, StringBuilder builder, LogEventInfo logEvent)
        {
            var itemSeparator = GetRenderedItemSeparator(logEvent);
            var firstItem = true;
            foreach (var value in values)
            {
                if (!firstItem)
                {
                    builder.Append(itemSeparator);
                }
                firstItem = false;
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