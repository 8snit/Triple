using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using Quartz;
using Quartz.Spi;

namespace Triple.Api
{
    public class MefJobFactory : IJobFactory
    {
        private readonly ExportFactory<CompositionContext> _requestScopeFactory;

        public MefJobFactory(CompositionHost rootCompositionScope)
        {
            if (rootCompositionScope == null)
            {
                throw new ArgumentNullException("rootCompositionScope");
            }

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

        public virtual IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;
            var scope = new MefDependencyScope(this._requestScopeFactory.CreateExport());
            var service = scope.GetService(jobType);
            return service as IJob;
        }

        public virtual void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
