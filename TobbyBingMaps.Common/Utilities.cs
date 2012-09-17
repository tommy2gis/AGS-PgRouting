using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Media;
using System.Xml.Linq;
using TobbyBingMaps.Common.Entities;
using Microsoft.Maps.MapControl.Core;
using Location = Microsoft.Maps.MapControl.Location;
using Point = System.Windows.Point;


namespace TobbyBingMaps.Common
{
    public static class Utilities
    {
        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
                    if (controlName == name)
                    {
                        return child as T;
                    }
                    var result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        public static List<T> FindAllVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            List<T> resultList = new List<T>();
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
                    if (controlName == name)
                    {
                        resultList.Add(child as T);
                    }
                    var result = FindAllVisualChildByName<T>(child, name);
                    if (result != null)
                        resultList.AddRange(result);
                }
            }
            return resultList;
        }

        public static T GetParentInstance<T>(FrameworkElement element) where T : FrameworkElement
        {
            var parent = element;
            while (parent != null && parent is T == false)
            {
                parent = (FrameworkElement)VisualTreeHelper.GetParent(parent);
            }
            if (parent != null)
            {
                return parent as T;
            }
            return null;
        }

        public static bool IsDesignTime()
        {
            try
            {
                return DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual);
            }
            catch
            {
                return false;
            }
        }

        public static double GetScaleAtZoomLevel(double latitude, double zoomLevel)
        {
            double mapWidth = Math.Pow(2, zoomLevel + 8);
            double scale = (Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * Constants.EarthRadius / mapWidth) *
                           (GetScreenDPI() / Constants.INCH_TO_METER);
            return scale;
        }

        public static double GetZoomLevelAtScale(double latitude, double scale)
        {
            double zoomlevel = Math.Log((Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * Constants.EarthRadius) * (GetScreenDPI() / Constants.INCH_TO_METER) / scale, 2) - 8;

            return zoomlevel;
        }

        public static double GetScreenDPI()
        {
            if (HtmlPage.IsEnabled)
            {
                var screen = HtmlPage.Window.GetProperty("screen") as ScriptObject;
                if (screen != null)
                {
                    object o = screen.GetProperty("deviceYDPI");
                    if (o != null)
                    {
                        return (double)o;
                    }
                }
            }
            return 96;
        }

        public static Color ColorFromHexString(string HexColor)
        {
            try
            {
                //The input at this point could be HexColor = "FF00FF1F" 
                byte Alpha = byte.Parse(HexColor.Substring(0, 2), NumberStyles.HexNumber);
                byte Red = byte.Parse(HexColor.Substring(2, 2), NumberStyles.HexNumber);
                byte Green = byte.Parse(HexColor.Substring(4, 2), NumberStyles.HexNumber);
                byte Blue = byte.Parse(HexColor.Substring(6, 2), NumberStyles.HexNumber);
                return Color.FromArgb(Alpha, Red, Green, Blue);
            }
            catch (Exception)
            {
                return Colors.Black;
            }
        }

        public static string ColorToHexString(Color color)
        {
            return color.ToString().Replace("#", "");
        }

        public static string InvertColorFromHexString(string HexColor)
        {
            var c = ColorFromHexString(HexColor);
            //invert RGB, keep Alpha
            c.R = (byte)~c.R;
            c.G = (byte)~c.G;
            c.B = (byte)~c.B;
            return ColorToHexString(c);
        }

        public static GeometryType GetTypemask(GeometryType typemask, GeometryType single, GeometryType plural)
        {
            if ((typemask & single) > 0)
            {
                typemask = typemask | plural;
            }
            else if (typemask == GeometryType.None)
            {
                typemask = typemask | single;
            }
            else
            {
                typemask = typemask | GeometryType.GeometryCollection;
            }
            return typemask;
        }

        public static Dictionary<string, StyleSpecification> ProcessStyleXML(string xml)
        {
            var styles = new Dictionary<string, StyleSpecification>();
            XDocument doc = XDocument.Parse(xml);
            //parse Styles into style structure
            List<StyleSpecification> styleSpecifications = (doc.Descendants("Style").Select(
                xmlstyles => new StyleSpecification
                {
                    // ReSharper disable PossibleNullReferenceException
                    ID = xmlstyles.Attribute("id").Value,
                    IconScale =
                        (xmlstyles.Element("IconStyle") != null &&
                         xmlstyles.Element("IconStyle").Element("scale") != null)
                            ? Convert.ToDouble(
                                  xmlstyles.Element("IconStyle").Element("scale").Value)
                            : 0,
                    IconURL =
                        (xmlstyles.Element("IconStyle") != null &&
                         xmlstyles.Element("IconStyle").Element("Icon") != null &&
                         xmlstyles.Element("IconStyle").Element("Icon").Element("href") != null)
                            ? xmlstyles.Element("IconStyle").Element("Icon").Element("href").
                                  Value
                            : string.Empty,
                    IconOffsetX =
                        (xmlstyles.Element("IconStyle") != null &&
                         xmlstyles.Element("IconStyle").Element("IconOffsetX") != null)
                            ? Convert.ToDouble(
                                  xmlstyles.Element("IconStyle").Element("IconOffsetX").Value)
                            : 0,
                    IconOffsetY =
                        (xmlstyles.Element("IconStyle") != null &&
                         xmlstyles.Element("IconStyle").Element("IconOffsetY") != null)
                            ? Convert.ToDouble(
                                  xmlstyles.Element("IconStyle").Element("IconOffsetY").Value)
                            : 0,
                    LineColour =
                        (xmlstyles.Element("LineStyle") != null &&
                         xmlstyles.Element("LineStyle").Element("color") != null)
                            ? xmlstyles.Element("LineStyle").Element("color").Value
                            : string.Empty,
                    LineWidth =
                        (xmlstyles.Element("LineStyle") != null &&
                         xmlstyles.Element("LineStyle").Element("width") != null)
                            ? Convert.ToDouble(
                                  xmlstyles.Element("LineStyle").Element("width").Value)
                            : 0,
                    PolyFillColour =
                        (xmlstyles.Element("PolyStyle") != null &&
                         xmlstyles.Element("PolyStyle").Element("color") != null)
                            ? xmlstyles.Element("PolyStyle").Element("color").Value
                            : string.Empty,
                    PolygonLineColour =
                         (xmlstyles.Element("PolyStyle") != null &&
                          xmlstyles.Element("PolyStyle").Element("LineStyle") != null &&
                          xmlstyles.Element("PolyStyle").Element("LineStyle").Element("color") != null)
                             ? xmlstyles.Element("PolyStyle").Element("LineStyle").Element("color").Value
                             : string.Empty,
                    PolygonLineWidth =
                        (xmlstyles.Element("PolyStyle") != null &&
                         xmlstyles.Element("PolyStyle").Element("LineStyle") != null &&
                         xmlstyles.Element("PolyStyle").Element("LineStyle").Element("width") != null)
                            ? Convert.ToDouble(
                                  xmlstyles.Element("PolyStyle").Element("LineStyle").Element("width").Value)
                            : 0,
                    ShowFill =
                        (xmlstyles.Element("PolyStyle") != null &&
                         xmlstyles.Element("PolyStyle").Element("fill") != null)
                            ? (xmlstyles.Element("PolyStyle").Element("fill").Value == "1")
                            : true,
                    ShowLine =
                        (xmlstyles.Element("PolyStyle") != null &&
                         xmlstyles.Element("PolyStyle").Element("outline") != null)
                            ? (xmlstyles.Element("PolyStyle").Element("outline").Value == "1")
                            : true,
                    Frames =
                        (xmlstyles.Element("AnimationStyle") != null &&
                         xmlstyles.Element("AnimationStyle").Element("frames") != null)
                            ? Convert.ToInt32(
                                  xmlstyles.Element("AnimationStyle").Element("frames").Value)
                            : 1,
                    FrameInterval =
                        (xmlstyles.Element("AnimationStyle") != null &&
                         xmlstyles.Element("AnimationStyle").Element("frameinterval") != null)
                            ? Convert.ToInt32(
                                  xmlstyles.Element("AnimationStyle").Element("frameinterval").
                                      Value)
                            : 500,
                    Width =
                        (xmlstyles.Element("AnimationStyle") != null &&
                         xmlstyles.Element("AnimationStyle").Element("width") != null)
                            ? Convert.ToInt32(
                                  xmlstyles.Element("AnimationStyle").Element("width")
                                      .Value)
                            : 5000,
                    Height =
                        (xmlstyles.Element("AnimationStyle") != null &&
                         xmlstyles.Element("AnimationStyle").Element("height") !=
                         null)
                            ? Convert.ToInt32(
                                  xmlstyles.Element("AnimationStyle").Element("height")
                                        .Value)
                            : 500,
                    // ReSharper restore PossibleNullReferenceException
                })).ToList();
            foreach (StyleSpecification specification in styleSpecifications)
            {
                styles.Add(specification.ID, specification);
            }
            return styles;
        }

        public static string DDMMSSFromDecimal(double decimaldegrees)
        {
            double d = Math.Abs(decimaldegrees);
            d += 1.3888888888888888888888888888889e-4;  // add ½ second for rounding  
            var deg = Math.Floor(d);
            var min = Math.Floor((d - deg) * 60);
            var sec = Math.Floor((d - deg - min / 60) * 3600);

            // add leading zeros if required  
            return deg.ToString("000") + '\u00B0' + min.ToString("00") + '\u2032' + sec.ToString("00") + '\u2033';
        }

        public static Location GetMidLocation(Location loc1, Location loc2)
        {
            //TODO: proper conversion.
            return new Location((loc1.Latitude + loc2.Latitude) / 2, (loc1.Longitude + loc2.Longitude) / 2);
        }

        public static void AutoPan(Point pos, MapCore map)
        {
            Point center = map.LocationToViewportPoint(map.Center);
            if (pos.X < Constants.AutoPanTolleranceToEdge)
            {
                center.X = center.X - Constants.AutoPanAmount;
                map.Center = map.ViewportPointToLocation(center);
            }
            else if (pos.X > (map.ViewportSize.Width - Constants.AutoPanTolleranceToEdge))
            {
                center.X = center.X + Constants.AutoPanAmount;
                map.Center = map.ViewportPointToLocation(center);
            }
            if (pos.Y < Constants.AutoPanTolleranceToEdge)
            {
                center.Y = center.Y - Constants.AutoPanAmount;
                map.Center = map.ViewportPointToLocation(center);
            }
            else if (pos.Y > (map.ViewportSize.Height - Constants.AutoPanTolleranceToEdge))
            {
                center.Y = center.Y + Constants.AutoPanAmount;
                map.Center = map.ViewportPointToLocation(center);
            }
        }

        //public static GisSharpBlog.NetTopologySuite.Geometries.Geometry GetViewPort(MapCore map)
        //{
        //    var coordinates = new ICoordinate[5];
        //    coordinates[0] = CoordinateConvertor.Convert(map.ViewportPointToLocation(new Point(0, 0)));
        //    coordinates[1] = CoordinateConvertor.Convert(map.ViewportPointToLocation(new Point(0, map.ViewportSize.Height)));
        //    coordinates[2] = CoordinateConvertor.Convert(map.ViewportPointToLocation(new Point(map.ViewportSize.Width, map.ViewportSize.Height)));
        //    coordinates[3] = CoordinateConvertor.Convert(map.ViewportPointToLocation(new Point(map.ViewportSize.Width, 0)));
        //    coordinates[4] = coordinates[0];

        //    //if we cross the internation date line (-180/180) it is drawing the polygon the wrong way
        //    //split into two
        //    if (coordinates[0].X > coordinates[2].X)
        //    {
        //        var coordinatesPoly1 = new ICoordinate[5];
        //        coordinatesPoly1[0] = coordinates[0];
        //        coordinatesPoly1[1] = coordinates[1];
        //        coordinatesPoly1[2] = new Coordinate(180, coordinates[2].Y);
        //        coordinatesPoly1[3] = new Coordinate(180, coordinates[3].Y);
        //        coordinatesPoly1[4] = coordinatesPoly1[0];

        //        var coordinatesPoly2 = new ICoordinate[5];
        //        coordinatesPoly2[0] = new Coordinate(-180, coordinates[0].Y);
        //        coordinatesPoly2[1] = new Coordinate(-180, coordinates[1].Y);
        //        coordinatesPoly2[2] = coordinates[2];
        //        coordinatesPoly2[3] = coordinates[3];
        //        coordinatesPoly2[4] = coordinatesPoly2[0];

        //        var polygons = new IPolygon[2];
        //        polygons[0] = new Polygon(new LinearRing(coordinatesPoly1));
        //        polygons[1] = new Polygon(new LinearRing(coordinatesPoly2));

        //        return new MultiPolygon(polygons);
        //    }

        //    return new Polygon(new LinearRing(coordinates));
        //}

        public static void Pan(double deltaX, double deltaY, MapCore map)
        {
            Point center = map.LocationToViewportPoint(map.Center);
            center.X = center.X + deltaX;
            center.Y = center.Y + deltaY;
            map.Center = map.ViewportPointToLocation(center);
        }

        public static double GetSimplificationDistance(double zoomLevel)
        {
            return (1 / (Math.Pow(2, zoomLevel - 1))) * 2;
        }
    }
}
