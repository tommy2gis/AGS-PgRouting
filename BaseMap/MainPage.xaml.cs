using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = GisSharpBlog.NetTopologySuite.Geometries.Point;
using GisSharpBlog.NetTopologySuite.IO;
using GisSharpBlog.NetTopologySuite.Geometries;
using Microsoft.Maps.MapControl;
using System.Windows.Markup;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using TobbyBingMaps.MapGeometry;
using TobbyBingMaps.Common.Entities;
using BaseMap.Class;
using BaseMap.RoutingServiceReference;
using System.Text;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.Core;



namespace BaseMap
{
    public partial class MainPage : UserControl
    {
        LocationCollection carloc=new LocationCollection();
        WMSTileSourse wms;
        MapTileLayer mtl = new MapTileLayer();
        MapLayer ml = new MapLayer();
        MapLayer realml = new MapLayer();
        MapLayer carml = new MapLayer();
        bool isbegin = false;
        bool islast = false;
        bool isbarrier = false;
        bool isgetloc = false;
        bool isrealroadupdated = true;
        int begin;
        int last;
        int barrierS;
        int barrierT;
        int bn = 1;
        int cari = 0;
        private EnhancedMapLayer layer;
        realTileSourse realwms = new realTileSourse();
        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }



        public void getrouting(string layer,int begin,int last)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetRoutingCompleted += new EventHandler<GetRoutingCompletedEventArgs>(rsc_GetRoutingCompleted);
            rsc.GetRoutingAsync(layer,begin,last);
        }

        public  void Getrealtimeroad(string layer,string leve)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.QueryRealRoadsCompleted+=new EventHandler<QueryRealRoadsCompletedEventArgs>(rsc_QueryRealRoadsCompleted);
            rsc.QueryRealRoadsAsync(layer,leve);
        }

        void rsc_QueryRealRoadsCompleted(object sender, QueryRealRoadsCompletedEventArgs e)
        {
            MapLayer layer = (MapLayer)this.ctlMap.FindName("realroads");
            var builder = new StringBuilder();
            if (e.Error==null)
            {
                 Color color = Colors.Blue;
                for (int i = 0; i < e.Result.Count; i++)
                {
                    string[] strArray = e.Result[i].Split(new char[] { ';' });

                    builder.Length = 0;
                    int index = 2;
                    while (index < (strArray.Length - 1))
                    {
                        builder.Append(strArray[index] + ": ");
                        index++;
                    }
                    string[] strArray2 =  strArray[strArray.Length - 1].Split(new char[] { ',' });
                
                    if ((strArray2.Length == 1) && (strArray2[0].Length > 1))
                    {
             
                    }
                    else
                    {
                        LocationCollection locations;
                        Location location2;
                       
                        if (strArray2[0].Equals(strArray2[strArray2.Length - 1]))
                        {
                            var polygon = new MapPolygon
                                              {
                                                  Stroke = new SolidColorBrush(color),
                                                  StrokeThickness = 0.5,
                                                  Fill = new SolidColorBrush(color),
                                                  Opacity = 0.75
                                              };
                            locations = new LocationCollection();
                            index = 0;
                            while (index < strArray2.Length)
                            {
                                if (strArray2[index].IndexOf(" ") != -1)
                                {
                                    location2 = new Location(double.Parse(strArray2[index].Split(new char[] {' '})[0]),
                                                             double.Parse(strArray2[index].Split(new char[] {' '})[1]));
                                    locations.Add(location2);
                                }
                                index++;
                            }
                            polygon.Locations = locations;
                            ToolTipService.SetToolTip(polygon, builder.ToString());
                            layer.Children.Add(polygon);
                        }
                        else
                        {
                            var polyline = new MapPolyline {Stroke = new SolidColorBrush(color), StrokeThickness = 6.0};
                            var doubles = new DoubleCollection {3.0, 3.0};
                            polyline.StrokeDashArray = doubles;
                            locations = new LocationCollection();

                            for (index = strArray2.Length - 1; index >= 0; index--)
                            {
                                location2 = new Location(double.Parse(strArray2[index].Split(new char[] {' '})[0]),
                                                         double.Parse(strArray2[index].Split(new char[] {' '})[1]));

                                locations.Add(location2);
                               

                            }

                            polyline.Locations = locations;
                            ToolTipService.SetToolTip(polyline, builder.ToString());

                            layer.Children.Add(polyline);


                        }
                    }
                }
            }
        }

        void rsc_GetRoutingCompleted(object sender, GetRoutingCompletedEventArgs e)
        {

            carloc.Clear();
             cari = 0;
            MapLayer layer = (MapLayer)this.ctlMap.FindName("routing");
            layer.Children.Clear();
            var builder = new StringBuilder();
            if (e.Error == null)
            {
                Color color = Colors.Blue;
                for (int i = 0; i < e.Result.Count; i++)
                {
                    string[] strArray = e.Result[i].Split(new char[] { ';' });
                    
               
                    builder.Length = 0;
                    int index = 2;
                    while (index < (strArray.Length - 1))
                    {
                        builder.Append(strArray[index] + ": ");
                        index++;
                    }
                    string[] strArray2 =  strArray[strArray.Length - 1].Split(new char[] { ',' });
                    string[] strArrayPoints= strArray[strArray.Length - 2].Split(new char[] { ',' });
                    if ((strArray2.Length == 1) && (strArray2[0].Length > 1))
                    {
                        //Ellipse dependencyObject = new Ellipse();
                        //dependencyObject.Width=10.0;
                        //dependencyObject.Height=(10.0);
                        //dependencyObject.Fill=(new SolidColorBrush(color));
                        //dependencyObject.Opacity=(0.65);
                        //Location location = new Location(double.Parse(strArray2[0].Split(new char[] { ' ' })[0]), double.Parse(strArray2[0].Split(new char[] { ' ' })[1]));
                        //MapLayer.SetPosition(dependencyObject, location);
                        //ToolTipService.SetToolTip(dependencyObject, builder.ToString());
                        //layer.Children.Add(dependencyObject);
                    }
                    else
                    {
                        LocationCollection locations;
                        Location location2;
                        Location locPoint;
                        if (strArray2[0].Equals(strArray2[strArray2.Length - 1]))
                        {
                            var polygon = new MapPolygon
                                              {
                                                  Stroke = new SolidColorBrush(color),
                                                  StrokeThickness = 2.0,
                                                  Fill = new SolidColorBrush(color),
                                                  Opacity = 0.75
                                              };
                            locations = new LocationCollection();
                            index = 0;
                            while (index < strArray2.Length)
                            {
                                if (strArray2[index].IndexOf(" ") != -1)
                                {
                                    location2 = new Location(double.Parse(strArray2[index].Split(new char[] { ' ' })[0]), double.Parse(strArray2[index].Split(new char[] { ' ' })[1]));
                                    locations.Add(location2);
                                }
                                index++;
                            }
                            polygon.Locations = locations;
                            ToolTipService.SetToolTip(polygon, builder.ToString());
                            layer.Children.Add(polygon);
                        }
                        else
                        {
                            var polyline = new MapPolyline {Stroke = new SolidColorBrush(color), StrokeThickness = 6.0};
                            var doubles = new DoubleCollection {3.0, 3.0};
                            polyline.StrokeDashArray = doubles;
                            locations = new LocationCollection();
                           
                            for (index = strArray2.Length-1; index >=0; index--)
                            {
                                location2 = new Location(double.Parse(strArray2[index].Split(new char[] { ' ' })[0]), double.Parse(strArray2[index].Split(new char[] { ' ' })[1]));
                             
                                  locations.Add(location2);
                                  if (i==0)
                                  {
                                         carloc.Add(location2);

                                  }
                               
                            }


                           
                            //添加坐标到小车轨迹
                            if (i>0)
                            {
                                Location loc1 = new Location(double.Parse(strArrayPoints[0].Split(new char[] { ' ' })[0]), double.Parse(strArrayPoints[0].Split(new char[] { ' ' })[1]));
                                Location loc2 = new Location(double.Parse(strArrayPoints[strArrayPoints.Length - 1].Split(new char[] { ' ' })[0]), double.Parse(strArrayPoints[strArrayPoints.Length - 1].Split(new char[] { ' ' })[1]));
                                if (IsCloset(carloc.ElementAt(carloc.Count - 1), loc1, loc2))
                                {
                                    for (index = strArrayPoints.Length - 1; index >= 0; index--)
                                    {
                                        var tmoloc1 = new Location(double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[0]), double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[1]));

                                        carloc.Add(tmoloc1);


                                    }
                                }
                                else
                                {
                                    for (index = 0; index < strArrayPoints.Length - 1; index++)
                                    {
                                        var tmoloc = new Location(double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[0]), double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[1]));
                                        carloc.Add(tmoloc);
                                    }
                                }
                            }
                              


                          
                            polyline.Locations = locations;
                            ToolTipService.SetToolTip(polyline, builder.ToString());
                           
                            layer.Children.Add(polyline);
                           
                           
                        }


                    }

                }
               
            }
            
            
           
            CustomPushpin otmpcp = GetCarByMSID("car", carml);

            for (int i = 0; i < carloc.Count; i++)
            {
                var item = carloc.ElementAt(i);
                if (item.Latitude == otmpcp.Location.Latitude && item.Longitude == otmpcp.Location.Longitude)
                {

                    for (int y = 0; y < i + 1; y++)
                    {
                        if (carloc.Count > y+1)
                        {
                            carloc.RemoveAt(y);
                        }
                    }
                   
                }
            }
            carloc.RemoveAt(0);
            isgetloc = false;
         
            timer.Begin();
            //RouteLoadingPanel.Stop();
        }

        private bool IsCloset(Location loc,Location loc1,Location loc2)
        {
            bool isclose=false;
            double x = Math.Pow((loc.Latitude - loc1.Latitude), 2) + Math.Pow((loc.Longitude - loc1.Longitude), 2);
            double y= Math.Pow((loc.Latitude - loc2.Latitude), 2) + Math.Pow((loc.Longitude - loc2.Longitude), 2);
            if (x>y)
            {
                isclose= true;
            }
            return isclose;
        }



        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Duration = new TimeSpan(0, 0, 0, 0,500); 
            Dtimer.Duration = new TimeSpan(0, 0, 0, 20);
            Ltimer.Duration = new TimeSpan(0, 0, 0, 55);
            //Ltimer.Begin();
            ml.Name = "routing";
            realml.Name = "realroads";
            this.ctlMap.Mode = new MercatorMode();
            wms = new WMSTileSourse();
            mtl.CacheMode = null;
            mtl.TileSources.Add(wms);
            var fiverealwms = new fiveleveroadsTileSourse();
            var fourRealWms = new fourleveroadsTileSourse();
            var threerealwms = new threeleveroadsTileSourse();
            var twoRealWms = new twoleveroadsTileSourse();


        
            realwms.laynum = 0;
            mtl.TileSources.Add(realwms);
            //mtl.TileSources.Add(fiverealwms);
            //mtl.TileSources.Add(fourRealWms);
            //mtl.TileSources.Add(threerealwms);
            //mtl.TileSources.Add(twoRealWms);


            ctlMap.Children.Add(mtl);
            ctlMap.Center = new Location(31.64, 120.31);
            ctlMap.ZoomLevel = 10;

            var styles = new Dictionary<string, StyleSpecification>
                             {
                                 {
                                     "defaultstyle", new StyleSpecification
                                                         {
                                                             ID = "style1",
                                                             LineColour = "FF1B0AA5",
                                                             LineWidth = 2,
                                                             PolyFillColour = "88677E1E",
                                                             ShowFill = true,
                                                             ShowLine = true,
                                                             IconURL = "http://soulsolutions.com.au/Images/pin.png",
                                                             IconScale = 1,
                                                             //IconOffsetX = 70, //35.41666666666667,
                                                             //IconOffsetY = -90 //-45.83333333333333
                                                         }
                                     }
                             };

            layer = new EnhancedMapLayer(ctlMap)
                        {
                            Styles = styles,
                            LayerDefinition = new LayerDefinition
                                                  {
                                                      CurrentVersion = DateTime.Now,
                                                      IsEditable = false,
                                                      LabelOn = true,
                                                      LayerAlias = "Sample Layer",
                                                      LayerID = "1",
                                                      LayerStyleName = "style3",
                                                      LayerTimeout = -1,
                                                      LayerType = 1,
                                                      MaxDisplayLevel = 100,
                                                      MBR = new byte[0],
                                                      MinDisplayLevel = 1,
                                                      PermissionToEdit = false,
                                                      Selected = true,
                                                      Tags = "Test Group",
                                                      ZIndex = 30,
                                                      Temporal = true,
                                                      IconURI = "http://soulsolutions.com.au/Images/pin.png",
                                                      ObjectAttributes =
                                                          new Dictionary<int, LayerObjectAttributeDefinition>()
                                                  },
                            EnableBalloon = false,
                            EnableClustering = true,
                            ID = "routing",
                        };
            ctlMap.Children.Add(layer);
            ctlMap.Children.Add(ml);
            //ctlMap.Children.Add(realml);
            ctlMap.Children.Add(carml);
            //Getrealtimeroad("11", "3");
            InitCar();
        }

        private void ctlMap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Point viewportPoint = e.GetPosition(ctlMap);
            Location location;
            if (ctlMap.TryViewportPointToLocation(viewportPoint, out location))
            {
                Coords.Text = String.Format("坐标: {0:f6},{1:f6}", location.Longitude, location.Latitude);
            }

        }


       

        private void ctlMap_MouseClick(object sender, MapMouseEventArgs e)
        {
          
            if (isbegin==true)
            {
                BaseGeometry bg = GetItemByID("routing", "begin");

                if (bg != null)
                {
                    LayerReset();
                }

              
                    var data = new ObservableCollection<VectorLayerData>();
                    var point = new Point(ctlMap.ViewportPointToLocation(e.ViewportPoint).Longitude, ctlMap.ViewportPointToLocation(e.ViewportPoint).Latitude);
                    data.Add(new VectorLayerData
                    {
                        Geo = point.AsBinary(),
                        ID = "begin",
                        Label = "起点",

                    });

                    layer.Add(data);
               
                isbegin = false;
            }

            if (islast==true)
            {
                 var data = new ObservableCollection<VectorLayerData>();
                var point = new Point(ctlMap.ViewportPointToLocation(e.ViewportPoint).Longitude, ctlMap.ViewportPointToLocation(e.ViewportPoint).Latitude);
                data.Add(new VectorLayerData
                {
                    Geo = point.AsBinary(),
                    ID = "last",
                    Label = "终点",
                  
                });

                layer.Add(data);


                DraggablePushpin dp = GetDCarByMSID("drapcar", carml);
                if (dp != null)
                {
                    GetBegionRoad(dp.Location.Latitude, dp.Location.Longitude, 100);
                }
                Dtimer.Begin();
                Ltimer.Begin();
                islast = false;
            }

            if (isbarrier==true)
            {
                 //初始化一个图标
                
                var data = new ObservableCollection<VectorLayerData>();
                var point = new Point(ctlMap.ViewportPointToLocation(e.ViewportPoint).Longitude, ctlMap.ViewportPointToLocation(e.ViewportPoint).Latitude);
                GetBarrier(point.X, point.Y, 30);
                data.Add(new VectorLayerData
                {
                    Geo = point.AsBinary(),
                    ID = "barrier",
                    Label = "障碍"+bn.ToString(),
                  });

                layer.Add(data);

                isbarrier = false;
                bn++;


            }
           
        }

        private void LayerReset()
        {
            updatePassableToT();
            MapLayer ml = GetLayerByID("routing", ctlMap);
            ml.Children.Clear();
            EnhancedMapLayer eml = GetELayerByID("routing", ctlMap);
            eml.Clear();
        }

        private void btnbegin_Click(object sender, RoutedEventArgs e)
        {
            isbegin = true;
        }

        private void btnlast_Click(object sender, RoutedEventArgs e)
        {
            islast = true;
        }

        private void btnbarrier_Click(object sender, RoutedEventArgs e)
        {
            isbarrier = true;
        }

        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            RouteLoadingPanel.Begin();
            BaseGeometry bg = GetItemByID("routing", "begin");
            GetBegionRoad(bg.Geometry.Coordinate.Y, bg.Geometry.Coordinate.X, 100);         
            
        }




          public void updatePassableToT()
        {
            RoutingWebServiceSoapClient rsc = new RoutingWebServiceSoapClient();
            rsc.updatePassableToTCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(rsc_updatePassableToTCompleted);
            rsc.updatePassableToTAsync();
          }

          void rsc_updatePassableToTCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
          {
             
          }


        public void UpdatePassable(int source, int target, int passable)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.UpdatePassableCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(rsc_UpdatePassableCompleted);
            rsc.UpdatePassableAsync(source, target,passable);
        }

        void rsc_UpdatePassableCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
           
        }

        public void GetLastRoad(double lg, double lt, double ditMini)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetLastRoadCompleted += new EventHandler<GetLastRoadCompletedEventArgs>(rsc_GetLastRoadCompleted);
            rsc.GetLastRoadAsync(lg, lt, ditMini);
        }

        void rsc_GetLastRoadCompleted(object sender, GetLastRoadCompletedEventArgs e)
        {
            string[] strArray = e.Result[0].Split(new char[] { ',' });
            last = int.Parse(strArray[2]);
            
            getrouting("routing", begin, last);
        }


        public void GetBegionRoad(double lg, double lt, double ditMini)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetBeginRoadCompleted += new EventHandler<GetBeginRoadCompletedEventArgs>(rsc_GetBeginRoadCompleted);
           
            rsc.GetBeginRoadAsync(lg, lt, ditMini);
            
        }

        public void GetBarrier(double lg, double lt, double ditMini)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetBarrierRoadCompleted += new EventHandler<GetBarrierRoadCompletedEventArgs>(rsc_GetBarrierRoadCompleted);
            rsc.GetBarrierRoadAsync(lg,lt,ditMini);
        }

        public void UpdateLength()
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.updateLengthCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(rsc_updateLengthCompleted);
            rsc.updateLengthAsync();
        }

        void rsc_updateLengthCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            isrealroadupdated = true;
            Dtimer.Begin();
            realwms.Visibility = Visibility.Collapsed;
            //getrouting("routing", begin, last);   
        }

        void rsc_GetBarrierRoadCompleted(object sender, GetBarrierRoadCompletedEventArgs e)
        {
            string[] strArray = e.Result[0].Split(new char[] { ',' });
            barrierS = int.Parse(strArray[1]);
            barrierT = int.Parse(strArray[2]);
            UpdatePassable(barrierS,barrierT,0);
        }
        void rsc_GetBeginRoadCompleted(object sender, GetBeginRoadCompletedEventArgs e)
        {
            string[] strArray = e.Result[0].Split(new char[] { ',' });
            begin = int.Parse(strArray[2]);

            BaseGeometry bg = GetItemByID("routing", "last");

            GetLastRoad(bg.Geometry.Coordinate.Y, bg.Geometry.Coordinate.X, 1000);
         
        }

        public  EnhancedMapLayer GetELayerByID(string layerID, MapCore map)
        {
            return map.Children.OfType<EnhancedMapLayer>().FirstOrDefault(geomLayer => geomLayer.ID == layerID);
        }

        public MapLayer GetLayerByID(string layerName, MapCore map)
        {
            return map.Children.OfType<MapLayer>().FirstOrDefault(geomLayer => geomLayer.Name == layerName);
        }

        public BaseGeometry GetItemByID(string layerId, string itemId)
        {
            var layer = GetELayerByID(layerId, ctlMap);
            if (layer != null)
            {
                return layer.GetGeometryByItemID(itemId);
            }
            return null;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            //LayerReset();
            UpdateLength();
        }

        private void btnReflesh_Click(object sender, RoutedEventArgs e)
        {
            //RouteLoadingPanel.Begin();
            //UpdateLength();


            //var twoRealWms = new fiveleveroadsTileSourse2();
           
            //realwms.laynum++;
            //if (realwms.laynum > 19)
            //{
            //    realwms.laynum = 0;
            //}
            //mtl.TileSources.Add(twoRealWms);

                  
        }

        private CustomPushpin GetCarByMSID(string strName, MapLayer layer)
        {
            try
            {
                foreach (CustomPushpin geometry in layer.Children.OfType<CustomPushpin>())
                {
                    if (geometry.Name.Equals(strName))
                    {

                        return geometry;
                    }

                }
            }
            catch 
            {
              
            }
            return null;
        }
        private void InitCar()
        {
            double CarSize = 50;
            string strImagePath = "/BaseMap;component/image/vehicle.png";
            //string strImagePath = "http://soulsolutions.com.au/Images/pin.png";
            var iconSpec3 = new CustomIconSpecification()
            {
              
                IconUri = new Uri(strImagePath, UriKind.RelativeOrAbsolute),
                Width = CarSize,
                Height = CarSize,
                IconOffset = new System.Windows.Point(-CarSize / 2, -CarSize / 2),//getIconOffset(iRotateTr),
                //TextContent = ClientStatic.GetIconLblText(initData.CarNo),
                TextOffet = new System.Windows.Point(-15, -46),
                //FontSize = ClientStatic.LableFontSize,
                //TextColor = ClientStatic.CarIconLblColorSCB,
                RotateTr = 0
            };
            var pin = new CustomPushpin
                          {
                              IconSpecification = iconSpec3,
                              Location = new Location(31.6, 120.3),
                              MapInstance = ctlMap,
                              Name = "car",
                          };
            carml.AddChild(pin);

            var dp = new DraggablePushpin {Location = new Location(31.6, 120.3), Name = "drapcar"};
            carml.AddChild(dp, dp.Location);
        }

        private DraggablePushpin GetDCarByMSID(string strName, MapLayer layer)
        {
            try
            {
                foreach (DraggablePushpin geometry in layer.Children.OfType<DraggablePushpin>().Where(geometry => geometry.Name.Equals(strName)))
                {
                    return geometry;
                }
            }
            catch
            {

            }
            return null;
        }
        //定时移动小车
        private void timer_Completed(object sender, EventArgs e)
        {
            
            //carml.Children.Add(carph);
           
            CustomPushpin cp = GetCarByMSID("car", carml);
            if (carloc.Count>0)
            {
                Location lc = carloc.ElementAt(cari);
                if (lc != null)
                {
                    MapLayer.SetPosition(cp, lc);
                    cp.Location = lc;

                    //double CarSize = 50;
                    //string strImagePath = "/BaseMap;component/image/vehicle.png";
                    //var icon = new CustomIconSpecification()
                    //{

                    //    IconUri = new Uri(strImagePath, UriKind.RelativeOrAbsolute),
                    //    Width = CarSize,
                    //    Height = CarSize,
                    //    IconOffset = new System.Windows.Point(-CarSize / 2, -CarSize / 2),//getIconOffset(iRotateTr),
                       
                    //    TextOffet = new System.Windows.Point(-15, -46),
                      
                    //    RotateTr = 60
                    //};


                    //cp.IconSpecification = icon;
                }
            }

            if (isgetloc==false)
            {
                timer.Begin();
            }
           
           
            if (cari<carloc.Count-1)
            {
                cari++;
            }
           
        }


        //private LocationCollection mulitLoc(LocationCollection locvol)
        //{
           
        //    LocationCollection templc1=locvol;

        //    int count = locvol.Count;
        //    for (int i = 0; i < count-1; i++)
        //    {
        //        if (0 < i && i < count-6)
        //        {
        //            double x = (templc1.ElementAt(i + 1).Longitude - templc1.ElementAt(i).Longitude) / 5;
        //            double y = (templc1.ElementAt(i + 1).Latitude - templc1.ElementAt(i).Latitude) / 5;
        //            if (Math.Abs(x)>0.00001||Math.Abs(y)>0.00001)
        //            {
        //                for (int m = 1; m < 5; m++)
        //                {
        //                    Location loc = new Location();
        //                    loc.Latitude = templc1.ElementAt(i).Latitude + m * y;
        //                    loc.Longitude = templc1.ElementAt(i).Longitude + m * x;
        //                    if ((i + (i-1) * 4 )<locvol.Count-1)
        //                    {
        //                        locvol.Insert(i + (i - 1) * 4, loc);
        //                    }
                            
        //                }
        //            }
                  
        //        }
              

        //    }



        //    return locvol;
        //}
        //定时去获取路径
        private void Dtimer_Completed(object sender, EventArgs e)
        {
            

            //if (cbxCar.IsChecked == true)
            //{
                CustomPushpin cp = GetCarByMSID("car", carml);
                if (cp != null)
                {
                    GetBegionRoad(cp.Location.Latitude, cp.Location.Longitude, 100);
                    isgetloc = true;
                    timer.Stop();
                }
            //}
            //else
            //{
            //    DraggablePushpin dp = GetDCarByMSID("drapcar", carml);
            //    if (dp != null)
            //    {
            //        GetBeginPoint(dp.Location.Latitude, dp.Location.Longitude, 100);
            //    }
            //}
                //if (isrealroadupdated==true)
                //{
                    Dtimer.Begin();
                //}
          
           
        }

        private void Ltimer_Completed(object sender, EventArgs e)
        {
            isrealroadupdated = false;
            UpdateLength();
            Ltimer.Begin();
        }

      



    }

 
}
