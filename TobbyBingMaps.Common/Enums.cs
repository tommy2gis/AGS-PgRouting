using System;


namespace TobbyBingMaps.Common
{
    [Flags]
    public enum GeometryType : ulong
    {
        None = 0,
        Point = 1,
        LineString = 2,
        Polygon = 4,
        MultiPoint = 8,
        MultiLineString = 16,
        MultiPolygon = 32,
        GeometryCollection = 64
    }
}
