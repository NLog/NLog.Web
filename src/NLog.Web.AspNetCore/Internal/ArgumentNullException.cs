using System.Runtime.CompilerServices;

namespace NLog.Web.Internal
{
    internal static class ArgumentNullException
    {
        public static T ThrowIfNull<T>(T argument,
#if NETCOREAPP3_1_OR_GREATER
            [CallerArgumentExpression("argument")]
#endif
            string paramName = null)
            where T : class
        {
            if (argument is null)
            {
                throw new System.ArgumentNullException(paramName);
            }

            return argument;
        }
    }
}