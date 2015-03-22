using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Data;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using Npgsql;
using System.Data.Odbc;
using System.Data.OleDb;

namespace PostProcessing
{

    public partial class PostProcessing : Form
    {
        string destinationDataDir;
        string PolygonShapefile;
        string PolygonDir;
        IFeatureSet polygons;
        string reportPath = @"C:\users\public\documents\postprocessing\BlockReport.xlsx";
        string[] FolderNames;
        string PsqlDir;
        PostProcessingSettings settings;
        public PostProcessing()
        {
            settings = new PostProcessingSettings();
            settings.LoadSettings();
            destinationDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\PostProcessing\\";
            PolygonShapefile = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\PostProcessing\\" +@"SOURCEPOLYGON.shp";
            PsqlDir = FindPsqlDir();
            try
            {
                polygons = FeatureSet.Open(PolygonShapefile);
                if (polygons.NumRows() == 0)
                {
                    MessageBox.Show("No records found in polygon shapefile. Cannot run.");
                }
                FolderNames = polygons.Features.AsEnumerable().Select(x => (string)x.DataRow["harblkid"]).ToArray();
            }
            catch (Exception err)
            {
                MessageBox.Show("Unable to access the polygon shapefile which should be located at " + destinationDataDir + ".");
                throw;
            }
            if (!Directory.Exists(destinationDataDir))
            {
                Directory.CreateDirectory(destinationDataDir);
            }

            reportPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + @"BlockReport.xlsx";
            InitializeComponent();
            txt_DataSource.Text = settings.SourceDataDir;
            txt_DataDestination.Text = settings.DestinationDataDir;
            CreateFolders();


        }

        private string FindPsqlDir()
        {
            string PsqlDir = null;
            if (File.Exists(@"c:\program files (x86)\postgresql\9.2\bin\psql.exe"))
            {
                PsqlDir = @"c:\program files (x86)\postgresql\9.2\bin\";
            }
            else if (File.Exists(@"c:\program files\postgresql\9.2\bin\psql.exe"))
            {
                PsqlDir = @"c:\program files\postgresql\9.2\bin\";
            }
            else if (File.Exists(@"c:\program files\postgresql\9.3\bin\psql.exe"))
            {
                PsqlDir = @"C:\program files\postgresql\9.3\bin\";
            }
            else
                MessageBox.Show("Unable to locate psql, which is required for interfacing with PostgreSQL. Program can't run.");
            return PsqlDir;
        }

