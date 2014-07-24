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
using System.Diagnostics;
using Npgsql;

namespace PostProcessing
{
    public partial class Form1 : Form
    {
        string dataDir = @"C:\PostProcessing\";
        string PolygonShapefile;
        IFeatureSet polygons;
        string reportPath = @"C:\users\public\documents\postprocessing\BlockReport.xlsx";
        List<string> FolderNames = new List<string>();
        public Form1()
        {
            PolygonShapefile = dataDir + @"polygons\SOURCEPOLYGON.shp";
            reportPath = dataDir + @"BlockReport.xlsx";
            InitializeComponent();
            CreateFolders();
            //PopulateDatabase();
        }

        private void CreateFolders()
        {
            polygons = FeatureSet.Open(PolygonShapefile);
            polygons.FillAttributes();
            DataTable dt = polygons.DataTable;
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    FolderNames.Add((string)dt.Rows[i]["HARBLKID"]);
                }
                catch (InvalidCastException err)
                {

                }
                object[] original = dt.Rows[i].ItemArray;
            }

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

                string connstring = "Server=localhost;Port=5434;User Id=postgres;Password=postgres;Database=postgis_sample;";
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
                            string cmd = "\"c:\\program files\\postgresql\\9.2\\bin\\shp2pgsql\" -d \"{0}\" {1} > {1}.sql";
                            batchcommands.Add(String.Format(cmd, file, name));
                        }
                        else
                        {
                            string cmd = "\"c:\\program files\\postgresql\\9.2\\bin\\shp2pgsql\" -a \"{0}\" {1} >> {1}.sql";
                            batchcommands.Add(String.Format(cmd, file, name));
                        }
                    }
                }

                string cmd2 = "\"c:\\program files\\postgresql\\9.3\\bin\\psql\" -h localhost -U postgres -p 5434 -d postgis_21_sample -f {0}.sql";
                batchcommands.Add(string.Format(cmd2, name));
                string cmd3= @"DELETE 
                FROM {0}
	                USING (SELECT geom FROM polygons WHERE harblkid = '{0}') as a
                WHERE NOT st_contains(a.geom, {0}.geom)";
                batchcommands.Add(string.Format(cmd2, name.ToUpper()));
                File.WriteAllLines(name + ".bat", batchcommands.ToArray());
                p.StartInfo.FileName = name + ".bat";
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

            settings.HeightCats = new List<double>();
            settings.HeightCats.Add(Convert.ToDouble(txt_NDVIDeadMax.Text));
            settings.HeightCats.Add(Convert.ToDouble(txt_NDVIStressedMax.Text));
            settings.HeightCats.Add(Convert.ToDouble(txt_NDVIHealthyMax.Text));
            BlockSummary.heightClasses = settings.DensityCats;

        }
    }
}
