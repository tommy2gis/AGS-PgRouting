using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BaseMap.Control;
using ESRI.ArcGIS.Client;
using Diligentpig;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using BaseMap.RoutingServiceReference;
using System.Text;

namespace BaseMap
{
    public partial class ArcgisMap : UserControl
    {
        #region Field
        ObservableCollection<CustomMapPoint> carloc = new ObservableCollection<CustomMapPoint>();
        ObservableCollection<CustomMapPoint> fcarloc = new ObservableCollection<CustomMapPoint>();
       
        private string[] _fristArray;
        int _cari = 0;
        bool _isbegin = false;
        bool _islast = false;
        bool _isbarrier = false;
        bool _isgetlocing = false;
        bool _isupdatelength = false;
        int _barriarNum = 1;
        int _barrierS;
        int _barrierT;
        int _beginNum;
        int _lastNum;
        string[] otmpFirstArrays;


        private DateTime dt1;
        private DateTime dt2;
        private DateTime dt4;
        DynamicPushpin dpin;
#endregion
     
        #region  Method


        public ArcgisMap()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ArcgisMapLoaded);
        }

        void ArcgisMapLoaded(object sender, RoutedEventArgs e)
        {

            timer.Duration = new TimeSpan(0, 0, 0, 0, 500);
            Dtimer.Duration = new TimeSpan(0, 0, 0, 15);
            Ltimer.Duration = new TimeSpan(0, 0, 0, 45);
            var ml = new WuXiMapLayer();
            var MOml = new WuXiMapLayer();
            var gmap = new GoogleMap();
            var ly = new realLayer();
            var osm = new OpenStreetMapLayer();
            var cd = new ClientDynamicTileMapServiceLayer();
            var myGraphicsLayer = new GraphicsLayer { ID = "MyGraphicsLayer" };
            var myLineLayer = new GraphicsLayer { ID = "MyLineLayer" };
            MyMap.Layers.Add(ml);
            MyMap.Layers.Add(ly);
            MyMap.Layers.Add(myLineLayer);
            MyMap.Layers.Add(myGraphicsLayer);
            MyOverviewMap.Layer = MOml;
            AddPictureMarker();

        }
        private void CarBeginRun()
        {
            var c = GetCarByName("car", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer);
            if (c != null)
            {
                
                //MessageBox.Show(dt1.ToString());
                GetBeginPoint((c.Geometry as CustomMapPoint).Y, (c.Geometry as CustomMapPoint).X, 100);

            }
            Dtimer.Begin();
            Ltimer.Begin();
        }

        private bool IsCloset(CustomMapPoint loc, CustomMapPoint loc1, CustomMapPoint loc2)
        {
            bool isclose = false;
            double x = Math.Pow((loc.X - loc1.X), 2) + Math.Pow((loc.Y - loc1.Y), 2);
            double y = Math.Pow((loc.X - loc2.X), 2) + Math.Pow((loc.Y - loc2.Y), 2);
            if (x > y)
            {
                isclose = true;
            }
            return isclose;
        }

        private void AddPictureMarker()
        {
            var graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            var cg = new CarGraphic()
            {
                Geometry = new CustomMapPoint(120.428, 31.577),
                Name = "car",
                Symbol = GlobePictureSymbol
            };


            if (graphicsLayer != null) graphicsLayer.Graphics.Add(cg);
        }

        private CarGraphic GetCarByName(string name, GraphicsLayer glayer)
        {
            return glayer.Graphics.Select(geometry => geometry as CarGraphic).FirstOrDefault(otmcg => otmcg != null && otmcg.Name == name);
        }

        #endregion  

        #region WebService  Connects

        public void UpdatePassable(int source, int target, int passable)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.UpdatePassableCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(rsc_UpdatePassableCompleted);
            rsc.UpdatePassableAsync(source, target, passable);
        }

        void rsc_UpdatePassableCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        public void UpdateLength()
        {
            _isupdatelength = true;
            Dtimer.Stop();
            var rsc = new RoutingWebServiceSoapClient();
            rsc.updateLengthCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(RscUpdateLengthCompleted);
            rsc.updateLengthAsync();
        }

        void RscUpdateLengthCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _isupdatelength = false;
            Dtimer.Begin();
        }

        public void GetRouting(string layer, int begin, int last)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetRoutingCompleted += new EventHandler<GetRoutingCompletedEventArgs>(RscGetRoutingCompleted);
            rsc.GetRoutingAsync(layer, begin, last);
        }

        void RscGetRoutingCompleted(object sender, GetRoutingCompletedEventArgs e)
        {
        
             
        
       
           
            if (e.Result.Count() == 0)
            {
                Dtimer.Stop();
                timer.Stop();
                Ltimer.Stop();
                return;
            }
            carloc.Clear();
            _cari = 0;
            var lineLayer = MyMap.Layers["MyLineLayer"] as GraphicsLayer;
        
            if (lineLayer != null) lineLayer.ClearGraphics();
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
                    string[] strArray2 = strArray[strArray.Length - 1].Split(new char[] { ',' });
                    string[] strArrayPoints = strArray[strArray.Length - 2].Split(new char[] { ',' });
                    int beginNode = int.Parse(strArray[3]);
                    if (i==0)
                    {
                        otmpFirstArrays = strArrayPoints; 
                    }

                    if ((strArray2.Length == 1) && (strArray2[0].Length > 1))
                    {
                        //面
                    }
                    else
                    {
                        var locations = new ESRI.ArcGIS.Client.Geometry.PointCollection();
                        CustomMapPoint location2;
                        CustomMapPoint locPoint;
                        if (strArray2[0].Equals(strArray2[strArray2.Length - 1]))
                        {

                        }
                        else
                        {
                            var polyline = new ESRI.ArcGIS.Client.Geometry.Polyline();


                            for (index = strArray2.Length - 1; index >= 0; index--)
                            {
                                location2 = new CustomMapPoint(
                                    double.Parse(strArray2[index].Split(new char[] {' '})[1]),
                                    double.Parse(strArray2[index].Split(new char[] {' '})[0])) {Node = beginNode};
                                locations.Add(location2);


                            }

                            //添加坐标到小车轨迹
                            if (fcarloc.Count>0)
                            {
                                //取出轨迹数据的第一个点
                                var floc1 = new CustomMapPoint(double.Parse(otmpFirstArrays[0].Split(new char[] { ' ' })[1]),
                                                        double.Parse(otmpFirstArrays[0].Split(new char[] { ' ' })[0]));
                            
                                var ffloc1 = fcarloc.ElementAt(0);

                                var loc1 = new CustomMapPoint(double.Parse(strArrayPoints[0].Split(new char[] { ' ' })[1]),
                                                        double.Parse(strArrayPoints[0].Split(new char[] { ' ' })[0]));
                                var loc2 = new CustomMapPoint(double.Parse(strArrayPoints[strArrayPoints.Length - 1].Split(new char[] { ' ' })[1]),
                                                        double.Parse(strArrayPoints[strArrayPoints.Length - 1].Split(new char[] { ' ' })[0]));
                                CustomMapPoint oPoint = GetCarByName("car", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer).Geometry as CustomMapPoint;


                                if (i==0)
                                {
                                    //当前车辆不在在轨迹上
                                    if (!IsInLine(oPoint, otmpFirstArrays))
                                    {
                                     

                                        //调节车辆最后所在轨迹段顺序
                                        if (!Isclost(ffloc1, loc1, 0.0001) && !Isclost(ffloc1, loc2, 0.0001))
                                        {
                                            for (index = 0; index < fcarloc.Count - 1; index++)
                                            {
                                                var tmoloc = fcarloc.ElementAt(index);
                                              
                                                carloc.Add(tmoloc);


                                            }
                                           
                                        }
                                        else
                                        {

                                            for (index = fcarloc.Count - 1; index >= 0; index--)
                                            {
                                                var tmoloc1 = fcarloc.ElementAt(index);
                                                carloc.Add(tmoloc1);
                                            }
                                           
                                        }
                                    }
                                 

                                }
                              

                                if (i == 1)
                                {
                                    //调节第一段轨迹段顺序
                                    if (!Isclost(floc1, loc1, 0.00001) && !Isclost(floc1, loc2, 0.00001))
                                    {
                                     
                                        for (index = 0; index < otmpFirstArrays.Length - 1; index++)
                                        {
                                            var tmoloc =
                                                new CustomMapPoint(
                                                    double.Parse(otmpFirstArrays[index].Split(new char[] {' '})[1]),
                                                    double.Parse(otmpFirstArrays[index].Split(new char[] {' '})[0]))
                                                    {Node = beginNode};
                                            carloc.Add(tmoloc);

                                        }
                                     
                                    }
                                    else
                                    {
                                  
                                        for (index = otmpFirstArrays.Length - 1; index >= 0; index--)
                                        {
                                            var tmoloc1 =
                                                new CustomMapPoint(
                                                    double.Parse(otmpFirstArrays[index].Split(new char[] {' '})[1]),
                                                    double.Parse(otmpFirstArrays[index].Split(new char[] {' '})[0]))
                                                    {Node = beginNode};
                                            carloc.Add(tmoloc1);
                                        }
                                    }

                                    OrderPoints(strArrayPoints, carloc.ElementAt(carloc.Count - 1), loc1, loc2, beginNode);
                                }

                              
                                if(i>1)
                                {
                                    OrderPoints(strArrayPoints, carloc.ElementAt(carloc.Count - 1), loc1, loc2, beginNode);
                                }
                               
                               
                            }
                            polyline.Paths.Add(locations);
                            if (lineLayer != null)
                                lineLayer.Graphics.Add(new Graphic()
                                {
                                    Geometry = polyline,
                                    Symbol = DefaultLineSymbol
                                });
                        }
                    }

                }

            }
            CustomMapPoint c = GetCarByName("car", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer).Geometry as CustomMapPoint;                 
                for (int i = 0; i < carloc.Count-1; i++)
                {
                    var item = carloc.ElementAt(i);
                    if (Isclost(c, item, 0.0001))
                    {

                        for (int y = 0; y < i + 2; y++)
                        {
                            if (carloc.Count > y + 1)
                            {
                                carloc.RemoveAt(0);
                            }
                        }

                    }
                }
            
            _isgetlocing = false;
            timer.Begin();       
        }

        private bool IsInLine(CustomMapPoint cmp,string[] strings)
        {
            bool isclost=false;
            for (int i = 0; i < strings.Length-1; i++)
            {

             
                var tmoloc =
                    new CustomMapPoint(
                        double.Parse(otmpFirstArrays[i].Split(new char[] {' '})[1]),
                        double.Parse(otmpFirstArrays[i].Split(new char[] {' '})[0]));
                if (Isclost(cmp, tmoloc, 0.00001))
                {
                    isclost = true;
                }
                
            }
            return isclost;
        }

        private static bool Isclost(CustomMapPoint c, CustomMapPoint item,double l)
        {
            return Math.Abs(Math.Round(item.X, 7) - Math.Round(c.X, 7)) < l && Math.Abs(Math.Round(item.Y, 7) - Math.Round(c.Y, 7)) < l;
        }
      
        private void OrderPoints(string[] strArrayPoints, CustomMapPoint mpac, CustomMapPoint loc1, CustomMapPoint loc2,int node)
        {
            int index;
            if (Isclost(mpac, loc1, 0.001))
            {

                for (index = 0; index < strArrayPoints.Length - 1; index++)
                {
                    var tmoloc = new CustomMapPoint(double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[1]),
                                                    double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[0])) { Node = node };
                    carloc.Add(tmoloc);
                }
         
            }
            else
            {
                for (index = strArrayPoints.Length - 1; index >= 0; index--)
                {
                    var tmoloc1 = new CustomMapPoint(double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[1]),
                                                     double.Parse(strArrayPoints[index].Split(new char[] { ' ' })[0])) { Node = node };
                    carloc.Add(tmoloc1);
                }
            }
        }

        public void GetBeginPoint(double lg, double lt, double ditMini)
        {
            
            //_isgetlocing = true;
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetBeginRoadCompleted += new EventHandler<GetBeginRoadCompletedEventArgs>(RscGetBeginRoadCompleted);

            rsc.GetBeginRoadAsync(lg, lt, ditMini);

        }

        void RscGetBeginRoadCompleted(object sender, GetBeginRoadCompletedEventArgs e)
        {

            
            string[] strArray = e.Result[0].Split(new char[] { ';' });
            _beginNum = int.Parse(strArray[2]);

            if (_lastNum==0)
            {
                _fristArray = strArray[strArray.Length - 1].Split(new char[] { ',' });
                fcarloc.Clear();
           
                for (int i = 0; i < _fristArray.Length - 1; i++)
                {
                    var tmoloc =
                        new CustomMapPoint(
                            double.Parse(_fristArray[i].Split(new char[] { ' ' })[1]),
                            double.Parse(_fristArray[i].Split(new char[] { ' ' })[0])) { Node = _beginNum };

                    fcarloc.Add(tmoloc); 
                }
             
            }

            if (_lastNum!=0)
            {

                GetRouting("routing", _beginNum, _lastNum);
            }
            else
            {
                CarGraphic c = GetCarByName("终点", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer);
                if (c != null)
                {

                    GetLastPoint((c.Geometry as MapPoint).Y, (c.Geometry as MapPoint).X, 100);
                }
            }
           
            //BaseGeometry bg = GetItemByID("routing", "last");



        }

        public void GetBarrier(double lg, double lt, double ditMini)
        {
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetBarrierRoadCompleted += new EventHandler<GetBarrierRoadCompletedEventArgs>(RscGetBarrierRoadCompleted);
            rsc.GetBarrierRoadAsync(lg, lt, ditMini);
        }

        void RscGetBarrierRoadCompleted(object sender, GetBarrierRoadCompletedEventArgs e)
        {
            string[] strArray = e.Result[0].Split(new char[] { ';' });
            _barrierS = int.Parse(strArray[1]);
            _barrierT = int.Parse(strArray[2]);
            UpdatePassable(_barrierS, _barrierT, 0);
        }

        public void GetLastPoint(double lg, double lt, double ditMini)
        {
            
            var rsc = new RoutingWebServiceSoapClient();
            rsc.GetLastRoadCompleted += new EventHandler<GetLastRoadCompletedEventArgs>(RscGetLastRoadCompleted);
            rsc.GetLastRoadAsync(lg, lt, ditMini);
        }

        void RscGetLastRoadCompleted(object sender, GetLastRoadCompletedEventArgs e)
        {

            

            string[] strArray = e.Result[0].Split(new char[] { ';' });
            _lastNum = int.Parse(strArray[2]);
          
            GetRouting("routing", _beginNum, _lastNum);
        }

        #endregion

        #region 按钮单击事件
        //private void btnbegin_Click(object sender, RoutedEventArgs e)
        //{
            
        //    _isbegin = true;
        //}

        //private void btnlast_Click(object sender, RoutedEventArgs e)
        //{
        //    _islast = true;
          
        //}

        //private void btnbarrier_Click(object sender, RoutedEventArgs e)
        //{
        //     _isbarrier = true;
        //}

        //private void calculate_Click(object sender, RoutedEventArgs e)
        //{
        //    GetRouting("routing", 226, 256);
        //}

        //private void btnClear_Click(object sender, RoutedEventArgs e)
        //{
           
            
        //}

        //private void btnReflesh_Click(object sender, RoutedEventArgs e)
        //{
        //    UpdateLength();
        //}

        private void ToolBarPanelToolBarItemClick(object sender, string key)
        {
            switch (key)
            {
                case "beginPoint":
                    {
                        _isbegin = true;
                    }

                    break;
                case "lastPoint":
                    {
                        _islast = true;
                    }

                    break;
                case "barrierPoint":
                    {
                        _isbarrier = true;
                    }

                    break;
                case "calculate":
                    {
                        GetRouting("routing", 226, 256);
                    }

                    break;
                case "clear":
                    {

                    }

                    break;
                case "reflesh":
                    {
                        UpdateLength();
                    }

                    break;

                default:

                    break;

            }
        }
