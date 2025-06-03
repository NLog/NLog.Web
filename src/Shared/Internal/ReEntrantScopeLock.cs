#if !NET35

namespace NLog.Web.Internal
{
    using System;
    using System.Threading;

    /// <summary>
    /// Manages if a LayoutRenderer can be called recursively using AsyncLocal
    /// Example used by <see cref="NLog.Web.LayoutRenderers.AspNetSessionItemLayoutRenderer"/>
    /// </summary>
    internal readonly struct ReEntrantScopeLock : IDisposable
    {
        public ReEntrantScopeLock(bool acquireLock = true)
        {
            IsLockAcquired = acquireLock && TryGetLock();
        }

        internal bool IsLockAcquired { get; }

        private static readonly AsyncLocal<bool> ReEntrantLock = new AsyncLocal<bool>();

        private static bool TryGetLock()
        {
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
    }
}


#endif