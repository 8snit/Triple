using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Triple.Api.Model
{
    [JsonConverter(typeof(TripGeoJsonConverter))]
    public class Trip : IAuditable, IEquatable<Trip>
    {
        public Trip()
        {
            this.Activities = new HashSet<Activity>();
            this.Rides = new HashSet<Ride>();
            this.Attachments = new HashSet<Attachment>();
        }

        public long Id { get; set; }

        public string Slug { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<Ride> Rides { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? LastChangedAt { get; set; }

        public bool Equals(Trip other)
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
            return this.Equals((Trip) obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(Trip left, Trip right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Trip left, Trip right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Slug: {1}, Name: {2}", this.Id, this.Slug, this.Name);
        }
    }

    public class TripGeoJsonConverter : JsonConverter
    {
        private readonly ActivityGeoJsonConverter _activityGeoJsonConverter = new ActivityGeoJsonConverter();

        private readonly AttachmentGeoJsonConverter _attachmentGeoJsonConverter = new AttachmentGeoJsonConverter();

        private readonly RideGeoJsonConverter _rideGeoJsonConverter = new RideGeoJsonConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var trip = value as Trip;

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("FeatureCollection");
            writer.WritePropertyName("properties");
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(trip.Id);
            writer.WritePropertyName("slug");
            writer.WriteValue(trip.Slug);
            writer.WritePropertyName("name");
            writer.WriteValue(trip.Name);
            writer.WritePropertyName("createdAt");
            writer.WriteValue(trip.CreatedAt);
            writer.WritePropertyName("lastChangedAt");
            writer.WriteValue(trip.LastChangedAt);
            writer.WriteEndObject();
            writer.WritePropertyName("features");
            writer.WriteStartArray();
            foreach (var activity in trip.Activities)
            {
                this._activityGeoJsonConverter.WriteJson(writer, activity, serializer);
            }
            foreach (var ride in trip.Rides)
            {
                this._rideGeoJsonConverter.WriteJson(writer, ride, serializer);
            }
            foreach (var attachment in trip.Attachments)
            {
                this._attachmentGeoJsonConverter.WriteJson(writer, attachment, serializer);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Trip).IsAssignableFrom(objectType);
        }
    }
}
