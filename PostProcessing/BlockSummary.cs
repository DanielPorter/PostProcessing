using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Data;
using DotSpatial.Topology;
using System.Diagnostics;
using System.Data;
namespace PostProcessing
{
    public class BlockSummary
    {
        public string Date;
        // The Acres value is determined from the block polygon: polygon.Area
        public double Acres;

        // Row spacing is obtained from the block feature. Row spacing is the 
        // nominal space between planted rows of citrus trees
        public double RowSpacing;

        // TreeSpacing is obtained from the block feature. It is defined as
        // the spacing of the trees in the grove as "originally planted".
        // Actual plantings may vary significantly.
        public double TreeSpacing;

        // AdjustedTreeSpacing is obtained from the block feature. It attempts
        // to account for changes in planting density over time.
        public double AdjustedTreeSpacing;

        // Linear Feet is defined as the total square feet (Acres * 43560) 
        // divided by RowSpacing, or (Acres * 43560) / RowSpacing
        public double TotalLinearFeet;
        
        // TotalPointCount is defined as the sum of the point counts for
        // each height category located inside of the polygon boundary
        public double TotalPointCount;

        // Point percentage is defined as HeightCategoryPointCount/TotalPointCount,
        // e.g. H1PointCount/TotalPointCount
        public double H1PointPercentage;
        public double H2PointPercentage;
        public double H3PointPercentage;
        public double H4PointPercentage;
        public double H5PointPercentage;

        // Acres for each height category are defined as the respective
        // PointPercentage * Acreage.
        public double H1Acres;
        public double H2Acres;
        public double H3Acres;
        public double H4Acres;
        public double H5Acres;

        // MaxTreeCount is defined as the TotalLinearFeet * TreeSpacing, or
        // if it is not null, AdjustedTreeSpacing
        public double MaxTreeCount;

        // TreeCount is defined as the sum of the tree counts of H1,
        // H2, H3, H4, and H5.
        public double TotalTreeCount;

        // HeightCatTreeCount is defined as PointPercentage * TreeCount,
        // e.g. H1PointPercentage * TreeCount.
        public double H1Treecount;
        public double H2Treecount;
        public double H3Treecount;
        public double H4Treecount;
        public double H5Treecount;
        
        // HeightCatTreeCoutn is defined as PointPercentage * TotalLinearFeet,
        // e.g. H1PointPercentage * LinearFeet;
        public double H1LinearFeet;
        public double H2LinearFeet;
        public double H3LinearFeet;
        public double H4LinearFeet;
        public double H5LinearFeet;

        public double H5CDH;
        public double H4CDH;
        public double H3CDH;
        public double H2CDH;
        public double H1CDH;

        // ProductiveAcres is defined as the sum of the acreages of Height Cats H2-H5.
        public double ProductiveAcres;
        
        // I don't know???
        public double BlockAcres;

        // OpenArea is defined as the sum of empty points not within 3 feet of
        // category H2 or H3 points.
        public double OpenArea;
        // SkipsLinearFeet
        public double SkipsLinearFeet;

        // Expansion Space is defined as the sum of empty points within 3 feet
        // of category H2 or H3 points.
        public double ExpansionSpace;

        public double FEETSQPERACRE = 43560;
        List<IGrouping<int, double>> groups;
        private IEnumerable<IGrouping<int, global::DotSpatial.Data.IFeature>> enumerable;
        public static List<double> heightClasses;
        public static List<double> NDREClasses;
        double comparisonCount;
        public string groveID;
        public double TreeDensity;

        public int skips;

        public double NDVIDeadwood;
        public double NDVIStressed;
        public double NDVIHealthy;
        public double NDVIUnclassifiable;

        public double NDREDeadwood;
        public double NDREStressed;
        public double NDREHealthy;
        public double NDREUnclassifiable;
        private System.Data.DataTable pointsDataTable;
        private double p1;
        private double p2;
        private double p3;
        private double AdjustedTreeSpacing1;
        private string name;
        private double MeanTreeDensity;
        public double canopyVolume = 0;
        public BlockSummary()
        {
            
        }

