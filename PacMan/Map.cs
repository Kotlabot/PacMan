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
    /// <summary>
    /// Represents a static map definition for the game.
    /// Holds dimensions, tile sizes, background image, and an entity grid.
    /// This class is used to load, save, and manage maps.
    /// </summary>
    public class Map
    {
        // Total width and height of the map in pixels.
        public double Width { get; set; }
        public double Height { get; set; }

        // Width and height of one tile (cell) in pixels.
        public double TileSizeWidth { get; set; }
        public double TileSizeHeight { get; set; }

        // Path to the background image used for rendering the map.
        public string PathToBackground { get; set; }

        // Two-dimensional grid of entities (static objects) placed on the map.
        public Entity[,] map;

        // Path to the saved map file we use when we want to load next JSON levels (from the same folder).
        [JsonIgnore]
        public string PathToMap { get; set; }

        // Screen element that represents the map's background image.
        [JsonIgnore]
        public ScreenElement Background { get; private set; }

        // Represent number of tiles in one horizontal line of grid (height of grid/size of one tile)
        [JsonIgnore]
        public double HeightTileCount { get; private set; }

        // Represents number of tiles in one vertical line of grid (width of grid/size of one tile)
        [JsonIgnore]
        public double WidthTileCount { get; private set; }


        public Map(double width, double height, double tileSizeWidth, double tileSizeHeight)
        {
            Width = width;
            Height = height;
            TileSizeWidth = tileSizeWidth;
            TileSizeHeight = tileSizeHeight;
            
            // Load default background.
            ChangeBackgoundSource("../../../images/background.png");

            // Initialize the entity grid (map).
            LoadMap();
        }

        /// <summary>
        /// Initializes the entity grid based on map and tile dimensions.
        /// </summary>
        private void LoadMap()
        {
            WidthTileCount = Convert.ToInt32(Width / TileSizeWidth);
            HeightTileCount = Convert.ToInt32(Height / TileSizeHeight);

            map = new Entity[(int)HeightTileCount, (int)WidthTileCount];
        }

        /// <summary>
        /// Changes the background image of the map to a user-specified file, if the file is valid.
        /// </summary>
        public void ChangeBackgoundSource(string userPath)
        {
            PathToBackground = userPath;
            var background = new ScreenElement(Width, Height, userPath);
            if (background.LoadImage())
            {
                Background = background;
            }
        }

        /// <summary>
        /// Saves the map created in map editor mode to a JSON file chosen by the user.
        /// Includes type information for deserializing custom entities.
        /// </summary>
        public void SaveUserMap()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Error,
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

        /// <summary>
        /// Generates a unique name for saving maps (map1.json, map2.json, ...).
        /// </summary>
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

        /// <summary>
        /// Loads a map from a file chosen by the user.
        /// </summary>
        public Map LoadUserMap()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.Title = "Load map...";

            if (dialog.ShowDialog() == true)
            {
                PathToMap = dialog.FileName;
                string jsonMap = File.ReadAllText(PathToMap);
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };
                try
                {
                    return JsonConvert.DeserializeObject<Map>(jsonMap, settings);
                }
                catch
                {
                    PathToMap = "";
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a map from a given file path (automatically loads the next level after one is completed).
        /// </summary>
        public Map LoadUserMap(string path)
        {
            string jsonMap = File.ReadAllText(path);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            try
            {
                return JsonConvert.DeserializeObject<Map>(jsonMap, settings);
            }
            catch { }
            return null;
        }

    }
}
