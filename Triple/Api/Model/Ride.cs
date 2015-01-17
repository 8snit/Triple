using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using Microsoft.Spatial;
using Newtonsoft.Json;
using Triple.Api.Spatial;

namespace Triple.Api.Model
{
    [JsonConverter(typeof(RideGeoJsonConverter))]
    public class Ride : IAuditable, IEquatable<Ride>
    {
        private GeometryWrapper _wrappedRoute;

        public DbGeometry Route
        {
            get
            {
                return this._wrappedRoute;
            }
            set
            {
                this._wrappedRoute = value;
            }
        }

        [NotMapped]
        public GeometryLineString EdmRoute
        {
            get
            {
                return this._wrappedRoute;
            }
            set
            {
                this._wrappedRoute = value;
            }
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public Transport Transport { get; set; }

        public DateTimeOffset StartingAt { get; set; }

        public DateTimeOffset EndingAt { get; set; }

        public long TripId { get; set; }

        public virtual Trip Trip { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastChangedAt { get; set; }

        public bool Equals(Ride other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return this.Equals((Ride) obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(Ride left, Ride right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Ride left, Ride right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, Transport: {2}, StartsAt: {3}, EndsAt: {4}", this.Id, this.Name,
                this.Transport, this.StartingAt, this.EndingAt);
        }
    }

    public class RideGeoJsonConverter : JsonConverter
    {
        private readonly GeoJsonDbGeometryConverter _geoJsonDbGeometryConverter = new GeoJsonDbGeometryConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ride = value as Ride;

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Feature");
            this._geoJsonDbGeometryConverter.WriteJson(writer, ride.Route, serializer);
            writer.WritePropertyName("properties");
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(ride.Id);
            writer.WritePropertyName("name");
            writer.WriteValue(ride.Name);
            writer.WritePropertyName("transport");
            serializer.Serialize(writer, ride.Transport);
            writer.WritePropertyName("startingAt");
            writer.WriteValue(ride.StartingAt);
            writer.WritePropertyName("endingAt");
            writer.WriteValue(ride.EndingAt);
            writer.WritePropertyName("createdAt");
            writer.WriteValue(ride.CreatedAt);
            writer.WritePropertyName("lastChangedAt");
            writer.WriteValue(ride.LastChangedAt);
            writer.WritePropertyName("tripId");
            writer.WriteValue(ride.TripId);
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Ride).IsAssignableFrom(objectType);
        }
    }
}