#endregion

        #region  时间控制器事件
        private void TimerCompleted(object sender, EventArgs e)
        {

            CarGraphic c = GetCarByName("car", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer);
            if (carloc.Count > 0)
            {
                CustomMapPoint lc = carloc.ElementAt(_cari);
                if (lc != null)
                {
                    c.Node = lc.Node;
                    c.Geometry = lc;
                   
                }
                if (_cari%10==0&&cbxLocCar.IsChecked==true)
                {
                   
                    MyMap.PanTo(lc);
                }
              
            }
            fcarloc.Clear();
            var cmp = GetCarByName("car", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer);
            foreach (var trigger in carloc.Where(trigger => trigger.Node == cmp.Node))
            {
                fcarloc.Add(trigger);
            }

            if (fcarloc.Count > 25)
            {
                for (int k = 0; k < fcarloc.Count-1; k++)
                {
                    var cp = fcarloc.ElementAt(k);
                    if (Isclost(cmp.Geometry as CustomMapPoint, cp, 0.00001))
                    {

                        if (fcarloc.Count - k > 18)
                        {
                            _isgetlocing = true;
                        }

                    }
                }
            }
           
          
            //if (_isgetlocing == false)
            //{
                timer.Begin();
            //}


            if (_cari < carloc.Count - 1)
            {
                _cari++;
            }
        }

        private void DtimerCompleted(object sender, EventArgs e)
        {
            
            var c = GetCarByName("car", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer);
            if (c != null&&_beginNum==0)
            {
               
                GetBeginPoint((c.Geometry as CustomMapPoint).Y, (c.Geometry as CustomMapPoint).X, 100);
            }
            if (_beginNum != 0)
            {
                dt1 = DateTime.Now;
                //fcarloc.Clear();
                var cmp = GetCarByName("car", MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer);
                //foreach (var trigger in carloc.Where(trigger => trigger.Node==cmp.Node))
                //{
                //    fcarloc.Add(trigger);
                //}
               
                if (cmp.Node!=0)
                {
                    GetRouting("routing", cmp.Node, _lastNum);
                }
               
            }

            if (_isupdatelength==false)
            {
                Dtimer.Begin();
            }
          

        }

        private void LtimerCompleted(object sender, EventArgs e)
        {
            UpdateLength();
            Ltimer.Begin();
        }

