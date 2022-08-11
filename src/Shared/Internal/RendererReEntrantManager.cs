using System;
#if NET35
using System.Web;
#else
using System.Threading;
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
    internal class RendererReEntrantManager : IDisposable
    {
#if NET35
        private static readonly object ReEntrantLock = new object();

        internal RendererReEntrantManager(HttpContextBase context)
        {
            httpContext = context;
        }

        private readonly HttpContextBase httpContext;

        private bool ReadLock()
        {
            return httpContext?.Items?.Contains(ReEntrantLock) == true;
        }

        private void Lock()
        {
            httpContext.Items[ReEntrantLock] = bool.TrueString;
        }

        private void Unlock()
        {
            httpContext?.Items?.Remove(ReEntrantLock);
        }
#else
        // Manage access to the session re-entrancy, at least above .NET 3.5
        private static readonly AsyncLocal<bool> ReEntrantLock = new AsyncLocal<bool>();

        internal RendererReEntrantManager()
        {

        }

        private bool ReadLock()
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

        // Need to track if we were successful in the entry
        // If we were not, we should not unlock in the dispose code
        private bool entrySuccess;


        internal bool TryGetLock()
        {
            // If already locked, return false
            if (ReadLock())
            {
                return false;
            }
            // Get the lock
            Lock();
            // Mark that we locked it, not another instance locked it
            entrySuccess = true;
            // Return to the caller that we locked it
            return true;
        }

        private void DisposalImpl()
        {
            // Only unlock if we were the ones who locked it
            if (entrySuccess)
            {
                Unlock();
            }
        }

        // Dispose methods
        // Below code generated by Visual Studio 2022 and Resharper
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposalImpl();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
