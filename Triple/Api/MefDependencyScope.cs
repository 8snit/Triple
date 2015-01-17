using System;
using System.Collections.Generic;
using System.Composition;
using System.Web.Http.Dependencies;
using Serilog;

namespace Triple.Api
{
    public class MefDependencyScope : IDependencyScope
    {
        private readonly Export<CompositionContext> _compositionScope;

        public MefDependencyScope(Export<CompositionContext> compositionScope)
        {
            this._compositionScope = compositionScope;
        }

        public CompositionContext CompositionScope
        {
            get
            {
                return this._compositionScope.Value;
            }
        }

        public object GetService(Type serviceType)
        {
            try
            {
                if (serviceType == null)
                {
                    return null;
                }

                object result;
                this.CompositionScope.TryGetExport(serviceType, null, out result);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "resolving type {Resolved} failed", serviceType);
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                if (serviceType == null)
                {
                    return null;
                }

                return this.CompositionScope.GetExports(serviceType, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "resolving type {Resolved} failed", serviceType);
                return null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._compositionScope.Dispose();
            }
        }
    }
}
