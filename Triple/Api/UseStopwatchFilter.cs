using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Serilog;
using Serilog.Events;

namespace Triple.Api
{
    public class UseStopwatchFilter : ActionFilterAttribute
    {
        private const string Key = "Stopwatch";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<NoStopWatchAttribute>().Any()
                ||
                actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<NoStopWatchAttribute>().Any())
            {
                return;
            }

            var stopWatch = new Stopwatch();
            actionContext.Request.Properties[Key] = stopWatch;
            stopWatch.Start();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (!actionExecutedContext.Request.Properties.ContainsKey(Key))
            {
                return;
            }

            var stopWatch = actionExecutedContext.Request.Properties[Key] as Stopwatch;
            if (stopWatch == null)
            {
                return;
            }

            stopWatch.Stop();

            var elapsed = stopWatch.Elapsed;

            if (elapsed.TotalMilliseconds > 10000)
            {
                if (Log.IsEnabled(LogEventLevel.Warning))
                {
                    Log.Warning("execution of {@Request} took {Elapsed} ms]",
                        actionExecutedContext.Request.GetSnapshot(), elapsed.TotalMilliseconds);
                }
            }
            else if (elapsed.TotalMilliseconds > 3000)
            {
                if (Log.IsEnabled(LogEventLevel.Information))
                {
                    Log.Information("execution of {@Request} took {Elapsed} ms]",
                        actionExecutedContext.Request.GetSnapshot(), elapsed.TotalMilliseconds);
                }
            }
            else
            {
                if (Log.IsEnabled(LogEventLevel.Verbose))
                {
                    Log.Verbose("execution of {@Request} took {Elapsed} ms]",
                        actionExecutedContext.Request.GetSnapshot(), elapsed.TotalMilliseconds);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class NoStopWatchAttribute : Attribute
    {
    }
}
