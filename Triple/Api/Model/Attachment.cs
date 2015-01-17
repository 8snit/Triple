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
    [JsonConverter(typeof(AttachmentGeoJsonConverter))]
    public class Attachment : IAuditable, IEquatable<Attachment>
    {
        private GeometryWrapper _wrappedLocation;

        public DbGeometry Location
        {
            get
            {
                return this._wrappedLocation;
            }
            set
            {
                this._wrappedLocation = value;
            }
        }

        [NotMapped]
        public GeometryPoint EdmLocation
        {
            get
            {
                return this._wrappedLocation;
            }
            set
            {
                this._wrappedLocation = value;
            }
        }

        public long Id { get; set; }

        public Rel Rel { get; set; }

        public string Uri { get; set; }

        public string MetaData { get; set; }

        public Category Category { get; set; }

        public long TripId { get; set; }

        public virtual Trip Trip { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastChangedAt { get; set; }

        public bool Equals(Attachment other)
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
            return this.Equals((Attachment) obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(Attachment left, Attachment right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Attachment left, Attachment right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Rel: {1}, Uri: {2}", this.Id, this.Rel, this.Uri);
        }
    }

    public class AttachmentGeoJsonConverter : JsonConverter
    {
        private readonly GeoJsonDbGeometryConverter _geoJsonDbGeometryConverter = new GeoJsonDbGeometryConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var attachment = value as Attachment;

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Feature");
            this._geoJsonDbGeometryConverter.WriteJson(writer, attachment.Location, serializer);
            writer.WritePropertyName("properties");
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(attachment.Id);
            writer.WritePropertyName("rel");
            serializer.Serialize(writer, attachment.Rel);
            writer.WritePropertyName("uri");
            writer.WriteValue(attachment.Uri);
            writer.WritePropertyName("metaData");
            serializer.Serialize(writer, ((MetaData) attachment.MetaData).Data);
            writer.WritePropertyName("category");
            serializer.Serialize(writer, attachment.Category);
            writer.WritePropertyName("createdAt");
            writer.WriteValue(attachment.CreatedAt);
            writer.WritePropertyName("lastChangedAt");
            writer.WriteValue(attachment.LastChangedAt);
            writer.WritePropertyName("tripId");
            writer.WriteValue(attachment.TripId);
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
            return typeof(Attachment).IsAssignableFrom(objectType);
        }
    }
}
