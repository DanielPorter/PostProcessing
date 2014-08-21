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
        public List<int> ExpansionClasses = new List<int>();

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
        public static double NDREMin;
        public static double NDREMax;
        public static double NDVIMin;
        public static double NDVIMax;
        public BlockSummary()
        {
            
        }


        public BlockSummary(System.Data.DataTable pointsDataTable, 
            double acres, double rowspacing, double treeSpacing, double adjustedTreeSpacing, string id, double treeDensity)
        {
            TotalPointCount = pointsDataTable.Rows.Count;

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

            CalculateExpansionSpace3(pointsDataTable);

            
            H1PointPercentage = (groups[1].Count() - 1 + ExpansionClasses[1]) / TotalPointCount;
            H2PointPercentage = (groups[2].Count() - 1 + ExpansionClasses[2]) / TotalPointCount;
            H3PointPercentage = (groups[3].Count() - 1 + ExpansionClasses[3]) / TotalPointCount;
            H4PointPercentage = (groups[4].Count() - 1 + ExpansionClasses[4]) / TotalPointCount;
            H5PointPercentage = (groups[5].Count() - 1 + ExpansionClasses[5]) / TotalPointCount;
            double skipspct = SkipsLinearFeet / TotalLinearFeet;
            double productivepct = H1PointPercentage + H2PointPercentage + H3PointPercentage + H4PointPercentage + H5PointPercentage;
            double totalpct = productivepct + skipspct;
            double productpctwithoutexpansion = (groups[1].Count() + groups[2].Count() + groups[3].Count() + groups[4].Count() + groups[5].Count()) / TotalPointCount;
            double opnepct = groups[0].Count() / TotalPointCount;
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


            NDREClassification(pointsDataTable);
            NDVIClassification(pointsDataTable);
            this.name = name;
            this.MeanTreeDensity = MeanTreeDensity;
            double H0PointPercentage = (groups[0].Count() - 1 + ExpansionClasses[0]) / TotalPointCount;
            OpenArea = H0PointPercentage * acres;

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

        private void NDVIClassification(DataTable pointsDataTable)
        {
                        List<int> deadwoodIndexes = new List<int>();
            
            for(int i = 0; i < pointsDataTable.Rows.Count; i++)
            {
                DataRow feature = pointsDataTable.Rows[i];
                double TopNDVI = Convert.ToDouble(feature["TNDVI"]);
                double BottomNDVI = Convert.ToDouble(feature["TNDVI"]);
                if (Convert.ToDouble(feature["HEIGHT"]) > 7)
                {
                    if (TopNDVI > NDVIMax | TopNDVI < NDVIMin | BottomNDVI > NDVIMax | BottomNDVI < NDVIMin)
                    {
                        NDVIUnclassifiable++;
                    }
                    else if (TopNDVI > 0)
                    {
                        NDVIHealthy++;
                    }
                    else if (BottomNDVI > 0)
                    {
                        NDVIStressed++;
                    }
                    else if (BottomNDVI < 0)
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

                    if (TNDRE > NDREMax | TNDRE < NDREMin | BNDRE > NDREMax | BNDRE < NDREMin)
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


        private void CalculateExp(List<int> pointsClassifiedByHeight, List<DataRow> rows, List<int> classes, List<DataRow> emptyPoints)
        {
            List<Tuple<int, int>> group_locations = new List<Tuple<int, int>>();
            int? beginning = null;
            for (int i = 0; i < pointsClassifiedByHeight.Count; i++)
            {
                if (pointsClassifiedByHeight[i] != 0)
                {
                    if (beginning != null)
                    {
                        group_locations.Add(new Tuple<int, int>( beginning.Value, i - 1));
                        beginning = null;
                    }
                }
                else
                {
                    if (beginning == null)
                    {
                        beginning = i;
                    }
                }
            }
            //for (int i = 0; i < pointsClassifiedByHeight.Count; i++)
            //{
            //    if (pointsClassifiedByHeight[i] == 0)
            //    {
            //        if (beginning != null)
            //        {
            //            group_locations.Add(new Tuple<int, int>(beginning.Value, i - 1));
            //            beginning = null;
            //        }
            //    }
            //    else
            //    {
            //        if (beginning == null)
            //        {
            //            beginning = i;
            //        }
            //        else
            //        {
            //            // Assume point is part of the group, ignore it.
            //        }
            //    }
            //}

            foreach (Tuple<int, int> group in group_locations)
            {
                int item1Length;
                int item2Length;
                int groupLength = group.Item2 - group.Item1 + 1;
                if (groupLength >= 6)
                {
                    item1Length = 3;
                    item2Length = 3;
                }
                else
                {
                    item1Length = groupLength % 2 == 0 ? (groupLength / 2) : (groupLength / 2) + 1;
                    item2Length = groupLength / 2;
                }
                for (int i = group.Item1; i < group.Item1 + item1Length & i != 0; i++)
                {
                    if (Convert.ToDouble(rows[i]["HEIGHT"]) == 0)
                    {
                        classes[pointsClassifiedByHeight[group.Item1 - 1]]++;
                        if (!emptyPoints.Remove(rows[i]))
                        {

                        }
                    }
                    else
                        break;
                }
                for (int i = group.Item2; i > group.Item2 - item2Length & i != pointsClassifiedByHeight.Count - 1; i--)
                {
                    if (Convert.ToDouble(rows[i]["HEIGHT"]) == 0)
                    {
                        classes[pointsClassifiedByHeight[group.Item2 + 1]]++;
                        if(!emptyPoints.Remove(rows[i]))
                        {
                            
                        }
                    }
                    else break;
                }
            }

            int classesSum = classes.Sum();
            
            int zeropoints = pointsClassifiedByHeight.Where(x => x == 0).Count();
            zeropoints = emptyPoints.Count;   
        }

        private void CalculateExpansionSpace3(System.Data.DataTable featureTable)
        {
            ExpansionClasses.Add(0);
            for(int i = 0; i <= heightClasses.Count; i++)
            {
                ExpansionClasses.Add(0);
            }
            List<System.Data.DataRow> leftEmptyPoints = featureTable.AsEnumerable()
                .Where(x => Convert.ToDouble(x["HEIGHT"]) == 0 & Convert.ToString(x["left"]) == "left")
                .ToList();
            List<System.Data.DataRow> rightEmptyPoints = featureTable.AsEnumerable()
            .Where(x => Convert.ToDouble(x["HEIGHT"]) == 0 & Convert.ToString(x["left"]) == "right")
            .ToList();
            var leftPoints = featureTable.AsEnumerable().Where(x => Convert.ToString(x["left"]) == "left")
                .Select(x => AssignHeightClass(Convert.ToDouble(x["HEIGHT"]))).ToList();


            CalculateExp(leftPoints, 
                featureTable.AsEnumerable().Where(x=>Convert.ToString(x["left"]) == "left").ToList(), 
                ExpansionClasses, leftEmptyPoints);
            var rightPoints = featureTable.AsEnumerable().Where(x => Convert.ToString(x["left"]) == "right").Select(x => AssignHeightClass(Convert.ToDouble(x["HEIGHT"]))).ToList();
            CalculateExp(rightPoints,                
                featureTable.AsEnumerable().Where(x=>Convert.ToString(x["left"]) == "right").ToList(),
                ExpansionClasses, rightEmptyPoints);

            leftEmptyPoints.AddRange(rightEmptyPoints);
            
            SkipsLinearFeet = leftEmptyPoints.Count / TotalPointCount * TotalLinearFeet;
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
            return heights.GroupBy(x => AssignHeightClass(x)).OrderBy(x => x.Key);
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
