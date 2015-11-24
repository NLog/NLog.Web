using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog.Web.Internal
{
    internal class PropertyReader
    {
        /// <summary>
        /// Get value of a property
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="getVal">function to get a value with this key</param>
        /// <param name="evaluateAsNestedProperties">evaluate <paramref name="key"/> as a nested property path. E.g. A.B is property B inside A.</param>
        /// <returns>value</returns>
        public static object GetValue(string key, Func<string, object> getVal, bool evaluateAsNestedProperties)
        {
            object value;
            if (evaluateAsNestedProperties)
            {
                var path = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                value = getVal(path.First());

                foreach (var property in path.Skip(1))
                {
                    var propertyInfo = value.GetType().GetProperty(property);
                    value = propertyInfo.GetValue(value, null);
                }
            }
            else
            {
                value = getVal(key);
            }
            return value;
        }
    }
}