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
        public Renderer(Image MainScreen) 
        {
            ImageSource = MainScreen;
        }

        public BitmapSource Render(Map map, GameObject[,] objects)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                // Draw background
                if (map.Background.ImageSource != null)
                {
                    Rect bgRect = new Rect(0, 0, ImageSource.Width, ImageSource.Height);
                    context.DrawImage(map.Background.ImageSource, bgRect);
                }

                // Draw each subimage
                foreach (var obj in objects)
                {
                    if (obj != null)
                    {
                        Rect targetRect = new Rect((map.TileSizeWidth*obj.Coordinates.X), (map.TileSizeHeight*obj.Coordinates.Y), obj.Sprite.Width, obj.Sprite.Height);
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
