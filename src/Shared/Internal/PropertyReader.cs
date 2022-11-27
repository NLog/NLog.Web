using System;
using System.Linq;
using System.Reflection;

namespace NLog.Web.Internal
{
    [Obsolete("Instead use ObjectPath. Marked obsolete with NLog.Web 5.2")]
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
        [Obsolete("Instead use ObjectPath. Marked obsolete with NLog.Web 5.2")]
        public static object GetValue<T>(string key, T container, Func<T, string, object> getVal, bool evaluateAsNestedProperties)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return evaluateAsNestedProperties ? GetValueAsNestedProperties(key, container, getVal) : getVal(container, key);
        }

        [Obsolete("Instead use ObjectPath. Marked obsolete with NLog.Web 5.2")]
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

        [Obsolete("Instead use ObjectPath. Marked obsolete with NLog.Web 5.2")]
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