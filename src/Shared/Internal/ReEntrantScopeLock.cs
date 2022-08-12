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

#if NET35
        private static readonly object ReEntrantLock = new object();

        private static bool TryGetLock(HttpContextBase context)
        {
            // If context is null leave
            if (context == null)
            {
                return false;
            }

            // If already locked, return false
            if (context.Items?.Contains(ReEntrantLock) == true)
            {
                return false;
            }

            // Get the lock
            context.Items[ReEntrantLock] = bool.TrueString;

            // Indicate the lock was successfully acquired
            return true;
        }

        public void Dispose()
        {
            // Only unlock if we were the ones who locked it
            if (IsLockAcquired)
            {
                _httpContext.Items?.Remove(ReEntrantLock);
            }
        }
#else
        private static readonly AsyncLocal<bool> ReEntrantLock = new AsyncLocal<bool>();

        private static bool TryGetLock(HttpContextBase context)
        {
            // If context is null leave
            if (context == null)
            {
                return false;
            }

            // If already locked, return false
            if (ReEntrantLock.Value)
            {
                return false;
            }

            // Get the lock
            ReEntrantLock.Value = true;

            // Indicate the lock was successfully acquired
            return true;
        }

        public void Dispose()
        {
            // Only unlock if we were the ones who locked it
            if (IsLockAcquired)
            {
                ReEntrantLock.Value = false;
            }
        }
#endif
    }
}