        public BlockSummary(IFeatureList featurelist, 
            double acres, double rowspacing, double treeSpacing, double adjustedTreeSpacing, string id, double treeDensity)
        {

            this.groveID = id;
            this.groups = Classify(featurelist).ToList();
            var groupKeys = groups.Select(x => x.Key).ToList();
            this.TreeDensity = treeDensity;
            this.Date = Convert.ToString(featurelist[0].DataRow["TIME"]).Substring(0, 9);
            
            this.Acres = acres;
            this.RowSpacing = rowspacing;
            this.TreeSpacing = treeSpacing;
            this.AdjustedTreeSpacing = adjustedTreeSpacing;

            this.TotalLinearFeet = this.Acres * FEETSQPERACRE / this.RowSpacing;

            TotalPointCount = featurelist.Count;
            H1PointPercentage = (groups[1].Count() - 1) / TotalPointCount;
            H2PointPercentage = (groups[2].Count() - 1) / TotalPointCount;
            H3PointPercentage = (groups[3].Count() - 1) / TotalPointCount;
            H4PointPercentage = (groups[4].Count() - 1) / TotalPointCount;
            H5PointPercentage = (groups[5].Count() - 1) / TotalPointCount;

            H1Acres = this.Acres * H1PointPercentage;
            H2Acres = this.Acres * H2PointPercentage;
            H3Acres = this.Acres * H3PointPercentage;
            H4Acres = this.Acres * H4PointPercentage;
            H5Acres = this.Acres * H5PointPercentage;
            ProductiveAcres = H2Acres + H3Acres + H4Acres + H5Acres;

            H1LinearFeet = H1PointPercentage * TotalLinearFeet;
            H2LinearFeet = H2PointPercentage * TotalLinearFeet;
            H3LinearFeet = H3PointPercentage * TotalLinearFeet;
            H4LinearFeet = H4PointPercentage * TotalLinearFeet;
            H5LinearFeet = H5PointPercentage * TotalLinearFeet;

            double RealTreeSpacing = AdjustedTreeSpacing != 0 ? AdjustedTreeSpacing : TreeSpacing;
            H1Treecount = H1LinearFeet / RealTreeSpacing;
            H2Treecount = H2LinearFeet / RealTreeSpacing;
            H3Treecount = H3LinearFeet / RealTreeSpacing;
            H4Treecount = H4LinearFeet / RealTreeSpacing;
            H5Treecount = H5LinearFeet / RealTreeSpacing;
            TotalTreeCount = (H1LinearFeet + H2LinearFeet + H3LinearFeet + H4LinearFeet + H5LinearFeet) / RealTreeSpacing;
            //OpenArea = groups[0].Count() / TotalLinearFeet;
            CalculateExpansionSpace2(featurelist);

            //double expansionSpace = CalculateExpansionSpace();
            //SkipsLinearFeet = expansionSpace;

            ClassifyWoodTypes(featurelist);

            NDREClassification(featurelist);
            REDEDGEClassification(featurelist);
            //featurelist.
        }

