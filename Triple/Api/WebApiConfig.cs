using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Triple.Api.Model;
using Triple.Api.Spatial;

namespace Triple.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration httpConfiguration, CompositionHost compositionHost)
        {
            httpConfiguration.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            httpConfiguration.Formatters.Add(
                new GeoJsonFormatter(new QueryStringMapping("format", "geojson", "application/vnd.geo+json")));

            httpConfiguration.MessageHandlers.Add(new FormatQueryMessageHandler());

            httpConfiguration.MapHttpAttributeRoutes();

            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Trip>("Trips").EntityType.HasKey(e => e.Id);

            var attachments = builder.EntitySet<Attachment>("Attachments").EntityType;
            attachments.HasKey(e => e.Id);
            attachments.Ignore(e => e.Location);
            builder.StructuralTypes.First(t => t.ClrType == typeof(Attachment))
                .AddProperty(typeof(Attachment).GetProperty("EdmLocation")).Name = "Location";

            var activities = builder.EntitySet<Activity>("Activities").EntityType;
            activities.HasKey(e => e.Id);
            activities.Ignore(e => e.Extent);
            var activity = builder.StructuralTypes.First(t => t.ClrType == typeof(Activity));
            activity.AddProperty(typeof(Activity).GetProperty("EdmExtent")).Name = "Extent";

            var rides = builder.EntitySet<Ride>("Rides").EntityType;
            rides.HasKey(e => e.Id);
            rides.Ignore(e => e.Route);
            builder.StructuralTypes.First(t => t.ClrType == typeof(Ride))
                .AddProperty(typeof(Ride).GetProperty("EdmRoute")).Name = "Route";

            var model = builder.GetEdmModel();

            var pathHandler = new DefaultODataPathHandler();

            var routingConventions =
                ODataRoutingConventions.CreateDefaultWithAttributeRouting(httpConfiguration, model);
            routingConventions.Insert(0, new CompositeKeyRoutingConvention());

            httpConfiguration.MapODataServiceRoute("odata", "api", model, pathHandler, routingConventions);

            httpConfiguration.Filters.Add(new ModelValidationFilter());
            httpConfiguration.Filters.Add(new UnhandledExceptionFilter());
            httpConfiguration.Filters.Add(new UseStopwatchFilter());
        }
    }
}
