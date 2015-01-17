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
    [JsonConverter(typeof(ActivityGeoJsonConverter))]
    public class Activity : IAuditable, IEquatable<Activity>
    {
        private GeometryWrapper _wrappedExtent;

        public DbGeometry Extent
        {
            get
            {
                return this._wrappedExtent;
            }
            set
            {
                this._wrappedExtent = value;
            }
        }

        [NotMapped]
        public GeometryPoint EdmExtent
        {
            get
            {
                return this._wrappedExtent;
            }
            set
            {
                this._wrappedExtent = value;
            }
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public DateTimeOffset StartingAt { get; set; }

        public DateTimeOffset EndingAt { get; set; }

        public virtual long TripId { get; set; }

        public virtual Trip Trip { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastChangedAt { get; set; }

        public bool Equals(Activity other)
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
            return this.Equals((Activity) obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(Activity left, Activity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Activity left, Activity right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, StartingAt: {2}, EndingAt: {3}", this.Id, this.Name,
                this.StartingAt, this.EndingAt);
        }
    }

    public class ActivityGeoJsonConverter : JsonConverter
    {
        private readonly GeoJsonDbGeometryConverter _geoJsonDbGeometryConverter = new GeoJsonDbGeometryConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ride = value as Activity;

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Feature");
            this._geoJsonDbGeometryConverter.WriteJson(writer, ride.Extent, serializer);
            writer.WritePropertyName("properties");
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(ride.Id);
            writer.WritePropertyName("name");
            writer.WriteValue(ride.Name);
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
            return typeof(Activity).IsAssignableFrom(objectType);
        }
    }
}
