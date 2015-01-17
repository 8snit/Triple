using System.Data.Entity.Spatial;
using Microsoft.Spatial;

namespace Triple.Api.Spatial
{
    public class GeometryWrapper
    {
        private readonly DbGeometry _dbGeometry;

        protected GeometryWrapper(DbGeometry dbGeometry)
        {
            this._dbGeometry = dbGeometry;
        }

        public static implicit operator GeometryPoint(GeometryWrapper wrapper)
        {
            return GeometryConvert.ConvertPointTo(wrapper._dbGeometry);
        }

        public static implicit operator GeometryWrapper(GeometryPoint point)
        {
            return new GeometryWrapper(GeometryConvert.ConvertFrom(point));
        }

        public static implicit operator GeometryLineString(GeometryWrapper wrapper)
        {
            return GeometryConvert.ConvertLineStringTo(wrapper._dbGeometry);
        }

        public static implicit operator GeometryWrapper(GeometryLineString lineString)
        {
            return new GeometryWrapper(GeometryConvert.ConvertFrom(lineString));
        }

        public static implicit operator GeometryPolygon(GeometryWrapper wrapper)
        {
            return GeometryConvert.ConvertPolygonTo(wrapper._dbGeometry);
        }

        public static implicit operator GeometryWrapper(GeometryPolygon polygon)
        {
            return new GeometryWrapper(GeometryConvert.ConvertFrom(polygon));
        }

        public static implicit operator GeometryMultiPoint(GeometryWrapper wrapper)
        {
            return GeometryConvert.ConvertMultiPointTo(wrapper._dbGeometry);
        }

        public static implicit operator GeometryWrapper(GeometryMultiPoint multiPoint)
        {
            return new GeometryWrapper(GeometryConvert.ConvertFrom(multiPoint));
        }

        public static implicit operator GeometryMultiLineString(GeometryWrapper wrapper)
        {
            return GeometryConvert.ConvertMultiLineStringTo(wrapper._dbGeometry);
        }

        public static implicit operator GeometryWrapper(GeometryMultiLineString multiLineString)
        {
            return new GeometryWrapper(GeometryConvert.ConvertFrom(multiLineString));
        }

        public static implicit operator GeometryMultiPolygon(GeometryWrapper wrapper)
        {
            return GeometryConvert.ConvertMultiPolygonTo(wrapper._dbGeometry);
        }

        public static implicit operator GeometryWrapper(GeometryMultiPolygon multiPolygon)
        {
            return new GeometryWrapper(GeometryConvert.ConvertFrom(multiPolygon));
        }

        public static implicit operator GeometryCollection(GeometryWrapper wrapper)
        {
            return GeometryConvert.ConvertGeometryCollectionTo(wrapper._dbGeometry);
        }

        public static implicit operator GeometryWrapper(GeometryCollection geometryCollection)
        {
            return new GeometryWrapper(GeometryConvert.ConvertFrom(geometryCollection));
        }

        public static implicit operator DbGeometry(GeometryWrapper wrapper)
        {
            return wrapper._dbGeometry;
        }

        public static implicit operator GeometryWrapper(DbGeometry dbGeometry)
        {
            return new GeometryWrapper(dbGeometry);
        }
    }
}
