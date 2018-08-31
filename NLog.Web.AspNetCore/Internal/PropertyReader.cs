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
        /// <param name="container">Container to perform value lookup using key</param>
        /// <param name="getVal">function to get a value with this key</param>
        /// <param name="evaluateAsNestedProperties">evaluate <paramref name="key"/> as a nested property path. E.g. A.B is property B inside A.</param>
        /// <returns>value</returns>
        public static object GetValue<T>(string key, T container, Func<T, string, object> getVal, bool evaluateAsNestedProperties)
        {
            if (String.IsNullOrEmpty(key))
            {
                return null;
            }

            var value = evaluateAsNestedProperties ? GetValueAsNestedProperties(key, container, getVal) : getVal(container, key);
            return value;
        }

        private static object GetValueAsNestedProperties<T>(string key, T container, Func<T, string, object> getVal)
        {
            var path = key.IndexOf('.') >= 0 ? key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries) : null;

            var value = getVal(container, path?.First() ?? key);
            if (value != null && path?.Length > 1)
            {
                foreach (var property in path.Skip(1))
                {
                    var propertyInfo = GetPropertyInfo(value, property);
                    value = propertyInfo?.GetValue(value, null);
                    if (value == null)
                    {
                        //done
                        break;
                    }
                }
            }
            return value;
        }

        private static PropertyInfo GetPropertyInfo(object value, string propertyName)
        {
#if !ASP_NET_CORE
            return value?.GetType().GetProperty(propertyName);
#else
            return value?.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
#endif
        }
    }
}