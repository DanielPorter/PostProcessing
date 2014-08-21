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
        string dataDir = @"H:\PostProcessing\";
        string PolygonShapefile;
        IFeatureSet polygons;
        string reportPath = @"C:\users\public\documents\postprocessing\BlockReport.xlsx";
        string[] FolderNames;
        string PsqlDir;
        public PostProcessing()
        {
            dataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\PostProcessing\\";
            PolygonShapefile = dataDir + @"\SOURCEPOLYGON.shp";
            FindPsqlDir();
            try
            {
                polygons = FeatureSet.Open(PolygonShapefile);
                FolderNames = polygons.Features.AsEnumerable().Select(x => (string)x.DataRow["harblkid"]).ToArray();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\n" + err.StackTrace);
            }
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            reportPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + @"BlockReport.xlsx";
            InitializeComponent();
            txt_DataDirectory.Text = dataDir;
            CreateFolders();


        }

        private void FindPsqlDir()
        {
            if (File.Exists(@"c:\program files (x86)\postgresql\9.2\bin\psql.exe"))
            {
                PsqlDir = @"c:\program files (x86)\postgresql\9.2\bin\psql.exe";
            }
            else if (File.Exists(@"c:\program files\postgresql\9.2\bin\psql.exe"))
            {
                PsqlDir = @"c:\program files\postgresql\9.2\bin\psql.exe";
            }
            else
                MessageBox.Show("Unable to locate psql, which is required for interfacing with PostgreSQL. Program can't run.");
        }



        private void CreateFolders()
        {
            polygons.FillAttributes();
            DataTable dt = polygons.DataTable;


            //System.IO.File.WriteAllLines(@"C:\Users\Public\documents\blockids.txt", FolderNames);
            foreach(string folder in FolderNames)
            {
                if (!Directory.Exists(dataDir + folder))
                {
                    Directory.CreateDirectory(dataDir + folder);

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetSettingsFromForm();
            WriteReport();
        }

        private void WriteReport()
        {
            List<string> FoldersWithShapefiles = new List<string>();
            foreach (string name in FolderNames)
            {
                if (Directory.EnumerateFiles(dataDir + name).Count() > 0)
                {
                    FoldersWithShapefiles.Add(name);
                }
            }
            BlockReport report = new BlockReport(reportPath);
            FoldersWithShapefiles.Remove("LKP0801E&M");
            //FoldersWithShapefiles.Add("LKP0101VAL");
            //FoldersWithShapefiles.Add("LKP0102PIN");
            for(int j = 0; j < FoldersWithShapefiles.Count; j++)
            {
                
                string name = FoldersWithShapefiles[j];
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
                RunReportWorker.ReportProgress(Convert.ToInt32(Convert.ToDouble(j) / FoldersWithShapefiles.Count * 100));
                string connstring = "Server=localhost;Port=5434;User Id=postgres;Password=postgres;Database=postgis_21_sample;";
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();

                string query = "select * from {0};";
                query = string.Format(query, name);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                System.Data.DataSet ds = new System.Data.DataSet();
                ds.Reset();
                da.Fill(ds);
                
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
                //MessageBox.Show(String.Format("Active row: {0}, name: {1}, meantreedensity: {2}, rowspacing {3}", 
                //    activeRow, name, MeanTreeDensity,
                //    Convert.ToDouble(dt.Rows[activeRow]["Row_Spacin"])
                //    ));
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
            RunReportWorker.ReportProgress(100);

        }

        private void PopulateDatabase()
        {
            List<string> FoldersWithShapefiles = new List<string>();
            foreach (string name in FolderNames)
            {
                if (Directory.EnumerateFiles(dataDir + name).Count() > 0)
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
                foreach (string file in Directory.EnumerateFiles(dataDir + name))
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

            BlockSummary.heightClasses = settings.DensityCats;
            BlockSummary.NDREMin = Convert.ToDouble(txt_NDREmin.Text);
            BlockSummary.NDREMax = Convert.ToDouble(txt_NDREMax.Text);
            BlockSummary.NDVIMin = Convert.ToDouble(txt_ndviMin.Text);
            BlockSummary.NDVIMax = Convert.ToDouble(txt_ndviMax.Text);

        }

        private void btn_DataDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (DialogResult.OK == fb.ShowDialog())
            {
                txt_DataDirectory.Text = fb.SelectedPath + "\\";
                dataDir = txt_DataDirectory.Text;
                CreateFolders();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        
        {
            MessageBox.Show("POPULATING DATABASE");
            PopulateDatabase();
            List<string> FoldersWithShapefiles = new List<string>();
            foreach (string name in FolderNames)
            {
                if (Directory.EnumerateFiles(dataDir + name).Where(x => x.Contains(".shp")).Count() > 0)
                {
                    FoldersWithShapefiles.Add(name);
                }
            }
            //ZipShapefiles(FoldersWithShapefiles);
            //ExportShapefiles(FoldersWithShapefiles);
        }

        private void ZipShapefiles(List<string> FoldersWithShapefiles)
        {
            foreach (string name in FoldersWithShapefiles)
            {
                ZipFile.CreateFromDirectory(dataDir + name, dataDir + name + ".zip");
                Directory.Delete(dataDir + name, true);
                Directory.CreateDirectory(dataDir + name);
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
                batchcommands.Add(string.Format(cmd, name.ToLower(), dataDir));

            }

            File.WriteAllLines("pgsql2shp.bat", batchcommands.ToArray());
            p.StartInfo.FileName = "pgsql2shp.bat";
            p.Start();
            p.WaitForExit();
        }

        private void RunReportWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void RunReportWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
    }
}
