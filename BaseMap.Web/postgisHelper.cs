using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseMap.Web
{
    public class postgisHelper
    {



        public static List<string> GetClosetRoad(double lg, double lt, double ditMini)
        {
            var records = new List<string>();
            var pg = new PostGIS();
            string strSQL = string.Format(@"select source ,target,st_astext(pointgeom) as pointgeom from ch10.realroad
           WHERE
                ST_DWithin(the_geom ,GeometryFromText('point({0} {1})',4326), {2}) 
           order by ST_Distance(the_geom,GeometryFromText('point({0} {1})',4326))  limit 1", lt, lg, ditMini);
            records = pg.QueryAdd("mini", strSQL);
            return records;
        }

        public static void updateLength()
        {

            var pg = new PostGIS();
            string strSQL = "update ch10.realtimeroad set length1=random()*40";
            pg.SQLExcute(strSQL);
        }

     

        public static void updatePassable(int source ,int target,int passable)
        {
            PostGIS pg = new PostGIS();
            string strSQL = string.Format(@" update ch10.realtimeroad set passable={2}  
           WHERE realtimeroad.source={0} and realtimeroad.target={1}", source, target, passable);
            pg.SQLExcute(strSQL);
        }

        public static void updatePassableToT()
        {
            var pg = new PostGIS();
            string strSQL = "update ch10.realtimeroad set passable=1";
          
            pg.SQLExcute(strSQL);
        }

        public static List<string> GetAllTableFeatures(string layer,int begin,int last)
        {
            var pg = new PostGIS();
            string sql = "  set search_path = public, ch10;"+

                "SELECT  NAME,source,target,st_astext(pointgeom) as pointgeom, st_astext(the_geom) as geom FROM shortest_path('select st_astext(the_geom),gid as id,source::int4,target::int4,length::double precision as cost "
                
                + "from realtimeroad  where passable <>0 '," + begin + "," + last + ",false,false) as sp ,realtimeroad as tc "
                
                +"where sp.edge_id=tc.gid; ";

            var records = pg.Query(layer, sql);
            return records;
        }

        public static List<string> GetRealRoads(string layer,string leve)
        {
            var pg = new PostGIS();
            string sql = "  set search_path = public, ch10;" +
                "select sroadid,st_astext(the_geom) as geom,accessleve from fourleveroads";

            var records = pg.QueryRealRoads(layer, sql);
            return records;
        }
    }


    public class PostGIS
    {
        NpgsqlConnection conn;
        private string _layer;
        private string _bbox;
        private string _table;
        readonly string _connStr;
        public PostGIS()
        {
            this._connStr = "server=localhost;uid=postgres;pwd=123456;database=postgis;port=5432";     
        }
        public  void SQLExcute(string SQLCmd)
        {
            NpgsqlConnection Conn = new NpgsqlConnection(_connStr);

            try
            {
               
                Conn.Open();

                NpgsqlCommand Cmd = new NpgsqlCommand();

                Cmd.Connection = Conn;

                Cmd.CommandTimeout = 150;

                Cmd.CommandType = System.Data.CommandType.Text;

                Cmd.CommandText = SQLCmd;

                Cmd.ExecuteNonQuery();

               
            }
            catch 
            {
                
                
            }

            Conn.Close();

        }



        public void UpdateData(string sql)
        {
            string connStr = _connStr;
            if (conn == null)
            {
                conn = new NpgsqlConnection(connStr);
            }
            conn.Open();

            var command = new NpgsqlCommand(sql, conn) {CommandTimeout = 120};

            conn.Close();
        }

        public List<string> QueryAdd(string _layer, string sql)
        {
            StringBuilder sb;
            string[] fields;
            var records = new List<string>();
            string connStr = _connStr;
            if (conn == null)
            {
                conn = new NpgsqlConnection(connStr);
            }
            conn.Open();
            try
            {
                var command = new NpgsqlCommand(sql, conn) {CommandTimeout = 120};
                NpgsqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(_layer))
                        sb = new StringBuilder(_layer + ";");
                    for (int i = 0; i < rdr.FieldCount - 1; i++)
                    {
                        sb.Append(rdr[i].ToString() + ";");
                    }
                    fields = Regex.Split(rdr["pointgeom"].ToString(), @"\(+(.*?)\)+");
                    if (fields[0].Equals("MULTIPOINT"))
                    {

                        string[] pts = fields[1].Split(',');
                        for (int i = 0; i < pts.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append((pts[i].Split(' '))[1]);//latitude
                            sb.Append(" " + (pts[i].Split(' '))[0]);//longitude
                        }

                    }
                    records.Add(sb.ToString());

                  
                }
                rdr.Close();
            }
            catch
            {

            }


            conn.Close();
            return records;
        }

        public List<string> QueryRealRoads(string _layer, string sql)
        {
            StringBuilder sb;
            string[] fields;
            var records = new List<string>();
            string connStr = _connStr;
            if (conn == null)
            {
                conn = new NpgsqlConnection(connStr);
            }
            conn.Open();
            try
            {
                var command = new NpgsqlCommand(sql, conn) { CommandTimeout = 120 };
                NpgsqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                  
                    sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(_layer))
                        sb = new StringBuilder(_layer + ";");
                    for (int i = 0; i < rdr.FieldCount - 2; i++)
                    {
                        sb.Append(rdr[i].ToString() + ";");
                    }
                    fields = Regex.Split(rdr["geom"].ToString(), @"\(+(.*?)\)+");
                    if (fields[0].Equals("LINESTRING"))
                    {
                      
                        string[] pts = fields[1].Split(',');
                        for (int i = 0; i < pts.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append((pts[i].Split(' '))[1]);//latitude
                            sb.Append(" " + (pts[i].Split(' '))[0]);//longitude
                        }

                    }
                    records.Add(sb.ToString());
                }
                rdr.Close();
            }
            catch
            {

            }


            conn.Close();
            return records;
        }

        public List<string> Query(string _layer, string sql)
        {
            StringBuilder sb;
            List<string> records = new List<string>();
            string[] fields;
            string[] pointsfields;
            string geomType;
            string[] names;
            string[] Locations;
            string[] OrderIds;
            string connStr = _connStr;
            if (conn == null)
            {
                conn = new NpgsqlConnection(connStr);
            }

            conn.Open();

            var command = new NpgsqlCommand(sql, conn);
            command.CommandTimeout = 120;
            int index = 0;
            try
            {
                NpgsqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                //    if (index == 500)
                //    {
                //        break;
                //    }
                //    index++;
                    sb = new StringBuilder();
                    if (!string.IsNullOrEmpty(_layer))
                        sb = new StringBuilder(_layer + ";");
                    for (int i = 0; i < rdr.FieldCount - 2; i++)
                    {
                        sb.Append(rdr[i].ToString() + ";");
                    }
                    fields = Regex.Split(rdr["geom"].ToString(), @"\(+(.*?)\)+");
                    pointsfields = Regex.Split(rdr["pointgeom"].ToString(), @"\(+(.*?)\)+");
                    names = Regex.Split(rdr["name"].ToString(), @"\(+(.*?)\)+");
                    geomType = fields[0];

                    if (pointsfields[0].Equals("MULTIPOINT"))
                    {
                          string[] mpts = pointsfields[1].Split(',');
                                for (int i = 0; i < mpts.Length; i++)
                                {
                                    if (i > 0) sb.Append(",");
                                    sb.Append((mpts[i].Split(' '))[1]);//latitude
                                    sb.Append(" " + (mpts[i].Split(' '))[0]);//longitude
                                }
                    }

                    if (geomType.Equals("MULTILINESTRING"))
                    {
                        sb.Append(";");
                        string[] pts = fields[1].Split(',');
                        for (int i = 0; i < pts.Length; i++)
                        {
                            if (i > 0) sb.Append(",");
                            sb.Append((pts[i].Split(' '))[1]);//latitude
                            sb.Append(" " + (pts[i].Split(' '))[0]);//longitude
                        }
                        
                    }
                   
                      
                  

                    records.Add(sb.ToString());
                }
                rdr.Close();
            }
            catch (Exception e)
            {
                records.Add(e.Message);
            }
            conn.Close();
            return records;
        }
    }
}