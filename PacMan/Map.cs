using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PacMan
{
    public class Map
    {
        public double Width { get; private set; }
        public double Height { get; private set; }    
        public double TileSizeWidth { get; private set; }
        public double TileSizeHeight { get; private set; }
        public List<GameObject> Objects { get; private set; }
        public string[,] map;

        public Map(double width, double height, double tileSizeWidth, double tileSizeHeight) 
        {
            Width = width;
            Height = height;
            TileSizeWidth = tileSizeWidth;
            TileSizeHeight = tileSizeHeight;
        }

        private void LoadMap()
        {
            double offsetWidth = (Width % TileSizeWidth)/2;
            double offsetHeight = (Height % TileSizeHeight)/2;
            int numberOfTilesWidth = Convert.ToInt32(Width/TileSizeWidth);
            int numberOfTilesHeight = Convert.ToInt32(Height/TileSizeHeight);

            map = new string[numberOfTilesWidth, numberOfTilesHeight];
            Objects = new List<GameObject>();
        }

    }
}
