using System;
using System.Data.Entity.Spatial;
using Microsoft.Spatial;

namespace Triple.Api.Spatial
{
    public class GeometryConvert
    {
        public static DbGeometry ConvertFrom(Geometry geometry)
        {
            var geometryEwkt = geometry.ToString();
            var semicolon = geometryEwkt.IndexOf(';');
            var geometryWkt = geometryEwkt.Substring(semicolon + 1);
            return DbGeometry.FromText(geometryWkt, int.Parse(geometry.CoordinateSystem.Id));
        }

        public static GeometryPoint ConvertPointTo(DbGeometry dbGeometry)
        {
            var lat = dbGeometry.XCoordinate ?? 0;
            var lon = dbGeometry.YCoordinate ?? 0;
            var alt = dbGeometry.Elevation;
            var m = dbGeometry.Measure;
            return GeometryPoint.Create(lat, lon, alt, m);
        }

        public static GeometryLineString ConvertLineStringTo(DbGeometry dbGeometry)
        {
            var spatialBuilder = SpatialBuilder.Create();
            var pipeLine = spatialBuilder.GeometryPipeline;
            pipeLine.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            pipeLine.BeginGeometry(SpatialType.LineString);
            var pointCount = dbGeometry.PointCount ?? 0;
            for (var n = 0; n < pointCount; n++)
            {
                var pointN = dbGeometry.PointAt(n + 1);
                var lat = pointN.XCoordinate ?? 0;
                var lon = pointN.YCoordinate ?? 0;
                var alt = pointN.Elevation;
                var m = pointN.Measure;
                var position = new GeometryPosition(lat, lon, alt, m);
                if (n == 0)
                {
                    pipeLine.BeginFigure(position);
                }
                else
                {
                    pipeLine.LineTo(position);
                }
            }
            pipeLine.EndFigure();
            pipeLine.EndGeometry();
            return (GeometryLineString) spatialBuilder.ConstructedGeometry;
        }

        public static GeometryPolygon ConvertPolygonTo(DbGeometry dbGeometry)
        {
            throw new NotImplementedException();
        }

        public static GeometryMultiPoint ConvertMultiPointTo(DbGeometry dbGeometry)
        {
            var spatialBuilder = SpatialBuilder.Create();
            var pipeLine = spatialBuilder.GeometryPipeline;
            pipeLine.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            pipeLine.BeginGeometry(SpatialType.LineString);
            var pointCount = dbGeometry.PointCount ?? 0;
            for (var n = 0; n < pointCount; n++)
            {
                var pointN = dbGeometry.PointAt(n + 1);
                var lat = pointN.XCoordinate ?? 0;
                var lon = pointN.YCoordinate ?? 0;
                var alt = pointN.Elevation;
                var m = pointN.Measure;
                var position = new GeometryPosition(lat, lon, alt, m);
                if (n == 0)
                {
                    pipeLine.BeginFigure(position);
                }
                else
                {
                    pipeLine.LineTo(position);
                }
            }
            pipeLine.EndFigure();
            pipeLine.EndGeometry();
            return (GeometryMultiPoint) spatialBuilder.ConstructedGeometry;
        }

        public static GeometryMultiLineString ConvertMultiLineStringTo(DbGeometry dbGeometry)
        {
            throw new NotImplementedException();
        }

        public static GeometryMultiPolygon ConvertMultiPolygonTo(DbGeometry dbGeometry)
        {
            throw new NotImplementedException();
        }

        public static GeometryCollection ConvertGeometryCollectionTo(DbGeometry dbGeometry)
        {
            throw new NotImplementedException();
        }
    }
}