        private void LoadShapefiles(string postgresBinDir, string tableName, string dir)
        {
            var old_sql = Directory.EnumerateFiles(Directory.GetCurrentDirectory()).Where(x => x.Contains("laserpointstable"));
            foreach(var old_sql_file in old_sql)
            {
                File.Delete(old_sql_file);
            }
            var a = Directory.EnumerateDirectories(dir).Select(directory => Directory.EnumerateFiles(directory).Where(file => file.EndsWith(".shp"))).SelectMany(i => i).ToList();

            ShapefileLoader sl = new ShapefileLoader(postgresBinDir + "shp2pgsql");

            var cmds = a.Select(x => sl.ConvertShapefileToSql(tableName, x)).ToList();

            string cmd2 = "\"" + postgresBinDir + "psql.exe\" -h localhost -U postgres -p 5434 -d postgis_21_sample -f \"{0}.sql\" >> log";
            cmds.Insert(0, string.Format(cmd2, "createlasertable"));


            string directory1 = Directory.GetCurrentDirectory();
            Execute(cmds, "CreateSQLFromShapefiles");
            List<string> loadSql = Directory.EnumerateFiles(Directory.GetCurrentDirectory())
                 .Where(x => x.Contains("laserpointstable"))
                 .Select(x => x.Substring(0, x.Length - 4))
                 .Select(x => string.Format(cmd2, x)).ToList();

            cmds.Clear();

            cmds.AddRange(loadSql);
            cmds.Add(string.Format(cmd2, "assignharblkid"));
            Execute(cmds, "LoadSQL");
            CreateShapefiles();
        }
        private void CreateShapefiles()
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
            Directory.CreateDirectory(settings.DestinationDataDir);
            string cmd = "\"{2}\" -f {3}\\{0}\\{0} -h localhost -u postgres -P postgres -p 5434 postgis_21_sample "
                + "\"SELECT * FROM laserpoints WHERE harblkid = '{1}'\" >> log 2>&1";
            List<string> cmds = new List<string>();
            foreach (DataRow dr in pointsDataTable.Rows)
            {
                string harblkid;
                try
                {
                    harblkid = (string)dr[0];
                    Directory.CreateDirectory(string.Format("{0}\\{1}", settings.DestinationDataDir, harblkid));
                }
                catch
                {
                    harblkid = "";
                    continue;
                }
                string shapefile_name = harblkid != "" ? harblkid : "emptypoints";
                cmds.Add(string.Format(cmd, shapefile_name, harblkid, FindPsqlDir() + "pgsql2shp.exe", settings.DestinationDataDir));
            }

            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + "getshapefiles" + ".bat", cmds);
            p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + "getshapefiles" + ".bat";
            p.Start();
            p.WaitForExit(-1);
        }

        private void CreateFolders()
        {
            polygons.FillAttributes();
            DataTable dt = polygons.DataTable;


            //System.IO.File.WriteAllLines(@"C:\Users\Public\documents\blockids.txt", FolderNames);
            foreach(string folder in FolderNames)
            {
                if (!Directory.Exists(destinationDataDir + folder))
                {
                    Directory.CreateDirectory(destinationDataDir + folder);

                }
            }
        }

        private static void Execute(List<string> cmds, string batchFileName)
        {
            File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + batchFileName + ".bat", cmds);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + batchFileName + ".bat";
            p.Start();
            p.WaitForExit(-1);
        }
        private void WriteReport()
        {
            List<string> FoldersWithShapefiles = new List<string>();
            foreach (string name in FolderNames)
            {
                if (Directory.EnumerateFiles(destinationDataDir + name).Count() > 0)
                {
                    FoldersWithShapefiles.Add(name);
                }
            }
            BlockReport report = new BlockReport(reportPath);
            FoldersWithShapefiles.Remove("LKP0801E&M");
            //FoldersWithShapefiles.Add("LKP0101VAL");
            //FoldersWithShapefiles.Add("LKP0102PIN");
            string connstring1 = "Server=localhost;Port=5434;User Id=postgres;Password=postgres;Database=postgis_21_sample;CommandTimeout=1000;";
            NpgsqlConnection conn1 = new NpgsqlConnection(connstring1);
            conn1.Open();
            string query1 = "SELECT DISTINCT harblkid FROM laserpoints;";
            NpgsqlDataAdapter da1 = new NpgsqlDataAdapter(query1, conn1);
            System.Data.DataSet ds1 = new System.Data.DataSet();
            ds1.Reset();
            da1.Fill(ds1);
            List<string> blocks = new List<string>();
            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                blocks.Add(dr[0].ToString());
            }
            MessageBox.Show(string.Join(",", blocks));

            for(int j = 0; j < blocks.Count; j++)
            {

                string name = blocks[j];
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = "YOURBATCHFILE.bat";
                
                DataTable dt = polygons.DataTable;
                int activeRow = -1;
                
                for (int i = 0; i < FolderNames.Length; i++)
                {
                    if (FolderNames[i] == name)
                    {
                        activeRow = i;
                    }
                }
                List<string> batchcommands = new List<string>();
                RunReportWorker.ReportProgress(Convert.ToInt32(Convert.ToDouble(j) / blocks.Count * 100), name + " Now querying postgres");
                string connstring = "Server=localhost;Port=5434;User Id=postgres;Password=postgres;Database=postgis_21_sample;CommandTimeout=1000000;";
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                string query = "select * from LASERPOINTS where harblkid = '{0}';";
                query = string.Format(query, name);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                System.Data.DataSet ds = new System.Data.DataSet();
                ds.Reset();
                da.Fill(ds);
                RunReportWorker.ReportProgress(Convert.ToInt32(Convert.ToDouble(j) / blocks.Count * 100), name + " Successfully queried postgres. Running calculations");
                
                System.Data.DataTable pointsDataTable = ds.Tables[0];
                conn.Close();
                double MeanTreeDensity;
                try
                {
                    MeanTreeDensity = Convert.ToDouble(dt.Rows[activeRow]["MEAN_TREE_"]);
                }
                catch
                {
                    MeanTreeDensity = 0;
                }
                double AdjustedTreeSpacing;
                try
                {
                    AdjustedTreeSpacing = Convert.ToDouble(dt.Rows[activeRow]["Adj_Tree_S"]);
                }
                catch
                {
                    AdjustedTreeSpacing = 0;
                }
                if (pointsDataTable.Rows.Count == 0)
                {
                    MessageBox.Show(string.Format("Block {0} returned 0 rows.", name));
                    continue;
                }
                report.WriteRow(new BlockSummary(pointsDataTable,
                    polygons.Features[activeRow].Area() / 4046.86, // acres conversion factor
                    Convert.ToDouble(dt.Rows[activeRow]["Row_Spacin"]),
                    Convert.ToDouble(dt.Rows[activeRow]["Initial_Tr"]),
                    AdjustedTreeSpacing,
                    name,
                    MeanTreeDensity
                    ));

            }
            report.Save();
            RunReportWorker.ReportProgress(100, "Done!");

        }

        private void PopulateDatabase()
        {
            List<string> FoldersWithShapefiles = new List<string>();
            foreach (string name in FolderNames)
            {
                if (Directory.EnumerateFiles(destinationDataDir + name).Count() > 0)
                {
                    FoldersWithShapefiles.Add(name);
                }
            }

            string CreatePolygonSql = "\"C:\\program files (x86)\\postgresql\\9.2\\bin\\shp2pgsql\" -d  {0} > polygons.sql\r\n";
            CreatePolygonSql = string.Format(CreatePolygonSql, PolygonShapefile);
            string LoadPolygon = "\"{0}\" -h localhost -U postgres -p 5434 -d postgis_21_sample -f polygons.sql";
            LoadPolygon = string.Format(LoadPolygon, PsqlDir);
            MessageBox.Show(String.Format("{0} folders with shapefiles.", FoldersWithShapefiles.Count));
            
            File.WriteAllText("batch.bat", CreatePolygonSql + LoadPolygon);
            Process pr = new Process();
            pr.StartInfo.UseShellExecute = false;
            pr.StartInfo.FileName = "batch.bat";
            pr.Start();
            pr.WaitForExit();
            foreach (string name in FoldersWithShapefiles)
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = "YOURBATCHFILE.bat";

                DataTable dt = polygons.DataTable;
                int activeRow = -1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["HARBLKID"] == name)
                    {
                        activeRow = i;
                    }
                }
                
                IFeatureList allPoints = null;
                List<string> batchcommands = new List<string>();
                foreach (string file in Directory.EnumerateFiles(destinationDataDir + name))
                {

                    if (file.EndsWith(".shp"))
                    {
                        if (batchcommands.Count == 0)
                        {
                            string cmd = "\"c:\\program files (x86)\\postgresql\\9.2\\bin\\shp2pgsql\" -d \"{0}\" {1} > {1}.sql";
                            batchcommands.Add(String.Format(cmd, file, name));
                        }
                        else
                        {
                            string cmd = "\"c:\\program files (x86)\\postgresql\\9.2\\bin\\shp2pgsql\" -a \"{0}\" {1} >> {1}.sql";
                            batchcommands.Add(String.Format(cmd, file, name));
                        }
                    }
                }

                string cmd2 = "\"c:\\Program Files (x86)\\postgresql\\9.2\\bin\\psql\" -h localhost -U postgres -p 5434 -d postgis_21_sample -f {0}.sql >> log";

                string cmd3= @"DELETE 
                FROM {0}
	                USING (SELECT geom FROM polygons WHERE harblkid = '{0}') as a
                WHERE NOT st_contains(a.geom, {0}.geom)";
                batchcommands.Add(string.Format(cmd2, name.ToUpper()));
                File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + name + ".bat", batchcommands.ToArray());
                p.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + name + ".bat";
                p.Start();
                p.WaitForExit();
            }
        }

        private void GetSettingsFromForm()
        {
            PostProcessingSettings settings = new PostProcessingSettings();
            settings.DensityCats = new List<double>();
            settings.DensityCats.Add(Convert.ToDouble(txt_H1Max.Text));
            settings.DensityCats.Add(Convert.ToDouble(txt_H2Max.Text));
            settings.DensityCats.Add(Convert.ToDouble(txt_H3Max.Text));
            settings.DensityCats.Add(Convert.ToDouble(txt_H4Max.Text));
            settings.DestinationDataDir = txt_DataDestination.Text;
            settings.SourceDataDir = txt_DataSource.Text;
            
            BlockSummary.heightClasses = settings.DensityCats;
            BlockSummary.NDREMin = Convert.ToDouble(txt_NDREmin.Text);
            BlockSummary.NDREMax = Convert.ToDouble(txt_NDREMax.Text);
            BlockSummary.NDVIMin = Convert.ToDouble(txt_ndviMin.Text);
            BlockSummary.NDVIMax = Convert.ToDouble(txt_ndviMax.Text);
            
            settings.SaveSettings();
        }


        private void ZipShapefiles(List<string> FoldersWithShapefiles)
        {
            foreach (string name in FoldersWithShapefiles)
            {
                ZipFile.CreateFromDirectory(destinationDataDir + name, destinationDataDir + name + ".zip");
                Directory.Delete(destinationDataDir + name, true);
                Directory.CreateDirectory(destinationDataDir + name);
            }
        }

        private void ExportShapefiles(List<string> FoldersWithShapefiles)
        {
            string connstring = "Server=localhost;Port=5434;User Id=postgres;Password=postgres;Database=postgis_21_sample;";
            string cmd = "\"c:\\program files (x86)\\postgresql\\9.2\\bin\\pgsql2shp\" -f \"{1}\\{0}\\{0}\" -h localhost -u postgres -P postgres -p 5434 postgis_21_sample {0}";
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "YOURBATCHFILE.bat";

            List<string> batchcommands = new List<string>();
            foreach (string name in FoldersWithShapefiles)
            {
                batchcommands.Add(string.Format(cmd, name.ToLower(), destinationDataDir));

            }

            File.WriteAllLines("pgsql2shp.bat", batchcommands.ToArray());
            p.StartInfo.FileName = "pgsql2shp.bat";
            p.Start();
            p.WaitForExit();
        }

        private void RunReportWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            WriteReport();
        }

        private void RunReportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lbl_Progress.Text = "Processing block " + e.UserState.ToString() + "...";
        }

        private void RunReportWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btn_Process.Enabled = true;
        }

        private void data_destination_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (DialogResult.OK == fb.ShowDialog())
            {
                txt_DataDestination.Text = fb.SelectedPath + "\\";
                settings.DestinationDataDir = txt_DataDestination.Text;
                CreateFolders();
            }
        }

    }
}