#endregion

        #region  地图事件
        private void MyMap_MouseClick(object sender, Map.MouseEventArgs e)
        {
            if (_isbegin == true)
            {
                //BaseGeometry bg = GetItemByID("routing", "beginNum");

                //if (bg != null)
                //{
                //    LayerReset();
                //}

                var graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
                var cg = new CarGraphic()
                {
                    Geometry = e.MapPoint,
                    Name = "起点",
                    Symbol = BeginPictureSymbol
                };


                if (graphicsLayer != null) graphicsLayer.Graphics.Add(cg);

                _isbegin = false;
            }

            if (_islast == true)
            {
                var graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
                var cg = new CarGraphic()
                {
                    Geometry = e.MapPoint,
                    Name = "终点",
                    Symbol = EndPictureSymbol
                };

                if (graphicsLayer != null) graphicsLayer.Graphics.Add(cg);

                CarBeginRun();
                //Ltimer.Begin();
                _islast = false;
            }

            if (_isbarrier == true)
            {
                //初始化一个图标

                var graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
                var cg = new CarGraphic()
                {
                    Geometry = e.MapPoint,
                    Name = "障碍"+_barriarNum.ToString(),
                    Symbol = BarriarPictureSymbol
                };

                if (graphicsLayer != null) graphicsLayer.Graphics.Add(cg);

                _isbarrier = false;
                _barriarNum++;


            }
        }

        private void MyMap_MouseMove(object sender, MouseEventArgs e)
        {

            //显示坐标
            if (MyMap.Extent != null)
            {
                System.Windows.Point screenPoint = e.GetPosition(MyMap);
                ESRI.ArcGIS.Client.Geometry.MapPoint mapPoint = MyMap.ScreenToMap(screenPoint);
                MapCoordsTextBlock.Text = string.Format("X = {0}, Y = {1}",
                    Math.Round(mapPoint.X, 4), Math.Round(mapPoint.Y, 4));
            }

        }
#endregion
  
    }

    public class CarGraphic : Graphic
    {
        public string Name { get; set; }
        public int Node { get; set; }
    }

    public class CustomMapPoint : MapPoint
    {
        public string Name { get; set; }
        public int Node { get; set; }
        public CustomMapPoint(double X,double Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
