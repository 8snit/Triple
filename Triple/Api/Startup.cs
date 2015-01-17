using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Owin;
using Owin;
using Serilog;
using Serilog.Events;
using Triple.Api;

[assembly: OwinStartup(typeof(Startup))]

namespace Triple.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigurationManager.AppSettings["triple.startuptimestamp"] =
                DateTimeOffset.Now.ToString(CultureInfo.InvariantCulture);
            ConfigurationManager.AppSettings["triple.appdir"] = HostingEnvironment.ApplicationPhysicalPath;

            var logDirectory = AppSettings.LogDirectory;

            var logLevel = ConfigurationManager.AppSettings["triple.loglevel"];
            if (string.IsNullOrEmpty(logLevel))
            {
                logLevel = "info";
            }

            var loggerConfiguration = new LoggerConfiguration();

            switch (logLevel.ToLowerInvariant()[0])
            {
                case 'v':
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Verbose();
                    break;
                case 'd':
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Debug();
                    break;
                case 'i':
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Information();
                    break;
                case 'w':
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Warning();
                    break;
                case 'e':
                    loggerConfiguration = loggerConfiguration.MinimumLevel.Error();
                    break;
            }

            Log.Logger = loggerConfiguration
                .WriteTo.RollingFile(logDirectory + "triple-{Date}.log",
                    outputTemplate: "{Timestamp} [{Level}] {Message:l}{NewLine:l}{Exception:l}")
                .CreateLogger();

            Log.Information("starting version {Version}", ConfigurationManager.AppSettings["triple.version"]);

            var assemblies = new HashSet<Assembly>
            {
                Assembly.GetExecutingAssembly()
            };

            var conventionBuilder = new ConventionBuilder();
            conventionBuilder.ForTypesDerivedFrom<IHttpController>().Export();
            var container = new ContainerConfiguration()
                .WithAssemblies(assemblies.ToArray(), conventionBuilder)
                .CreateContainer();

            app.Use((context, next) =>
            {
                if (Log.IsEnabled(LogEventLevel.Verbose))
                {
                    Log.Verbose("Url={Url}", context.Request.Uri);
                }

                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["triple.appurl"]))
                {
                    var regex = new Regex("(" + context.Request.Uri.AbsolutePath + ")$");
                    var appUrl = regex.Replace(context.Request.Uri.AbsoluteUri, string.Empty);
                    ConfigurationManager.AppSettings["triple.appurl"] = appUrl;

                    Log.Information("running at {ApplicationUrl}", appUrl);
                }

                context.Request.Headers.Remove("If-None-Match");
                context.Request.Headers.Remove("If-Modified-Since");

                return next.Invoke();
            });

            var httpConfiguration = new HttpConfiguration
            {
                DependencyResolver = new MefDependencyResolver(container)
            };
            WebApiConfig.Register(httpConfiguration, container);
            app.UseWebApi(httpConfiguration);

            //var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            //scheduler.JobFactory = new MefJobFactory(container);
            //scheduler.Start();

            //var job = JobSpatialBuilder.Create().Create<UpdateJob>()
            //    .WithIdentity(typeof(UpdateJob).Name, "triple")
            //    .Build();

            //var trigger =
            //    TriggerSpatialBuilder.Create().Create().WithSimpleSchedule(s => s.WithIntervalInSeconds(10).RepeatForever()).Build();

            //scheduler.ScheduleJob(job, trigger);
        }
    }
}
