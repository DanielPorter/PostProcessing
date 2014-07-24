using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PostProcessing
{
    class PostProcessingSettings
    {
        public List<double> HeightCats;
        public List<double> DensityCats;
        public List<double> IRCats;

        public void SaveSettings()
        {
            Stream TestFileStream = File.Create("PostProcessingSettings.bin");
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(TestFileStream, this);
            TestFileStream.Close();
        }

        public void LoadSettings()
        {
            try
            {
                Stream fs = File.Open("PostProcessingSettings.bin", FileMode.Open);
                BinaryFormatter serializer = new BinaryFormatter();
                PostProcessingSettings tempSettings = (PostProcessingSettings)serializer.Deserialize(fs);

                HeightCats = tempSettings.HeightCats;
                DensityCats = tempSettings.DensityCats;
                IRCats = tempSettings.IRCats;
            }
            catch
            {
                LoadDefaultSettings();
                return;
            }


        }

        private void LoadDefaultSettings()
        {
            HeightCats.Add(5);
            HeightCats.Add(7);
            HeightCats.Add(9);
            HeightCats.Add(11);
            HeightCats.Add(13);

            DensityCats.Add(.2);
            DensityCats.Add(.4);
            DensityCats.Add(.6);
            DensityCats.Add(.8);
            DensityCats.Add(1);

            IRCats.Add(1);
            IRCats.Add(2);
            IRCats.Add(3);
            IRCats.Add(4);
            IRCats.Add(5);
        }
    }
}
