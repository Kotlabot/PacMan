using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PacMan
{
    public class ScreenElement
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public BitmapImage? ImageSource { get; private set; }
        public string PathToSource { get; private set; } = string.Empty;

        public ScreenElement(double x, double y, string path)
        {
            Width = x;
            Height = y;
            PathToSource = path;
        }

        public void LoadImage()
        {
            ImageSource = new BitmapImage();
            ImageSource.BeginInit();
            ImageSource.UriSource = new Uri(PathToSource, UriKind.Relative);
            ImageSource.EndInit();
        }
    }
}
