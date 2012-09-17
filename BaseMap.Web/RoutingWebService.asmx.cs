using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections.Generic;


namespace BaseMap.Web
{
    /// <summary>
    /// RoutingWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class RoutingWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public List<string> GetRouting(string layer,int begin,int last)
        {
            var records = postgisHelper.GetAllTableFeatures(layer,begin,last);
            return records;
        }

        [WebMethod]
        public List<string> GetBeginRoad(double lg, double lt, double ditMini)
        {
            var records = postgisHelper.GetClosetRoad(lg, lt, ditMini);
            return records;
        }

        [WebMethod]
        public List<string> GetLastRoad(double lg, double lt, double ditMini)
        {
            var records = postgisHelper.GetClosetRoad(lg, lt, ditMini);
            return records;
        }

        [WebMethod]
        public List<string> GetBarrierRoad(double lg, double lt, double ditMini)
        {
            var records = postgisHelper.GetClosetRoad(lg, lt, ditMini);
            return records;
        }

        [WebMethod]
        public void UpdatePassable(int source, int target, int passable)
        {

            postgisHelper.updatePassable(source,target, passable);
        
        }
        [WebMethod]
        public List<string> QueryRealRoads(string layer, string leve)
        {

            var records = postgisHelper.GetRealRoads(layer, leve);
            return records;
        }

         [WebMethod]
        public void updateLength()
        {

            postgisHelper.updateLength();
        
        }
        
              [WebMethod]
         public void updatePassableToT()
        {

            postgisHelper.updatePassableToT();
        
        }

            
    }
}