        public BlockSummary(System.Data.DataTable pointsDataTable, 
            double acres, double rowspacing, double treeSpacing, double adjustedTreeSpacing, string id, double treeDensity)
        {
            // TODO: Complete member initialization
            this.pointsDataTable = pointsDataTable;

            this.groveID = id;
            this.groups = Classify(pointsDataTable).ToList();
            var groupKeys = groups.Select(x => x.Key).ToList();
            this.TreeDensity = treeDensity;
            this.Date = Convert.ToString(pointsDataTable.Rows[0]["TIME"]).Substring(0, 9);

            this.Acres = acres;
            this.RowSpacing = rowspacing;
            this.TreeSpacing = treeSpacing;
            this.AdjustedTreeSpacing = adjustedTreeSpacing;

            this.TotalLinearFeet = this.Acres * FEETSQPERACRE / this.RowSpacing;

            TotalPointCount = pointsDataTable.Rows.Count;
            H1PointPercentage = (groups[1].Count() - 1) / TotalPointCount;
            H2PointPercentage = (groups[2].Count() - 1) / TotalPointCount;
            H3PointPercentage = (groups[3].Count() - 1) / TotalPointCount;
            H4PointPercentage = (groups[4].Count() - 1) / TotalPointCount;
            H5PointPercentage = (groups[5].Count() - 1) / TotalPointCount;

            H1Acres = this.Acres * H1PointPercentage;
            H2Acres = this.Acres * H2PointPercentage;
            H3Acres = this.Acres * H3PointPercentage;
            H4Acres = this.Acres * H4PointPercentage;
            H5Acres = this.Acres * H5PointPercentage;
            ProductiveAcres = H2Acres + H3Acres + H4Acres + H5Acres;

            H1LinearFeet = H1PointPercentage * TotalLinearFeet;
            H2LinearFeet = H2PointPercentage * TotalLinearFeet;
            H3LinearFeet = H3PointPercentage * TotalLinearFeet;
            H4LinearFeet = H4PointPercentage * TotalLinearFeet;
            H5LinearFeet = H5PointPercentage * TotalLinearFeet;

            double RealTreeSpacing = AdjustedTreeSpacing != 0 ? AdjustedTreeSpacing : TreeSpacing;
            H1Treecount = H1LinearFeet / RealTreeSpacing;
            H2Treecount = H2LinearFeet / RealTreeSpacing;
            H3Treecount = H3LinearFeet / RealTreeSpacing;
            H4Treecount = H4LinearFeet / RealTreeSpacing;
            H5Treecount = H5LinearFeet / RealTreeSpacing;
            TotalTreeCount = (H1LinearFeet + H2LinearFeet + H3LinearFeet + H4LinearFeet + H5LinearFeet) / RealTreeSpacing;
            OpenArea = groups[0].Count() / TotalLinearFeet;
            CalculateExpansionSpace3(pointsDataTable);


            NDREClassification(pointsDataTable);
            REDEDGEClassification(pointsDataTable);
            this.name = name;
            this.MeanTreeDensity = MeanTreeDensity;

            bool distancePresent = false;
            foreach (DataColumn c in pointsDataTable.Columns)
            {
                if (c.ColumnName == "DISTANCE")
                {
                    distancePresent = true;
                }
            }
            if (distancePresent)
            {
                CalculateVolume();
            }

        }

        private void CalculateVolume()
        {
            // V=PI*a*b*h, where a = tree height, b = distance, and h = the distance between points, 1.0ft
            canopyVolume = 
                pointsDataTable.AsEnumerable().Sum(
                    x => Math.PI * Convert.ToDouble(x["HEIGHT"]) * Convert.ToDouble(x["DISTANCE"]) * 1.0
                );
        }

        private void REDEDGEClassification(DataTable pointsDataTable)
        {
                        List<int> deadwoodIndexes = new List<int>();
            
            for(int i = 0; i < pointsDataTable.Rows.Count; i++)
            {
                DataRow feature = pointsDataTable.Rows[i];
                double TopRE = Convert.ToDouble(feature["TNDVI"]);
                double BottomRE = Convert.ToDouble(feature["TNDVI"]);
                if (Convert.ToDouble(feature["HEIGHT"]) > 7)
                {
                    if (TopRE > 1 | TopRE < -1 | BottomRE > 1 | BottomRE < -1)
                    {
                        NDVIUnclassifiable++;
                    }
                    else if (TopRE > 0)
                    {
                        NDVIHealthy++;
                    }
                    else if (BottomRE > 0)
                    {
                        NDVIStressed++;
                    }
                    else if (BottomRE < 0)
                    {
                        deadwoodIndexes.Add(i);
                    }
                }
                else if (Convert.ToDouble(feature["HEIGHT"]) > 3)
                {
                    if (Convert.ToDouble(feature["DENSITY"]) > .75)
                    {
                        NDVIHealthy++;
                    }
                    else
                    {
                        NDVIUnclassifiable++;
                    }
                }
            }
            List<int> indexesToRemove = new List<int>();
            for(int i = 0; i < deadwoodIndexes.Count; i++)
            {
                int index = deadwoodIndexes[i];
                if(i > 0 && index - deadwoodIndexes[i - 1] == 1)
                {
                    // keep
                }
                else
                {
                    if(i < deadwoodIndexes.Count - 1 && index - deadwoodIndexes[i + 1] == -1)
                    {

                    }
                    else
                    {
                        indexesToRemove.Add(index);
                    }
                }
            }

            NDVIStressed += indexesToRemove.Count;
            NDVIDeadwood += deadwoodIndexes.Count - indexesToRemove.Count;
            NDVIStressed = NDVIStressed / TotalPointCount;
            NDVIDeadwood = NDVIDeadwood / TotalPointCount;
            NDVIHealthy = NDVIHealthy / TotalPointCount;
            NDVIUnclassifiable = NDVIUnclassifiable / TotalPointCount;
        }

