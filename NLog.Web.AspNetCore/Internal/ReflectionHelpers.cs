using System;
using System.Linq;
using System.Reflection;

namespace NLog.Web.Internal
{
    internal static class ReflectionHelpers
    {
        public static TAttr GetCustomAttribute<TAttr>(Assembly assembly)
            where TAttr : Attribute
        {
#if NETSTANDARD1_5
            return assembly.GetCustomAttributes(typeof(TAttr)).FirstOrDefault() as TAttr;
#else
            return (TAttr)Attribute.GetCustomAttribute(assembly, typeof(TAttr));
#endif
        }
    }
}
