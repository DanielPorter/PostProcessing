using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace PostProcessing
{
    public partial class PostProcessing : Form
    {
        private void createReport_click(object sender, EventArgs e)
        {
            GetSettingsFromForm();
            btn_Process.Enabled = false;
            RunReportWorker.RunWorkerAsync();
        }

        private void btn_DataDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = Environment.SpecialFolder.MyComputer;
            if (DialogResult.OK == fb.ShowDialog())
            {
                txt_DataSource.Text = fb.SelectedPath + "\\";
                settings.SourceDataDir = txt_DataSource.Text;
                CreateFolders();
            }
        }

        private void loadShapefiles_Click(object sender, EventArgs e)
        {
            string tableName = "laserpoints";
            LoadShapefiles(FindPsqlDir(), tableName, settings.SourceDataDir);

            MessageBox.Show("POPULATING DATABASE");
            //PopulateDatabase();
            List<string> FoldersWithShapefiles = new List<string>();
            foreach (string name in FolderNames)
            {
                if (Directory.EnumerateFiles(destinationDataDir + name).Where(x => x.Contains(".shp")).Count() > 0)
                {
                    FoldersWithShapefiles.Add(name);
                }
            }
        }
    }
}
