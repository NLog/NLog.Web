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
    internal struct RendererReEntrantManager : IDisposable
    {
        internal RendererReEntrantManager(HttpContextBase context)
        {
            _httpContext = context;
            _obtainedLock = false;
        }

        private readonly HttpContextBase _httpContext;

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

        // Need to track if we were successful in the lock
        // If we were not, we should not unlock in the dispose code
        private bool _obtainedLock;


        internal bool TryGetLock()
        {
            // If already locked, return false
            if (IsLocked())
            {
                return false;
            }
            // Get the lock
            Lock();
            // Mark that we locked it, not another instance locked it
            _obtainedLock = true;
            // Return to the caller that we locked it
            return true;
        }

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
