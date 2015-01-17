using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.Web.Http.Dependencies;

namespace Triple.Api
{
    public class MefDependencyResolver : MefDependencyScope, IDependencyResolver
    {
        private readonly ExportFactory<CompositionContext> _requestScopeFactory;

        public MefDependencyResolver(CompositionHost rootCompositionScope)
            : base(new Export<CompositionContext>(rootCompositionScope, rootCompositionScope.Dispose))
        {
            var factoryContract = new CompositionContract(typeof(ExportFactory<CompositionContext>), null,
                new Dictionary<string, object>
                {
                    {
                        "SharingBoundaryNames", new[]
                        {
                            "HttpRequest"
                        }
                    }
                });

            this._requestScopeFactory =
                (ExportFactory<CompositionContext>) rootCompositionScope.GetExport(factoryContract);
        }

        public IDependencyScope BeginScope()
        {
            var scope = new MefDependencyScope(this._requestScopeFactory.CreateExport());
            return scope;
        }
    }
}
