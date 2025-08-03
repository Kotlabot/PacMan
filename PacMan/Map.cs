using Microsoft.Win32;
using PacMan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using Newtonsoft.Json;

namespace PacMan
{
    public class Map
    {
        public double Width { get; set; }
        public double Height { get; set; }    
        public double TileSizeWidth { get; set; }
        public double TileSizeHeight { get; set; }
  
        public Entity[,] map;
        public string PathToBackground { get; set; }
        [JsonIgnore]
        public ScreenElement Background { get; private set; }

        public Map(double width, double height, double tileSizeWidth, double tileSizeHeight) 
        {
            Width = width;
            Height = height;
            TileSizeWidth = tileSizeWidth;
            TileSizeHeight = tileSizeHeight;
            ChangeBackgoundSource("../../../images/background.png");
            LoadMap();  
        }

        private void LoadMap()
        {
            double offsetWidth = (Width % TileSizeWidth)/2;
            double offsetHeight = (Height % TileSizeHeight)/2;
            int numberOfTilesWidth = Convert.ToInt32(Width/TileSizeWidth);
            int numberOfTilesHeight = Convert.ToInt32(Height/TileSizeHeight);

            map = new Entity[numberOfTilesHeight, numberOfTilesWidth];
        }


        public void ChangeBackgoundSource(string userPath)
        {
            PathToBackground = userPath;
            Background = new ScreenElement(Width, Height, userPath);
            Background.LoadImage();
        }

        public void SaveUserMap()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            string jsonMap = JsonConvert.SerializeObject(this, settings);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Save as...";
            dialog.FileName = GetName();

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, jsonMap);
            }
        }

        public string GetName()
        {
            int counter = 1;
            string name = $"map{counter}.json";
            while (File.Exists(name))
            {
                name = $"map{++counter}.json";
            }
            return name;
        }

        public Map LoadUserMap()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Load map...";

            if (dialog.ShowDialog() == true)
            {
                string jsonMap = File.ReadAllText(dialog.FileName);
                var settings = new JsonSerializerSettings
                {
                    //NullValueHandling = NullValueHandling.Include,
                    //DefaultValueHandling = DefaultValueHandling.Include,
                    //ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };

                return JsonConvert.DeserializeObject<Map>(jsonMap, settings);
            }
            return null;
        }
    }
}
