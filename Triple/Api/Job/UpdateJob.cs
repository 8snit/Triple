using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Serilog;
using Triple.Api.EntityFramework;
using Triple.Api.Model;

namespace Triple.Api.Job
{
    public class UpdateJob : IJob
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public UpdateJob(IDbContextScopeFactory dbContextScopeFactory)
        {
            this._dbContextScopeFactory = dbContextScopeFactory;
        }

        public void Execute(IJobExecutionContext context)
        {
            if (!context.ScheduledFireTimeUtc.HasValue)
            {
                return;
            }

            var date = context.ScheduledFireTimeUtc.Value;

            using (var dbContextScope = this._dbContextScopeFactory.CreateReadOnly())
            {
                var tripleContext = dbContextScope.DbContexts.Get<TripleContext>();

                Log.Verbose(tripleContext.Activities.ToString());
            }
        }
    }
}
