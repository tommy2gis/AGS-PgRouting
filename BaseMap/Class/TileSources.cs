using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;

namespace BaseMap.Class
{
    public class ChinaTileSource : TileSource
    {

        public ChinaTileSource()
            : base("http://r2.tiles.ditu.live.com/tiles/r{quadkey}.png?g=41")
        { }
    }

    public class fourleveroadsTileSourse : TileSource
    {
        // Fields
        public const int TILE_SIZE = 0x100;

        // Methods
        public fourleveroadsTileSourse()
            //: base("http://mapserver-slp.mendelu.cz/cgi-bin/mapserv?map=/var/local/slp/krtinyWMS.map&REQUEST=GetMap&SERVICE=wms&VERSION=1.1.1&SRS=EPSG:4326&WIDTH={4}&HEIGHT={4}&FORMAT=image/png&BBOX={0},{1},{2},{3}&LAYERS=typologie,hm2003&Transparent=true")
            : base("http://172.16.0.98:8080/geoserver/wms?service=WMS&version=1.1.0&request=GetMap&layers=zteitsmap:fourleveroads&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png&transparent=true")
        {
        }

        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
            int zoom = tileLevel;
            double num2 = 0;
            double num3 = 0;
            double num4 = TileToWorldPosX((double)tilePositionX, zoom) + num2;
            double num5 = TileToWorldPosY((double)tilePositionY, zoom) + num3;
            double num6 = TileToWorldPosX((double)(tilePositionX + 1), zoom) + num2;
            double num7 = TileToWorldPosY((double)(tilePositionY + 1), zoom) + num3;
            return new Uri(string.Format(base.UriFormat, new object[] { num4, num7, num6, num5, 0x100 }));
        }

        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            return (double)((float)(((tile_x / Math.Pow(2.0, (double)zoom)) * 360.0) - 180.0));
        }

        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double num = 3.1415926535897931 - ((6.2831853071795862 * tile_y) / Math.Pow(2.0, (double)zoom));
            return (double)((float)(57.295779513082323 * Math.Atan(Math.Sinh(num))));
        }

    }

    public class threeleveroadsTileSourse : TileSource
    {
        // Fields
        public const int TILE_SIZE = 0x100;

        // Methods
        public threeleveroadsTileSourse()
            //: base("http://mapserver-slp.mendelu.cz/cgi-bin/mapserv?map=/var/local/slp/krtinyWMS.map&REQUEST=GetMap&SERVICE=wms&VERSION=1.1.1&SRS=EPSG:4326&WIDTH={4}&HEIGHT={4}&FORMAT=image/png&BBOX={0},{1},{2},{3}&LAYERS=typologie,hm2003&Transparent=true")
            : base("http://172.16.0.98:8080/geoserver/wms?service=WMS&version=1.1.0&request=GetMap&layers=zteitsmap:threeleveroads&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png&transparent=true")
        {
        }

        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
            int zoom = tileLevel;
            double num2 = 0;
            double num3 = 0;
            double num4 = TileToWorldPosX((double)tilePositionX, zoom) + num2;
            double num5 = TileToWorldPosY((double)tilePositionY, zoom) + num3;
            double num6 = TileToWorldPosX((double)(tilePositionX + 1), zoom) + num2;
            double num7 = TileToWorldPosY((double)(tilePositionY + 1), zoom) + num3;
            return new Uri(string.Format(base.UriFormat, new object[] { num4, num7, num6, num5, 0x100 }));
        }

        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            return (double)((float)(((tile_x / Math.Pow(2.0, (double)zoom)) * 360.0) - 180.0));
        }

        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double num = 3.1415926535897931 - ((6.2831853071795862 * tile_y) / Math.Pow(2.0, (double)zoom));
            return (double)((float)(57.295779513082323 * Math.Atan(Math.Sinh(num))));
        }

    }

    public class twoleveroadsTileSourse : TileSource
    {
        // Fields
        public const int TILE_SIZE = 0x100;

        // Methods
        public twoleveroadsTileSourse()
            //: base("http://mapserver-slp.mendelu.cz/cgi-bin/mapserv?map=/var/local/slp/krtinyWMS.map&REQUEST=GetMap&SERVICE=wms&VERSION=1.1.1&SRS=EPSG:4326&WIDTH={4}&HEIGHT={4}&FORMAT=image/png&BBOX={0},{1},{2},{3}&LAYERS=typologie,hm2003&Transparent=true")
            : base("http://172.16.0.98:8080/geoserver/wms?service=WMS&version=1.1.0&request=GetMap&layers=zteitsmap:twoleveroads&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png&transparent=true")
        {
        }

        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
            int zoom = tileLevel;
            double num2 = 0;
            double num3 = 0;
            double num4 = TileToWorldPosX((double)tilePositionX, zoom) + num2;
            double num5 = TileToWorldPosY((double)tilePositionY, zoom) + num3;
            double num6 = TileToWorldPosX((double)(tilePositionX + 1), zoom) + num2;
            double num7 = TileToWorldPosY((double)(tilePositionY + 1), zoom) + num3;
            return new Uri(string.Format(base.UriFormat, new object[] { num4, num7, num6, num5, 0x100 }));
        }

        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            return (double)((float)(((tile_x / Math.Pow(2.0, (double)zoom)) * 360.0) - 180.0));
        }

        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double num = 3.1415926535897931 - ((6.2831853071795862 * tile_y) / Math.Pow(2.0, (double)zoom));
            return (double)((float)(57.295779513082323 * Math.Atan(Math.Sinh(num))));
        }

    }

    public class realTileSourse: TileSource
    {
        // Fields
        public const int TILE_SIZE = 0x100;
        public int laynum { get; set; }
        private int zoomleve;
        // Methods
   

        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
       
            string layer = "realtimelayer" + laynum.ToString();
            string url = "http://172.16.0.98:8080/geoserver/wms?service=WMS&version=1.1.0&request=GetMap&layers=" + layer + "&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png&transparent=true";
        
            int zoom = tileLevel;

            //if (zoomleve!=tileLevel)
            //{
            //    laynum++;
            //    if (laynum == 9)
            //    {
            //        laynum = 0;
            //    }
            //}
            //zoomleve = tileLevel;


           
            double num2 = 0;
            double num3 = 0;
            double num4 = TileToWorldPosX((double)tilePositionX, zoom) + num2;
            double num5 = TileToWorldPosY((double)tilePositionY, zoom) + num3;
            double num6 = TileToWorldPosX((double)(tilePositionX + 1), zoom) + num2;
            double num7 = TileToWorldPosY((double)(tilePositionY + 1), zoom) + num3;
            return new Uri(string.Format(url, new object[] { num4, num7, num6, num5, 0x100 }));

        }

        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            return (double)((float)(((tile_x / Math.Pow(2.0, (double)zoom)) * 360.0) - 180.0));
        }

        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double num = 3.1415926535897931 - ((6.2831853071795862 * tile_y) / Math.Pow(2.0, (double)zoom));
            return (double)((float)(57.295779513082323 * Math.Atan(Math.Sinh(num))));
        }

    }

    public class fiveleveroadsTileSourse : TileSource
    {
        // Fields
        public const int TILE_SIZE = 0x100;

        // Methods
        public fiveleveroadsTileSourse()
            //: base("http://mapserver-slp.mendelu.cz/cgi-bin/mapserv?map=/var/local/slp/krtinyWMS.map&REQUEST=GetMap&SERVICE=wms&VERSION=1.1.1&SRS=EPSG:4326&WIDTH={4}&HEIGHT={4}&FORMAT=image/png&BBOX={0},{1},{2},{3}&LAYERS=typologie,hm2003&Transparent=true")
            : base("http://172.16.0.98:8080/geoserver/wms?service=WMS&version=1.1.0&request=GetMap&layers=zteitsmap:fiveleveroads&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png&transparent=true")
            
        {
        }

        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
            int zoom = tileLevel;
            double num2 = 0;
            double num3 = 0;
            double num4 = TileToWorldPosX((double)tilePositionX, zoom) + num2;
            double num5 = TileToWorldPosY((double)tilePositionY, zoom) + num3;
            double num6 = TileToWorldPosX((double)(tilePositionX + 1), zoom) + num2;
            double num7 = TileToWorldPosY((double)(tilePositionY + 1), zoom) + num3;
            return new Uri(string.Format(base.UriFormat, new object[] { num4, num7, num6, num5, 0x100 }));
        }

        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            return (double)((float)(((tile_x / Math.Pow(2.0, (double)zoom)) * 360.0) - 180.0));
        }

        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double num = 3.1415926535897931 - ((6.2831853071795862 * tile_y) / Math.Pow(2.0, (double)zoom));
            return (double)((float)(57.295779513082323 * Math.Atan(Math.Sinh(num))));
        }

    }

    public class fiveleveroadsTileSourse2 : TileSource
    {
        // Fields
        public const int TILE_SIZE = 0x100;

        // Methods
        public fiveleveroadsTileSourse2()
            //: base("http://mapserver-slp.mendelu.cz/cgi-bin/mapserv?map=/var/local/slp/krtinyWMS.map&REQUEST=GetMap&SERVICE=wms&VERSION=1.1.1&SRS=EPSG:4326&WIDTH={4}&HEIGHT={4}&FORMAT=image/png&BBOX={0},{1},{2},{3}&LAYERS=typologie,hm2003&Transparent=true")
            : base("http://172.16.0.98:8080/geoserver/wms?service=WMS&version=1.1.0&request=GetMap&layers=zteitsmap:fiveleveroads&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png&transparent=true")
        {
        }

        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
            int zoom = tileLevel;
            double num2 = 0;
            double num3 = 0;
            double num4 = TileToWorldPosX((double)tilePositionX, zoom) + num2;
            double num5 = TileToWorldPosY((double)tilePositionY, zoom) + num3;
            double num6 = TileToWorldPosX((double)(tilePositionX + 1), zoom) + num2;
            double num7 = TileToWorldPosY((double)(tilePositionY + 1), zoom) + num3;
            return new Uri(string.Format(base.UriFormat, new object[] { num4, num7, num6, num5, 0x100 }));
        }

        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            return (double)((float)(((tile_x / Math.Pow(2.0, (double)zoom)) * 360.0) - 180.0));
        }

        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double num = 3.1415926535897931 - ((6.2831853071795862 * tile_y) / Math.Pow(2.0, (double)zoom));
            return (double)((float)(57.295779513082323 * Math.Atan(Math.Sinh(num))));
        }

    }


    public class WMSTileSourse : TileSource
    {
        // Fields
        public const int TILE_SIZE = 0x100;

        // Methods
        public WMSTileSourse()
            : base("http://172.16.0.98:8080/geoserver/wms?service=WMS&version=1.1.0&request=GetMap&layers=wuxi&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png")
            //: base("http://172.16.0.98:8080/geoserver/gwc/service/wms?service=WMS&version=1.1.0&request=GetMap&layers=wuxi&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png")
           
            
        {
        }

        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
            int zoom = tileLevel;
            double num2 = 0;
            double num3 = 0;
            double num4 = TileToWorldPosX((double)tilePositionX, zoom) + num2;
            double num5 = TileToWorldPosY((double)tilePositionY, zoom) + num3;
            double num6 = TileToWorldPosX((double)(tilePositionX + 1), zoom) + num2;
            double num7 = TileToWorldPosY((double)(tilePositionY + 1), zoom) + num3;
            return new Uri(string.Format(base.UriFormat, new object[] { num4, num7, num6, num5, 0x100 }));
        }

        public static double TileToWorldPosX(double tile_x, int zoom)
        {
            return (double)((float)(((tile_x / Math.Pow(2.0, (double)zoom)) * 360.0) - 180.0));
        }

        public static double TileToWorldPosY(double tile_y, int zoom)
        {
            double num = 3.1415926535897931 - ((6.2831853071795862 * tile_y) / Math.Pow(2.0, (double)zoom));
            return (double)((float)(57.295779513082323 * Math.Atan(Math.Sinh(num))));
        }

    }


    public class GoogleTile : TileSource
    {
        // Fields
        private const string charPhysical = "t";
        private const string charPhysicalHybrid = "p";
        private const string charSatellite = "s";
        private const string charSatelliteHybrid = "y";
        private const string charStreet = "m";
        private const string charStreetOverlay = "h";
        private const string charStreetWaterOverlay = "r";
        private GoogleMapModes MapMode = GoogleMapModes.SatelliteHybrid;
        private int server_rr = 0;
        private const string TilePathBase = "http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}";

        // Methods
        public override Uri GetUri(int tilePositionX, int tilePositionY, int tileLevel)
        {
            int zoom = tileLevel;
            string uriString = string.Empty;
            this.server_rr = (this.server_rr + 1) % 4;
            switch (this.MapMode)
            {
                case GoogleMapModes.Street:
                    uriString = XYZUrl("http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}", this.server_rr, "m", zoom, tilePositionX, tilePositionY);
                    break;

                case GoogleMapModes.Satellite:
                    uriString = XYZUrl("http://mt{0}.google.cn/vt/lyrs={1}@83&gl=cn&z={2}&x={3}&y={4}&s=Galileo", this.server_rr, "s", zoom, tilePositionX, tilePositionY);
                    break;

                case GoogleMapModes.SatelliteHybrid:
                    uriString = XYZUrl("http://mt{0}.google.cn/vt/lyrs={1}&gl=cn&z={2}&x={3}&y={4}", this.server_rr, "y", zoom, tilePositionX, tilePositionY);
                    break;

                case GoogleMapModes.Physical:
                    uriString = XYZUrl("http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}", this.server_rr, "t", zoom, tilePositionX, tilePositionY);
                    break;

                case GoogleMapModes.PhysicalHybrid:
                    uriString = XYZUrl("http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}", this.server_rr, "p", zoom, tilePositionX, tilePositionY);
                    break;

                case GoogleMapModes.StreetOverlay:
                    uriString = XYZUrl("http://mt{0}.google.cn/vt/imgtp=png32&lyrs={1}@151000000&hl=zh-cn&gl=cn&z={2}&x={3}&y={4}", this.server_rr, "h", zoom, tilePositionX, tilePositionY);
                    break;

                case GoogleMapModes.StreetWaterOverlay:
                    uriString = XYZUrl("http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}", this.server_rr, "r", zoom, tilePositionX, tilePositionY);
                    break;
            }
            return new Uri(uriString);
        }

        private static string XYZUrl(string url, int server, string mapmode, int zoom, int tilePositionX, int tilePositionY)
        {
            url = string.Format(url, new object[] { server, mapmode, zoom, tilePositionX, tilePositionY });
            return url;
        }
        public enum GoogleMapModes
        {
            Street,
            Satellite,
            SatelliteHybrid,
            Physical,
            PhysicalHybrid,
            StreetOverlay,
            StreetWaterOverlay
        }
    }


    public class OpenStreetMapTileSource : TileSource
    {
        // Methods
        public OpenStreetMapTileSource()
            : base("http://tile.openstreetmap.org/{2}/{0}/{1}.png")
        {
        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            return new Uri(string.Format(base.UriFormat, x, y, zoomLevel));
        }
    }

    public class WXTileSource : TileSource
    {
        // Methods
        public WXTileSource()
            : base("http://172.16.0.98:8080/geoserver/gwc/service/wms?service=WMS&version=1.1.0&request=GetMap&layers=wuxi&styles=&BBox={0},{1},{2},{3}&Width={4}&Height={4}&srs=EPSG:4326&format=image/png")
        {
        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            return new Uri(string.Format(base.UriFormat, x, y, zoomLevel));
        }
    }

}
