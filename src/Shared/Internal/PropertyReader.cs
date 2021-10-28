using System;
using System.Linq;
using System.Reflection;

namespace NLog.Web.Internal
{
    internal static class PropertyReader
    {
        /// <summary>
        /// Get value of a property
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="container">Container to perform value lookup using key</param>
        /// <param name="getVal">function to get a value with this key</param>
        /// <param name="evaluateAsNestedProperties">evaluate <paramref name="key" /> as a nested property path. E.g. A.B is property B inside A.</param>
        /// <returns>value</returns>
        public static object GetValue<T>(string key, T container, Func<T, string, object> getVal, bool evaluateAsNestedProperties)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            var value = evaluateAsNestedProperties ? GetValueAsNestedProperties(key, container, getVal) : getVal(container, key);
            return value;
        }

        private static object GetValueAsNestedProperties<T>(string key, T container, Func<T, string, object> getVal)
        {
            var path = key.Contains('.') ? key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries) : null;

            var value = getVal(container, path?.First() ?? key);
            if (value != null && path?.Length > 1)
            {
                for (int i = 1; i < path.Length; ++i)
                {
                    var propertyInfo = GetPropertyInfo(value, path[i]);
                    value = propertyInfo?.GetValue(value, null);
                    if (value == null)
                    {
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