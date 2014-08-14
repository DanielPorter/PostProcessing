using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadShapefilesAndClip
{
    class ShapefileLoader
    {
        string PSQLDir;
        int filenumber = 0;
        public ShapefileLoader(string PSQLDir)
        {
            this.PSQLDir = PSQLDir;
        }

        public string AppendShapefile(string table, string path)
        {
            filenumber++;
            return String.Format("\"{0}\" -a \"{1}\" {2} > {2}table{3}.sql", PSQLDir, path, table, filenumber);
        }
    }
}
