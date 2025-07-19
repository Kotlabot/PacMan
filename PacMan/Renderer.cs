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
    public class Renderer
    {
        Image ImageSource { get; set; }
        ScreenElement Background {  get; set; }
        public Renderer(Image MainScreen, string pathToBackground) 
        {
            ImageSource = MainScreen;
            Background = new ScreenElement(ImageSource.Width, ImageSource.Height, pathToBackground);
            Background.LoadImage();
        }
        public BitmapSource Render(List<GameObject> objects)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                // Draw background
                if (Background.ImageSource != null)
                {
                    Rect bgRect = new Rect(0, 0, ImageSource.Width, ImageSource.Height);
                    context.DrawImage(Background.ImageSource, bgRect);
                }

                // Draw each subimage
                foreach (var obj in objects)
                {
                    if(obj != null)
                    {
                        Rect targetRect = new Rect(obj.Coordinates.X, obj.Coordinates.Y, obj.Sprite.Width, obj.Sprite.Height);
                        context.DrawImage(obj.Sprite.ImageSource, targetRect);
                    }
                }
            }

            RenderTargetBitmap finalImage = new RenderTargetBitmap(Convert.ToInt32(ImageSource.Width), Convert.ToInt32(ImageSource.Height), 96, 96, PixelFormats.Pbgra32);

            finalImage.Render(drawingVisual);
            return finalImage;
        }

    }
}
