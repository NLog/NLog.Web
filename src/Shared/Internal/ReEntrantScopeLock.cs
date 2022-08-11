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

            // The line below is required, because this is a struct, otherwise we get CS0188 compiler error
            // which is 'The 'this' object cannot be used before all of its fields have been assigned.'
            _obtainedLock = false;

            _obtainedLock = TryGetLock();
        }

        private readonly HttpContextBase _httpContext;

        // Need to track if we were successful in the lock
        // If we were not, we should not unlock in the dispose code
        private readonly bool _obtainedLock;

        internal bool IsLockAcquired => _obtainedLock;

        private bool TryGetLock()
        {
            // If already locked, return false
            if (IsLocked())
            {
                return false;
            }
            // Get the lock
            Lock();
            return true;
        }

#if NET35
        private static readonly object ReEntrantLock = new object();

        private bool IsLocked()
        {
            return _httpContext?.Items?.Contains(ReEntrantLock) == true;
        }

        private void Lock()
        {
            _httpContext.Items[ReEntrantLock] = bool.TrueString;
        }

        private void Unlock()
        {
            _httpContext?.Items?.Remove(ReEntrantLock);
        }
#else
        private static readonly AsyncLocal<bool> ReEntrantLock = new AsyncLocal<bool>();

        private bool IsLocked()
        {
            return ReEntrantLock.Value;
        }

        private void Lock()
        {
            ReEntrantLock.Value = true;
        }

        private void Unlock()
        {
            ReEntrantLock.Value = false;
        }
#endif

        private void DisposalImpl()
        {
            // Only unlock if we were the ones who locked it
            if (_obtainedLock)
            {
                Unlock();
            }
        }

        public void Dispose()
        {
            DisposalImpl();
        }
    }
}
