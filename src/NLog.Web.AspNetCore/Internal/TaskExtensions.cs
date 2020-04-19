#if NETSTANDARD

using System;
using System.Threading.Tasks;

namespace NLog.Web.Internal
{
    internal static class TaskExtensions
    {
        public static void RunTaskSynchronously(this Task t)
        {
            var task = Task.Run(async () => await t);
            task.Wait();
        }

        public static T RunTaskSynchronously<T>(this Task<T> t)
        {
            T result = default(T);
            var task = Task.Run(async () => result = await t);
            task.Wait();
            return result;
        }
    } 
}
#endif