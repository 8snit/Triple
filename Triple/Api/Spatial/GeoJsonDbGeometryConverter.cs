using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using Newtonsoft.Json;

namespace Triple.Api.Spatial
{
    public class GeoJsonDbGeometryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DbGeometry);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dbGeometry = value as DbGeometry;
            writer.WritePropertyName("geometry");
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue(dbGeometry.SpatialTypeName);
            writer.WritePropertyName("coordinates");
            switch (dbGeometry.SpatialTypeName)
            {
                case "Point":
                    this.WritePoint(writer, dbGeometry);
                    break;
                case "LineString":
                    this.WriteLineString(writer, dbGeometry);
                    break;
                case "Polygon":
                    this.WritePolygon(writer, dbGeometry);
                    break;
                case "MultiPoint":
                    this.WriteMultiPoint(writer, dbGeometry);
                    break;
                case "MultiLineString":
                    this.WriteMultiLineString(writer, dbGeometry);
                    break;
                case "MultiPolygon":
                    this.WriteMultiPolygon(writer, dbGeometry);
                    break;
                case "GeometryCollection":
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
            writer.WriteEndObject();
            if (dbGeometry.CoordinateSystemId > 0)
            {
                writer.WritePropertyName("crs");
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue("name");
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                writer.WriteValue(string.Format("{0}:{1}", "EPSG", dbGeometry.CoordinateSystemId));
                writer.WriteEndObject();
                writer.WriteEndObject();
            }
        }

        private void WritePoint(JsonWriter writer, DbGeometry dbGeometry)
        {
            writer.WriteStartArray();
            writer.WriteValue(dbGeometry.XCoordinate);
            writer.WriteValue(dbGeometry.YCoordinate);
            writer.WriteEndArray();
        }

        private void WriteLineString(JsonWriter writer, DbGeometry dbGeometry)
        {
            writer.WriteStartArray();
            var pointCount = dbGeometry.PointCount ?? 0;
            for (var i = 1; i <= pointCount; i++)
            {
                writer.WriteStartArray();
                writer.WriteValue(dbGeometry.PointAt(i).XCoordinate);
                writer.WriteValue(dbGeometry.PointAt(i).YCoordinate);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        private void WritePolygon(JsonWriter writer, DbGeometry dbGeometry)
        {
            writer.WriteStartArray();
            writer.WriteStartArray();
            var exteriorPointCount = dbGeometry.ExteriorRing.ElementAt(1).PointCount ?? 0;
            for (var j = 1; j <= exteriorPointCount; j++)
            {
                writer.WriteStartArray();
                writer.WriteValue(dbGeometry.ExteriorRing.PointAt(j).XCoordinate);
                writer.WriteValue(dbGeometry.ExteriorRing.PointAt(j).YCoordinate);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            var interiorRingCount = dbGeometry.InteriorRingCount ?? 0;
            for (var i = 1; i <= interiorRingCount; i++)
            {
                writer.WriteStartArray();
                var interiorPointCount = dbGeometry.InteriorRingAt(i).PointCount ?? 0;
                for (var j = 1; j <= interiorPointCount; j++)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(dbGeometry.InteriorRingAt(i).PointAt(j).XCoordinate);
                    writer.WriteValue(dbGeometry.InteriorRingAt(i).PointAt(j).YCoordinate);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        private void WriteMultiPoint(JsonWriter writer, DbGeometry dbGeometry)
        {
            writer.WriteStartArray();
            var pointCount = dbGeometry.PointCount ?? 0;
            for (var i = 1; i <= pointCount; i++)
            {
                writer.WriteStartArray();
                writer.WriteValue(dbGeometry.PointAt(i).XCoordinate);
                writer.WriteValue(dbGeometry.PointAt(i).YCoordinate);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        private void WriteMultiLineString(JsonWriter writer, DbGeometry dbGeometry)
        {
            writer.WriteStartArray();
            var elementCount = dbGeometry.ElementCount ?? 0;
            for (var j = 1; j <= elementCount; j++)
            {
                writer.WriteStartArray();
                var pointCount = dbGeometry.ElementAt(j).PointCount ?? 0;
                for (var i = 1; i <= pointCount; i++)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(dbGeometry.ElementAt(j).PointAt(i).XCoordinate);
                    writer.WriteValue(dbGeometry.ElementAt(j).PointAt(i).YCoordinate);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        private void WriteMultiPolygon(JsonWriter writer, DbGeometry dbGeometry)
        {
            writer.WriteStartArray();
            var elementCount = dbGeometry.ElementCount ?? 0;
            for (var k = 1; k <= elementCount; k++)
            {
                writer.WriteStartArray();
                writer.WriteStartArray();
                var exteriorPointCount = dbGeometry.ElementAt(k).ExteriorRing.ElementAt(1).PointCount ?? 0;
                for (var j = 1; j <= exteriorPointCount; j++)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(dbGeometry.ElementAt(k).ExteriorRing.PointAt(j).XCoordinate);
                    writer.WriteValue(dbGeometry.ElementAt(k).ExteriorRing.PointAt(j).YCoordinate);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
                var interiorRingCount = dbGeometry.ElementAt(k).InteriorRingCount ?? 0;
                for (var i = 1; i <= interiorRingCount; i++)
                {
                    writer.WriteStartArray();
                    var interiorPointCount = dbGeometry.ElementAt(k).InteriorRingAt(i).PointCount ?? 0;
                    for (var j = 1; j <= interiorPointCount; j++)
                    {
                        writer.WriteStartArray();
                        writer.WriteValue(dbGeometry.ElementAt(k).InteriorRingAt(i).PointAt(j).XCoordinate);
                        writer.WriteValue(dbGeometry.ElementAt(k).InteriorRingAt(i).PointAt(j).YCoordinate);
                        writer.WriteEndArray();
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }
}
