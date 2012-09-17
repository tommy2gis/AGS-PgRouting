using System;

namespace TobbyBingMaps.Common
{
    public static class Constants
    {

        public const int AutoPanTolleranceToEdge = 20;
        public const int AutoPanAmount = 200;

        //See http://en.wikipedia.org/wiki/Geographic_coordinate_system.
        public const double EarthMinLatitude = -85.05112878D;
        public const double EarthMaxLatitude = 85.05112878D;
        public const double EarthMinLongitude = -180D;
        public const double EarthMaxLongitude = 180D;
        public const double EarthCircumference = EarthRadius * 2 * Math.PI;
        public const double HalfEarthCircumference = EarthCircumference / 2;
        public const double EarthRadius = 6378137;

        public const double ProjectionOffset = EarthCircumference * 0.5;

        public const double INCH_TO_METER = 0.0254D;
        public const double METER_TO_INCH = 39.3700787D;
        public const double METER_TO_MILE = 0.000621371192D;
        public const double MILE_TO_METER = 1609.344D;
    }
}
