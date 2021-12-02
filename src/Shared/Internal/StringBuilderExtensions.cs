using System;
using System.Text;
using NLog.MessageTemplates;

namespace NLog.Web
{
    internal static class StringBuilderExtensions
    {
        private const string FormatAsJson = "@";

        internal static void AppendFormattedValue(this StringBuilder destination, object value, string format, IFormatProvider formatProvider, IValueFormatter valueFormatter)
        {
            string stringValue = value as string;
            if (stringValue != null && string.IsNullOrEmpty(format))
            {
                destination.Append(value);  // Avoid automatic quotes
            }
            else if (FormatAsJson.Equals(format))
            {
                valueFormatter.FormatValue(value, null, CaptureType.Serialize, formatProvider, destination);
            }
            else if (value != null)
            {
                valueFormatter.FormatValue(value, format, CaptureType.Normal, formatProvider, destination);
            }
        }
    }
}