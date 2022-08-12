using System;
#if !NET35
using System.Threading;
#endif
#if ASP_NET_CORE
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#else
using System.Web;
#endif

namespace NLog.Web.Internal
{
    /// <summary>
    /// Manages if a LayoutRenderer can be called recursively.
    /// Especially used by AspNetSessionValueLayoutRenderer
    /// 
    /// Since NET 35 does not support AsyncLocal or even ThreadLocal
    /// a different technique must be used for that platform
    /// </summary>
    internal readonly struct ReEntrantScopeLock : IDisposable
    {
        internal ReEntrantScopeLock(HttpContextBase context)
        {
            _httpContext = context;
            IsLockAcquired = TryGetLock(context);
        }

        private readonly HttpContextBase _httpContext;

        // Need to track if we were successful in the lock
        // If we were not, we should not unlock in the dispose code
        internal bool IsLockAcquired { get; }

        private static bool TryGetLock(HttpContextBase context)
        {
            // If context is null leave
            if (context == null)
            {
                return false;
            }

            // If already locked, return false
            if (IsLocked(context))
            {
                return false;
            }

            // Get the lock
            Lock(context);

            // Indicate the lock was successfully acquired
            return true;
        }

        public void Dispose()
        {
            // Only unlock if we were the ones who locked it
            if (IsLockAcquired)
            {
                Unlock(_httpContext);
            }
        }

#if NET35
        private static readonly object ReEntrantLock = new object();

        private static bool IsLocked(HttpContextBase context)
        {
            return context.Items?.Contains(ReEntrantLock) == true;
        }

        private static void Lock(HttpContextBase context)
        {
            context.Items[ReEntrantLock] = bool.TrueString;
        }

        private static void Unlock(HttpContextBase context)
        {
            context.Items?.Remove(ReEntrantLock);
        }
#else
        private static readonly AsyncLocal<bool> ReEntrantLock = new AsyncLocal<bool>();

        private static bool IsLocked(HttpContextBase context)
        {
            return ReEntrantLock.Value;
        }

        private static void Lock(HttpContextBase context)
        {
            ReEntrantLock.Value = true;
        }

        private static void Unlock(HttpContextBase context)
        {
            ReEntrantLock.Value = false;
        }
#endif
    }
}
