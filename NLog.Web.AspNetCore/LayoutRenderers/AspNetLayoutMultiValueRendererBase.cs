using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.Web.Enums;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Layout renderers for ASP.NET rendering multiple values.
    /// </summary>
    public abstract class AspNetLayoutMultiValueRendererBase : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Separator between item. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat"/>
        /// </summary>
        public string ItemSeparator { get; set; } = ",";

        /// <summary>
        /// Separator between value and key. Only used for <see cref="AspNetRequestLayoutOutputFormat.Flat"/>
        /// </summary>
        public string ValueSeparator { get; set; } = "=";

        /// <summary>
        /// Single item in array? Only used for <see cref="AspNetRequestLayoutOutputFormat.Json"/>
        /// 
        /// Mutliple items are always in an array.
        /// </summary>
        public bool SingleAsArray { get; set; } = true;

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public AspNetRequestLayoutOutputFormat OutputFormat { get; set; } = AspNetRequestLayoutOutputFormat.Flat;

        /// <summary>
        /// Serialize multiple values in pairs format.
        /// </summary>
        /// <param name="pairs">The key value pairs to serialize.</param>
        /// <param name="builder">Add to this builder.</param>
        protected void SerializePairs(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializePairsFlat(pairs, builder);
                    break;
                case AspNetRequestLayoutOutputFormat.Json:
                    SerializePairsJson(pairs, builder);
                    break;
            }
        }

        private void SerializePairsJson(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            var firstItem = true;
            var valueList = pairs.ToList();

            if (valueList.Count > 0)
            {
                var addArray = valueList.Count > (SingleAsArray ? 0 : 1);

                if (addArray)
                {
                    builder.Append('[');
                }

                foreach (var kpv in valueList)
                {
                    var key = kpv.Key;
                    var value = kpv.Value;
                    if (!firstItem)
                    {
                        builder.Append(',');
                    }
                    firstItem = false;

                    //quoted key
                    builder.Append('{');
                    AppendQuoted(builder, key);

                    builder.Append(':');
                    //quoted value;
                    AppendQuoted(builder, value);
                    builder.Append('}');
                }
                if (addArray)
                {
                    builder.Append(']');
                }
            }
        }

        private void SerializePairsFlat(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            var firstItem = true;
            foreach (var kpv in pairs)
            {
                var key = kpv.Key;
                var value = kpv.Value;

                if (!firstItem)
                {
                    builder.Append(ItemSeparator);
                }
                firstItem = false;
                builder.Append(key);
                builder.Append(ValueSeparator);
                builder.Append(value);
            }
        }

        /// <summary>
        /// Serialize values only from the given pairs.
        /// </summary>
        /// <param name="pairs">The key value pairs to serialize.</param>
        /// <param name="builder">Add to this builder.</param>
        protected void SerializeValues(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            switch (OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    SerializeValuesFlat(pairs, builder);
                    break;
                case AspNetRequestLayoutOutputFormat.Json:
                    SerializeValuesJson(pairs, builder);
                    break;
            }
        }

        private void SerializeValuesFlat(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            var firstItem = true;
            foreach (var kpv in pairs)
            {
                var value = kpv.Value;

                if (!firstItem)
                {
                    builder.Append(ItemSeparator);
                }
                firstItem = false;
                builder.Append(value);
            }
        }

        private void SerializeValuesJson(IEnumerable<KeyValuePair<string, string>> pairs, StringBuilder builder)
        {
            var firstItem = true;
            var valueList = pairs.ToList();

            if (valueList.Count > 0)
            {
                var addArray = valueList.Count > (SingleAsArray ? 0 : 1);

                if (addArray)
                {
                    builder.Append('[');
                }

                foreach (var kpv in valueList)
                {
                    var value = kpv.Value;
                    if (!firstItem)
                    {
                        builder.Append(',');
                    }
                    firstItem = false;

                    AppendQuoted(builder, value);
                }
                if (addArray)
                {
                    builder.Append(']');
                }
            }
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
                //escape quotes
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