        private void NDREClassification(DataTable pointsDataTable)
        {
            List<int> deadwoodIndexes = new List<int>();

            for (int i = 0; i < pointsDataTable.Rows.Count; i++)
            {
                DataRow feature = pointsDataTable.Rows[i];
                if (Convert.ToDouble(feature["HEIGHT"]) > 7)
                {
                    double TNDRE = Convert.ToDouble(feature["TNDRE"]);
                    double BNDRE = Convert.ToDouble(feature["BNDRE"]);

                    if (TNDRE > 1.5 | TNDRE < -1.5 | BNDRE > 1.5 | BNDRE < -1.5)
                    {
                        NDREUnclassifiable++;
                    }
                    else if (TNDRE > 0)
                    {
                        NDREHealthy++;
                    }
                    else if (BNDRE > 0)
                    {
                        NDREStressed++;
                    }
                    else
                    {
                        deadwoodIndexes.Add(i);
                    }
                }
                else if (Convert.ToDouble(feature["HEIGHT"]) > 3)
                {
                    if (Convert.ToDouble(feature["DENSITY"]) > .75)
                    {
                        NDREHealthy++;
                    }
                }
            }
            List<int> indexesToRemove = new List<int>();
            for (int i = 0; i < deadwoodIndexes.Count; i++)
            {
                int index = deadwoodIndexes[i];
                if (i > 0 && index - deadwoodIndexes[i - 1] == 1)
                {
                    // keep
                }
                else
                {
                    if (i < deadwoodIndexes.Count - 1 && index - deadwoodIndexes[i + 1] == -1)
                    {
                        //keep
                    }
                    else
                    {
                        indexesToRemove.Add(index);
                    }
                }
            }
            NDREStressed += indexesToRemove.Count;
            NDREDeadwood += deadwoodIndexes.Count - indexesToRemove.Count;

            NDREStressed = NDREStressed / TotalPointCount;
            NDREHealthy = NDREHealthy / TotalPointCount;
            NDREDeadwood = NDREDeadwood / TotalPointCount;
            NDREUnclassifiable = NDREUnclassifiable / TotalPointCount;
            double ndrepointcount = NDREStressed + NDREDeadwood + NDREHealthy + NDREUnclassifiable;
        }

        private void REDEDGEClassification(IFeatureList featurelist)
        {
            List<int> deadwoodIndexes = new List<int>();
            
            for(int i = 0; i < featurelist.Count; i++)
            {
                IFeature feature = featurelist[i];
                double TopRE = Convert.ToDouble(feature.DataRow["TNDVI"]);
                double BottomRE = Convert.ToDouble(feature.DataRow["TNDVI"]);

                if (TopRE > 1 | TopRE < -1 | BottomRE > 1 | BottomRE < -1)
                {
                    NDVIUnclassifiable++;
                }
                else if (TopRE > 0)
                {
                    NDVIHealthy++;
                }
                else if (BottomRE > 0)
                {
                    NDVIStressed++;
                }
                else if (BottomRE < 0)
                {
                    deadwoodIndexes.Add(i);
                }

            }
            List<int> indexesToRemove = new List<int>();
            for(int i = 0; i < deadwoodIndexes.Count; i++)
            {
                int index = deadwoodIndexes[i];
                if(i > 0 && index - deadwoodIndexes[i - 1] == 1)
                {
                    // keep
                }
                else
                {
                    if(i < deadwoodIndexes.Count - 1 && index - deadwoodIndexes[i + 1] == -1)
                    {

                    }
                    else
                    {
                        indexesToRemove.Add(index);
                    }
                }
            }

            NDVIStressed += indexesToRemove.Count;
            NDVIDeadwood += deadwoodIndexes.Count - indexesToRemove.Count;
            NDVIStressed = NDVIStressed / TotalPointCount;
            NDVIDeadwood = NDVIDeadwood / TotalPointCount;
            NDVIHealthy = NDVIHealthy / TotalPointCount;
            NDVIUnclassifiable = NDVIUnclassifiable / TotalPointCount;
        }


