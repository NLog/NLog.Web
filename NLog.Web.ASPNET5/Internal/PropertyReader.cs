using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            if (key == null)
            {
                return null;
            }

            object value;
            if (evaluateAsNestedProperties)
            {
                var path = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                value = getVal(path.First());

                foreach (var property in path.Skip(1))
                {
                    var propertyInfo = GetPropertyInfo(value, property);
                    value = propertyInfo.GetValue(value, null);
                }
            }
            else
            {
                value = getVal(key);
            }
            return value;
        }

        private static PropertyInfo GetPropertyInfo(object value, string propertyName)
        {
#if !DNX
            return value.GetType().GetProperty(propertyName);
#else
            return value.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
#endif
        }
    }
}