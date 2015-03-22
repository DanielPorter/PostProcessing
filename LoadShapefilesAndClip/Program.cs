using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Npgsql;
using System.IO;
using System.Data;

namespace LoadShapefilesAndClip
{
    class Program
    {
        private static string FindPsqlDir()
        {
            string PsqlDir = null;
            if (File.Exists(@"c:\program files (x86)\postgresql\9.2\bin\"))
            {
                PsqlDir = @"c:\program files (x86)\postgresql\9.2\bin\psql.exe";
            }
            else if (File.Exists(@"c:\program files\postgresql\9.2\bin\"))
            {
                PsqlDir = @"c:\program files\postgresql\9.2\bin\";
            }
            else if (File.Exists(@"c:\program files\postgresql\9.3\bin\psql.exe"))
            {
                PsqlDir = @"C:\program filepostgresql\9.3\bin\";
            }
            else
                Console.WriteLine("Unable to locate psql, which is required for interfacing with PostgreSQL. Program can't run.");
            return PsqlDir;
        }
        static void Main(string[] args)
        {
            string postgresBinDir = @"C:\program files (x86)\Postgresql\9.2\bin\";
            postgresBinDir = FindPsqlDir();
            string dir = @"I:\LaserData";
            string tableName = "LaserPoints";

            LoadShapefiles(postgresBinDir, tableName, dir);

            
        }
        private static void LoadShapefiles(string postgresBinDir, string tableName, string dir)
        {
            var a = Directory.EnumerateDirectories(dir).Select(directory => Directory.EnumerateFiles(directory).Where(file => file.EndsWith(".shp"))).SelectMany(i => i).ToList();

            ShapefileLoader sl = new ShapefileLoader(postgresBinDir + "shp2pgsql");

            var cmds = a.Select(x => sl.ConvertShapefileToSql(tableName, x)).ToList();

            string cmd2 = "\"" + postgresBinDir + "\" -h localhost -U postgres -p 5434 -d postgis_21_sample -f {0}.sql >> log";
            cmds.Insert(0, string.Format(cmd2, "createlasertable"));

            Execute(cmds, "CreateSQLFromShapefiles");
            List<string> loadSql = Directory.EnumerateFiles(Directory.GetCurrentDirectory())
                 .Where(x => x.Contains("LaserPointstable"))
                 .Select(x => x.Substring(0, x.Length - 4))
                 .Select(x => string.Format(cmd2, x)).ToList();

            cmds.Clear();

            cmds.AddRange(loadSql);
            cmds.Add(string.Format(cmd2, "assignharblkid"));
            Execute(cmds, "LoadSQL");
            CreateShapefiles();
        }
        private static void Execute(List<string> cmds, string batchFileName)
        {
            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + batchFileName +  ".bat", cmds);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + batchFileName +  ".bat";
            p.Start();
            p.WaitForExit(-1);
        }
        private static void ExecuteCommands(List<string> cmds, string tableName)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            string cmd3 = @"DELETE 
                FROM {0}
	                USING (SELECT geom FROM polygons WHERE harblkid = '{0}') as a
                WHERE NOT st_contains(a.geom, {0}.geom)";
            string cmd2 = "\"" + FindPsqlDir() + "\" -h localhost -U postgres -p 5434 -d postgis_21_sample -f {0}.sql >> log";
            cmds.Add(string.Format(cmd2, tableName));
            List<string> loadSql = Directory.EnumerateFiles(Directory.GetCurrentDirectory())
                 .Where(x => x.Contains("LaserPointstable"))
                 .Select(x => x.Substring(0, x.Length - 4))
                 .Select(x => string.Format(cmd2, x)).ToList();
            
            cmds.Insert(0, string.Format(cmd2, "createlasertable"));
            cmds.AddRange(loadSql);
            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + tableName + ".bat", cmds);
            p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + tableName + ".bat";
            p.Start();
            p.WaitForExit();
            
        }

        private static void CreateShapefiles()
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;

            string connstring = "Server=localhost;Port=5434;User Id=postgres;Password=postgres;Database=postgis_21_sample;";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string query = "select distinct harblkid from sourcepolygon;";
            
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
            System.Data.DataSet ds = new System.Data.DataSet();
            ds.Reset();
            da.Fill(ds);

            System.Data.DataTable pointsDataTable = ds.Tables[0];
            conn.Close();
            Directory.CreateDirectory("I:\\block_laser_shapefiles");
            string cmd = String.Format("\"{0}\" -f I:\\block_laser_shapefiles\\{0}\\{0} -h localhost -u postgres -P postgres -p 5434 postgis_21_sample "
                + "\"SELECT * FROM laserpoints WHERE harblkid = '{1}'\" >> log 2>&1", FindPsqlDir());
            List<string> cmds = new List<string>();
            foreach (DataRow dr in pointsDataTable.Rows)
            {
                string harblkid;
                try
                {
                    harblkid = (string)dr[0];
                    Directory.CreateDirectory(string.Format("I:\\block_laser_shapefiles\\{0}", harblkid));
                }
                catch
                {
                    harblkid = "";
                    continue;
                }
                string shapefile_name = harblkid != "" ? harblkid : "emptypoints";
                cmds.Add(string.Format(cmd, shapefile_name, harblkid));
            }

            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + "getshapefiles" + ".bat", cmds);
            p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + "getshapefiles" + ".bat";
            p.Start();
            p.WaitForExit(-1);
        }
    }
}
