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
    /// <summary>
    /// Represents a visual screen element for game objects. Can load a single image or multiple images (animation frames).
    /// </summary>
    public class ScreenElement
    {
        // Width and height of an image (in pixels).
        public double Width { get; private set; }
        public double Height { get; private set; }

        // Backing field for ImageSource.
        private BitmapImage? imageSource;

        // The current image to render (one image or one frame of an animation).
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

        // Path to the image or animation directory.
        public string PathToSource { get; set; } = string.Empty;

        // Holds frames for an animation if the source is a folder.
        public List<BitmapImage> animation = new List<BitmapImage>();

        // Counter to keep track of which animation frame is currently active.
        private int frameCounter;

        public ScreenElement(double x, double y, string path)
        {
            Width = x;
            Height = y;
            PathToSource = path;
        }

        /// <summary>
        /// Loads the image or animation frames from PathToSource.
        /// </summary>
        public bool LoadImage()
        {
            // If the path points to a single image file, create a BitmapImage and set the path from which to load the image.
            if (File.Exists(PathToSource))
            {
                imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(PathToSource, UriKind.Relative);
                imageSource.EndInit();

            }
            // If the path points to a directory containing multiple image files,
            // load each image file as a BitmapImage and add it to animation frames.
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

        /// <summary>
        /// Updates the currently displayed frame if this is an animated element.
        /// Increases the frame counter, loops back to the start when reaching the end.
        /// </summary>
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
