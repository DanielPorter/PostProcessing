using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Data;
using DotSpatial.Topology;
using System.Data;
using System.IO;
using PostProcessing;
using OfficeOpenXml;

namespace ReadShapefiles
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists(@"C:\users\public\PostProcessing"))
            {
                Directory.CreateDirectory(@"C:\users\public\PostProcessing");
            }

            IFeatureSet fs = FeatureSet.Open(@"C:\users\public\documents\BAS_LAND_ALL.shp");
            
            
            DataTable dt = fs.DataTable;
            List<string> landids = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine(String.Join(",", row.ItemArray.ToList().ConvertAll(x => x.ToString())));
            }

            IFeatureSet points = FeatureSet.Open(@"C:\Users\daniel\Documents\05-28-14 14.51.03\05-28-14 14.51.03.shp");
            IFeatureList contained_points = new FeatureList();
            IFeature feat = fs.Features[0];
            System.Collections.Generic.IEnumerable<IGrouping<int, DotSpatial.Data.IFeature>> classes = Classify(points.Features);
            classes = classes.OrderBy(x => x.Key);
            List<IGrouping<int, DotSpatial.Data.IFeature>> classesList = classes.ToList();
            foreach (IGrouping<int, DotSpatial.Data.IFeature> grouping in classesList)
            {
                Console.WriteLine("{0}: {1}", grouping.Key, grouping.Count());
                
            }

            BlockSummary bs = new BlockSummary(points.Features, 113, 25, 10, 15, "ID", 122);
            List<BlockSummary> bls = new List<BlockSummary>();
            bls.Add(bs);
            WriteShapefile(bls, "asf");
            List<IFeature> features = classesList[1].ToList();
            features.AddRange(classesList[2].ToList());
            features.AddRange(classesList[3].ToList());
            features.AddRange(classesList[4].ToList());
            FeatureSet newfs = new FeatureSet(features);
            progress pgr = new progress();
            List<IFeature> feats = classesList[1].ToList();
            IFeatureSet tallest = new FeatureSet(features);
            IFeatureSet bufferedPoints = tallest.Buffer(3.5, false);
            bufferedPoints.SaveAs(@"C:\users\public\documents\buffered.shp", true);
            IFeatureSet nonscans = new FeatureSet(classesList[0].ToList());
            IFeatureSet intersections = bufferedPoints.Intersection(new FeatureSet(classesList[0].ToList()), FieldJoinType.All, pgr);
            IFeatureSet openSpaces = tallest.Buffer(3.4, false).Intersection(new FeatureSet(classesList[0].ToList()), FieldJoinType.None, null);
            IFeatureSet all = newfs.Buffer(3.4, false).UnionShapes(ShapeRelateType.All);
            IFeatureSet openAreas =all.Intersection(new FeatureSet(classesList[0].ToList()), FieldJoinType.None, null);
        }

        private static System.Collections.Generic.IEnumerable<IGrouping<int, DotSpatial.Data.IFeature>> Classify(IFeatureList featureList)
        {
            return featureList.GroupBy(x => AssignHeightClass((double)x.DataRow["HEIGHT"]));
        }

        private static int AssignHeightClass(double height)
        {
            if (height == 0)
            {
                return 0;
            }
            if (height < 5)
            {
                return 1;
            }
            else if (height < 7)
            {
                return 2;
            }
            else if (height < 9)
            {
                return 3;
            }
            else if (height < 11)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        public static void WriteShapefile(List<BlockSummary> summaries, string path)
        {
            path = System.IO.Path.GetDirectoryName(
                 System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            File.Copy("REPORT_FORMAT.xlsx", @"C:\Users\public\documents\REPORT.xlsx", true);
            FileInfo newFile = new FileInfo(@"C:\users\public\documents\REPORT.xlsx");
            
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Inventory"];

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
                for (int i = 0; i < summaries.Count; i++)
                {
                    int j = i + 3;
                    BlockSummary summary = summaries[i];
                    worksheet.Cells[j, 1].Value = summary.groveID;
                    worksheet.Cells[j, 2].Value = summary.Acres;
                    worksheet.Cells[j, 3].Value = summary.H5Acres;
                    worksheet.Cells[j, 4].Value = summary.H4Acres;
                    worksheet.Cells[j, 5].Value = summary.H3Acres;
                    worksheet.Cells[j, 6].Value = summary.H2Acres;
                    worksheet.Cells[j, 7].Value = summary.H1Acres;
                    worksheet.Cells[j, 8].Value = summary.TotalTreeCount;
                    worksheet.Cells[j, 9].Value = summary.H5Treecount;
                    worksheet.Cells[j, 10].Value = summary.H4Treecount;
                    worksheet.Cells[j, 11].Value = summary.H3Treecount;
                    worksheet.Cells[j, 12].Value = summary.H2Treecount;
                    worksheet.Cells[j, 13].Value = summary.H1Treecount;
                    worksheet.Cells[j, 14].Value = summary.ProductiveAcres;
                    worksheet.Cells[j, 15].Value = summary.OpenArea;
                    worksheet.Cells[j, 16].Value = summary.ExpansionSpace;
                }

                package.Save();
            }
        }
    }

    public class progress : IProgressHandler
    {
        public void Progress(string key, int percentage, string message)
        {
            Console.WriteLine("{0}, {1}, {2}", key, percentage, message);
        }
    }

    
}