        private void NDREClassification(IFeatureList featurelist)
        {
            List<int> deadwoodIndexes = new List<int>();

            for (int i = 0; i < featurelist.Count; i++)
            {
                IFeature feature = featurelist[i];
                if (Convert.ToDouble(feature.DataRow["HEIGHT"]) > 7)
                {
                    double TNDRE = Convert.ToDouble(feature.DataRow["TNDRE"]);
                    double BNDRE = Convert.ToDouble(feature.DataRow["BNDRE"]);

                    if (TNDRE > 1.5 | TNDRE < -1.5 | BNDRE > 1.5 | BNDRE < -1.5)
                    {
                        NDREUnclassifiable++;
                    }
                    else if (TNDRE > 0)
                    {
                        NDREHealthy++;
                    }
                    else if (BNDRE > 0)
                    {
                        NDREStressed++;
                    }
                    else
                    {
                        deadwoodIndexes.Add(i);
                    }
                }
            }
            List<int> indexesToRemove = new List<int>();
            for (int i = 0; i < deadwoodIndexes.Count; i++)
            {
                int index = deadwoodIndexes[i];
                if (i > 0 && index - deadwoodIndexes[i - 1] == 1)
                {
                    // keep
                }
                else
                {
                    if (i < deadwoodIndexes.Count - 1 && index - deadwoodIndexes[i + 1] == -1)
                    {
                        //keep
                    }
                    else
                    {
                        indexesToRemove.Add(index);
                    }
                }
            }
            NDREStressed += indexesToRemove.Count;
            NDREDeadwood += deadwoodIndexes.Count - indexesToRemove.Count;

            NDREStressed = NDREStressed / TotalPointCount;
            NDREHealthy = NDREHealthy / TotalPointCount;
            NDREDeadwood = NDREDeadwood / TotalPointCount;
            NDREUnclassifiable = NDREUnclassifiable / TotalPointCount;
            double ndrepointcount = NDREStressed + NDREDeadwood + NDREHealthy + NDREUnclassifiable;
        }

        private void CalculateExpansionSpace3(System.Data.DataTable featureTable)
        {

            List<System.Data.DataRow> emptyPoints = featureTable.AsEnumerable().Where(x => Convert.ToDouble(x["HEIGHT"]) == 0).ToList();

            for (int i = 0; i < featureTable.Rows.Count; i++)
            {
                int heightclass = AssignHeightClass(Convert.ToDouble(featureTable.Rows[i]["HEIGHT"]));
                if (heightclass == 1 | heightclass == 2)
                {
                    for (int j = i - 3; j < i + 3; j++)
                    {
                        try
                        {
                            if (Convert.ToDouble(featureTable.Rows[j]["HEIGHT"]) == 0)
                            {
                                ExpansionSpace++;
                                emptyPoints.Remove(featureTable.Rows[j]);
                            }
                        }
                        catch (Exception err)
                        {

                        }
                    }
                }
            }
            OpenArea = emptyPoints.Count / TotalLinearFeet * Acres;
        }
        private void CalculateExpansionSpace2(IFeatureList featurelist)
        {

            List<IFeature> emptyPoints = featurelist.Where(x => Convert.ToDouble(x.DataRow["HEIGHT"]) == 0).ToList();

            for (int i = 0; i < featurelist.Count; i++)
            {
                int heightclass = AssignHeightClass(Convert.ToDouble(featurelist[i].DataRow["HEIGHT"]));
                if (heightclass == 1 | heightclass == 2)
                {
                    for (int j = i - 3; j < i + 3; j++)
                    {
                        try
                        {
                            if (Convert.ToDouble(featurelist[j].DataRow["HEIGHT"]) == 0)
                            {
                                ExpansionSpace++;
                                emptyPoints.Remove(featurelist[j]);
                            }
                        }
                        catch (Exception err)
                        {

                        }
                    }
                }
            }
            OpenArea = emptyPoints.Count / TotalLinearFeet * Acres;
        }

