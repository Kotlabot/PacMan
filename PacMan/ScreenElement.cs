using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PacMan
{
    public class ScreenElement
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public BitmapImage? ImageSource
        {
            get
            {
                var actualSource = imageSource;
                return actualSource;
            }
            set
            {
                imageSource = value;
            }
        }
        public string PathToSource { get; set; } = string.Empty; // private setter!!!
        public List<BitmapImage> animation = new List<BitmapImage>();

        private BitmapImage? imageSource;
        private int frameCounter;

        public ScreenElement(double x, double y, string path)
        {
            Width = x;
            Height = y;
            PathToSource = path;
        }

        public bool LoadImage()
        {
            if (File.Exists(PathToSource))
            {
                imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(PathToSource, UriKind.Relative);
                imageSource.EndInit();

            }
            else if (Directory.Exists(PathToSource))
            {
                animation = Directory.GetFiles(PathToSource, "*.*")
                      .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                     file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                      .Select(file =>
                      {
                          BitmapImage img = new BitmapImage();
                          img.BeginInit();
                          img.UriSource = new Uri(file, UriKind.Relative);
                          img.EndInit();
                          return img;
                      })
                      .ToList();
            }
            else
            {
                MessageBox.Show("Path does not exist.");
                return false;
            }
            return true;
        }

        public void UpdateAnimation()
        {
            if (animation.Count > 0)
            {
                if (frameCounter >= animation.Count)
                    frameCounter = 0;
                imageSource = animation[frameCounter];
                frameCounter++;
            }
        }
    }
}
