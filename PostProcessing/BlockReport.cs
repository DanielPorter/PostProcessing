using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml;
using System.IO;
namespace PostProcessing
{
    class BlockReport
    {
        ExcelPackage package;
        ExcelWorksheet worksheet;
        int currentRow = 10;
        public BlockReport(string path)
        {
            File.Copy("REPORT_FORMAT.xlsx", @"C:\Users\public\documents\REPORT.xlsx", true);
            FileInfo newFile = new FileInfo(@"C:\users\public\documents\REPORT.xlsx");

            package = new ExcelPackage(newFile);
            worksheet = package.Workbook.Worksheets["Inventory"];
            worksheet.Cells[2, 1].Value = "GROVE ID";
            worksheet.Cells[2, 2].Value = "Total Acres";
            worksheet.Cells[1, 3].Value = "Height Category Acres";
            worksheet.Cells[2, 3].Value = "H5";
            worksheet.Cells[2, 4].Value = "H4";
            worksheet.Cells[2, 5].Value = "H3";
            worksheet.Cells[2, 6].Value = "H2";
            worksheet.Cells[2, 7].Value = "H1";
            worksheet.Cells[2, 8].Value = "Total Tree Count";
            worksheet.Cells[1, 9].Value = "Height Category Tree Count";
            worksheet.Cells[2, 9].Value = "H5";
            worksheet.Cells[2, 10].Value = "H4";
            worksheet.Cells[2, 11].Value = "H3";
            worksheet.Cells[2, 12].Value = "H2";
            worksheet.Cells[2, 13].Value = "H1";
            worksheet.Cells[2, 14].Value = "Productive Acres";
            worksheet.Cells[2, 15].Value = "Open Space";
            worksheet.Cells[2, 16].Value = "Expansion Space";
        }

        public void WriteRow(BlockSummary summary)
        {
            worksheet.Cells[currentRow, 2].Value = summary.groveID;
            worksheet.Cells[currentRow, 3].Value = summary.Date;
            worksheet.Cells[currentRow, 4].Value = summary.H5LinearFeet;
            worksheet.Cells[currentRow, 5].Value = summary.H5CDH;
            worksheet.Cells[currentRow, 6].Value = summary.H4LinearFeet;
            worksheet.Cells[currentRow, 7].Value = summary.H4CDH;
            worksheet.Cells[currentRow, 8].Value = summary.H3LinearFeet;
            worksheet.Cells[currentRow, 9].Value = summary.H3CDH;
            worksheet.Cells[currentRow, 10].Value = summary.H2LinearFeet;
            worksheet.Cells[currentRow, 11].Value = summary.H2CDH;
            worksheet.Cells[currentRow, 12].Value = summary.H1LinearFeet;
            worksheet.Cells[currentRow, 13].Value = summary.H1CDH;
            worksheet.Cells[currentRow, 14].Value = summary.Acres;
            worksheet.Cells[currentRow, 15].Value = summary.TreeDensity;
            worksheet.Cells[currentRow, 16].Value = summary.H5Acres;
            worksheet.Cells[currentRow, 17].Value = summary.H4Acres;
            worksheet.Cells[currentRow, 18].Value = summary.H3Acres;
            worksheet.Cells[currentRow, 19].Value = summary.H2Acres;
            worksheet.Cells[currentRow, 20].Value = summary.H1Acres;
            worksheet.Cells[currentRow, 21].Value = summary.TotalTreeCount;
            worksheet.Cells[currentRow, 22].Value = summary.H5Treecount;
            worksheet.Cells[currentRow, 23].Value = summary.H4Treecount;
            worksheet.Cells[currentRow, 24].Value = summary.H3Treecount;
            worksheet.Cells[currentRow, 25].Value = summary.H2Treecount;
            worksheet.Cells[currentRow, 26].Value = summary.H1Treecount;
            worksheet.Cells[currentRow, 27].Value = summary.ProductiveAcres;
            worksheet.Cells[currentRow, 28].Value = summary.OpenArea;
            worksheet.Cells[currentRow, 29].Value = summary.SkipsLinearFeet;
            worksheet.Cells[currentRow, 30].Value = summary.NDREDeadwood;
            worksheet.Cells[currentRow, 31].Value = summary.NDREStressed;
            worksheet.Cells[currentRow, 32].Value = summary.NDREHealthy;
            worksheet.Cells[currentRow, 33].Value = summary.NDREUnclassifiable;
            worksheet.Cells[currentRow, 34].Value = summary.NDVIDeadwood;
            worksheet.Cells[currentRow, 35].Value = summary.NDVIStressed;
            worksheet.Cells[currentRow, 36].Value = summary.NDVIHealthy;
            worksheet.Cells[currentRow, 37].Value = summary.NDVIUnclassifiable;
            if (summary.canopyVolume != 0)
            {
                worksheet.Cells[currentRow, 38].Value = summary.canopyVolume;
            }
            currentRow++;
        }

        public void Format()
        {
            worksheet.Column(2).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(3).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(4).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(5).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(6).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(7).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(8).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(9).Style.Numberformat.Format = "#,###";
            worksheet.Column(10).Style.Numberformat.Format = "#,###";
            worksheet.Column(11).Style.Numberformat.Format = "#,###";
            worksheet.Column(12).Style.Numberformat.Format = "#,###";
            worksheet.Column(13).Style.Numberformat.Format = "#,###";
            worksheet.Column(14).Style.Numberformat.Format = "#,###";
            worksheet.Column(15).Style.Numberformat.Format = "#,###.##";
            worksheet.Column(16).Style.Numberformat.Format = "#.##";
            worksheet.Column(17).Style.Numberformat.Format = "######";
            worksheet.Column(18).Style.Numberformat.Format = "#.##";
            worksheet.Column(19).Style.Numberformat.Format = "#.##";
            worksheet.Column(20).Style.Numberformat.Format = "#.##";
        }

        internal void Save()
        {
            package.Save();
        }
    }
}