        private void ClassifyWoodTypes(IFeatureList featurelist)
        {
            var classified = featurelist.GroupBy(x => GetWoodType(x)).ToDictionary(x=> x.Key);
            foreach (var cl in classified)
            {
               
            }
        }

        public string GetWoodType(IFeature feature)
        {
            
            if (Convert.ToDouble(feature.DataRow["TREDEDGE"]) < 0)
            {
                if (Convert.ToDouble(feature.DataRow["BREDEDGE"]) < 0)
                {
                    return "dead";
                }
                else
                {
                    return "stressed";
                }
            }
            else
                return "healthy";

        }


        //public double CalculateExpansionSpace()
        //{
        //    System.Timers.Timer timer = new System.Timers.Timer();
        //    timer.Elapsed += timer_Elapsed;
        //    List<IFeature> features = groups[1].ToList();
        //    features.AddRange(groups[2].ToList());
        //    FeatureSet fs = new FeatureSet(features);
        //    progress prg = new progress();
        //    IFeatureSet buffers = fs.Buffer(3.1 * 0.3048, false);
        //    buffers.SaveAs(@"C:\users\public\documents\buffers.shp", true);
        //    IFeatureSet points = new FeatureSet(groups[0].ToList());
        //    points.SaveAs(@"C:\users\public\documents\points.shp", true);
        //    List<IFeature> zeroPoints = groups[0].ToList();
        //    double count = 0;
        //    comparisonCount = 0;
        //    buffers.IndexMode = true;
        //    timer.Start();
        //    //IFeatureSet unioned = buffers.UnionShapes(ShapeRelateType.All);
        //    IGeometry union = null;
        //    foreach (IFeature feat in buffers.Features)
        //    {
        //        for (int i = zeroPoints.Count - 1; i >= 0; i--)
        //        {
        //            comparisonCount++;
        //            if (feat.Intersects(zeroPoints[i]))
        //            {
        //                count++;
        //                //Console.WriteLine("AN INTERSECTION, {0}", count);
        //                zeroPoints.RemoveAt(i);
        //            }
        //        }
        //    }
        //    IFeatureSet remainingPoints = new FeatureSet(zeroPoints);
        //    remainingPoints.SaveAs(@"C:\users\public\documents\remaining_points.shp", true);
        //    IFeatureSet ExpansionSpace = fs.Buffer(3.1 * 0.3048, false).Intersection(new FeatureSet(groups[0].ToList()), FieldJoinType.ForeignOnly, prg);
        //    return ExpansionSpace.Features.Count() - groups[0].Count();
        //}

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("COMPARISONS PER SECOND: {0}", comparisonCount);
            comparisonCount = 0;
        }

        public class progress : IProgressHandler
        {
            public void Progress(string key, int percentage, string message)
            {
                Console.WriteLine("{0}, {1}, {2}", key, percentage, message);
            }
        }

        private static System.Collections.Generic.IEnumerable<IGrouping<int, double>> Classify(IFeatureList featureList)
        {
            List<double> heights = featureList.Select(x => Convert.ToDouble(x.DataRow["HEIGHT"])).ToList();
            foreach (double heightcat in heightClasses)
            {
                heights.Add(heightcat - 1);
            }
            heights.Add(0);
            heights.Add(heightClasses.Last() + 1);
            return heights.GroupBy(x => AssignHeightClass(x));
        }
        private static System.Collections.Generic.IEnumerable<IGrouping<int, double>> Classify(System.Data.DataTable featureTable)
        {
            List<double> heights = featureTable.AsEnumerable().Select(x => Convert.ToDouble(x["HEIGHT"])).ToList();
            foreach (double heightcat in heightClasses)
            {
                heights.Add(heightcat - 1);
            }
            heights.Add(0);
            heights.Add(heightClasses.Last() + 1);
            return heights.GroupBy(x => AssignHeightClass(x));
        }
        private static int AssignHeightClass(double height)
        {
            if (height == 0)
            {
                return 0;
            }
            for (int i = 0; i < heightClasses.Count; i++)
            {
                if (height < heightClasses[i])
                    return i + 1;
            }
            return heightClasses.Count + 1;
        }

    }
}
