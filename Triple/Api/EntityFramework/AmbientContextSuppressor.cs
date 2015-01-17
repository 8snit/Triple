using System;

namespace Triple.Api.EntityFramework
{
    public class AmbientContextSuppressor : IDisposable
    {
        private bool _disposed;

        private DbContextScope _savedScope;

        public AmbientContextSuppressor()
        {
            this._savedScope = DbContextScope.GetAmbientScope();

            // We're hiding the ambient scope but not removing its instance
            // altogether. This is to be tolerant to some programming errors. 
            // 
            // Suppose we removed the ambient scope instance here. If someone
            // was to start a parallel task without suppressing
            // the ambient context and then tried to suppress the ambient
            // context within the parallel task while the raw flow
            // of execution was still ongoing (a strange thing to do, I know,
            // but I'm sure this is going to happen), we would end up 
            // removing the ambient context instance of the raw flow 
            // of execution from within the parallel flow of execution!
            // 
            // As a result, any code in the raw flow of execution
            // that would attempt to access the ambient scope would end up 
            // with a null value since we removed the instance.
            //
            // It would be a fairly nasty bug to track down. So don't let
            // that happen. Hiding the ambient scope (i.e. clearing the CallContext
            // in our execution flow but leaving the ambient scope instance untouched)
            // is safe.
            DbContextScope.HideAmbientScope();
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            if (this._savedScope != null)
            {
                DbContextScope.SetAmbientScope(this._savedScope);
                this._savedScope = null;
            }

            this._disposed = true;
        }
    }
}
