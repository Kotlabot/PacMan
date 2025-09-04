using PacMan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PacMan
{
    /// <summary>
    /// Responsible for rendering the game world (background + objects) onto the main screen using WPF drawing.
    /// </summary>
    public class Renderer
    {
        // Reference to the main Image control in the UI, which displays the rendered output.
        Image ImageSource { get; set; }

        // Overlay color used when Pacman is in super mode (drawn with transparency).
        private Color Color { get; set; } = Color.FromRgb(128, 128, 128);
        public Renderer(Image MainScreen) 
        {
            ImageSource = MainScreen; // Stores a reference to the Image control that will display the rendered frames onto main screem.
        }

        /// <summary>
        /// Renders the background and all game objects to a BitmapSource. It's called every frame and returns the complete image to display.
        /// </summary>
        /// <param name="map">Provides the background and tile sizes.</param>
        /// <param name="objects">Provides all active game objects.</param>
        public BitmapSource Render(Map map, GameObject[,] objects)
        {
            // DrawingVisual is used for rendering vector graphics.
            DrawingVisual drawingVisual = new DrawingVisual();

            // DrawingContext is used to issue drawing commands.
            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                // Draw background by defining a rectangle that covers the entire Image control
                // and drawing the background image into that rectangle.
                if (map.Background.ImageSource != null)
                {
                    Rect bgRect = new Rect(0, 0, ImageSource.Width, ImageSource.Height);
                    context.DrawImage(map.Background.ImageSource, bgRect);
                }

                // Draw all game objects by defining the rectangle on screen where this object should be drawn
                // (based on its coordinates multiplied by the map tile size) and drawing it into that rectangle.
                foreach (var obj in objects)
                {
                    if (obj != null)
                    {
                        Rect targetRect = new Rect((map.TileSizeWidth*obj.Coordinates.X), (map.TileSizeHeight*obj.Coordinates.Y), obj.Sprite.Width, obj.Sprite.Height);
                        context.DrawImage(obj.Sprite.ImageSource, targetRect);
                    }
                }

                // Draw overlay when Pacman is in super mode (after eating super cookie)
                if (GameManager.instance.isSuperMode)
                {
                    // Cover the whole screen with a semi-transparent rectangle.
                    Rect overlayRect = new Rect(0, 0, ImageSource.Width, ImageSource.Height);
                    SolidColorBrush overlayBrush = new SolidColorBrush(Color)
                    {
                        Opacity = 0.3
                    };

                    context.DrawRectangle(overlayBrush, null, overlayRect);
                }
            }

            // Creates a bitmap (raster image) of the DrawingVisual. DPI resolution (96) was set as an standart value for WPF.
            RenderTargetBitmap finalImage = new RenderTargetBitmap(Convert.ToInt32(ImageSource.Width), Convert.ToInt32(ImageSource.Height), 96, 96, PixelFormats.Pbgra32);

            finalImage.Render(drawingVisual);
            return finalImage;
        }
    }
}
