using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadShapefilesAndClip
{
    class ShapefileLoader
    {
        string shp2pgsqlDir;
        int filenumber = 0;
        public ShapefileLoader(string shp2pgsqlDir)
        {
            this.shp2pgsqlDir = shp2pgsqlDir;
        }

        public string ConvertShapefileToSql(string table, string path)
        {
            filenumber++;
            return String.Format("\"{0}\" -a \"{1}\" {2} > {2}table{3}.sql", shp2pgsqlDir, path, table, filenumber);
        }
    }
}
