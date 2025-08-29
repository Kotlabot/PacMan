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
        private Color Color { get; set; } = Color.FromRgb(128, 128, 128);
        public Renderer(Image MainScreen) 
        {
            ImageSource = MainScreen;
        }

        public BitmapSource Render(Map map, GameObject[,] objects)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext context = drawingVisual.RenderOpen())
            {
                // Vykresleni pozadi
                if (map.Background.ImageSource != null)
                {
                    Rect bgRect = new Rect(0, 0, ImageSource.Width, ImageSource.Height);
                    context.DrawImage(map.Background.ImageSource, bgRect);
                }

                // Vykresleni vsech game objektu
                foreach (var obj in objects)
                {
                    if (obj != null)
                    {
                        Rect targetRect = new Rect((map.TileSizeWidth*obj.Coordinates.X), (map.TileSizeHeight*obj.Coordinates.Y), obj.Sprite.Width, obj.Sprite.Height);
                        context.DrawImage(obj.Sprite.ImageSource, targetRect);
                    }
                }

                if (GameManager.instance.isSuperMode)
                {
                    Rect overlayRect = new Rect(0, 0, ImageSource.Width, ImageSource.Height);
                    SolidColorBrush overlayBrush = new SolidColorBrush(Color)
                    {
                        Opacity = 0.3
                    };

                    context.DrawRectangle(overlayBrush, null, overlayRect);
                }
            }

            RenderTargetBitmap finalImage = new RenderTargetBitmap(Convert.ToInt32(ImageSource.Width), Convert.ToInt32(ImageSource.Height), 96, 96, PixelFormats.Pbgra32);

            finalImage.Render(drawingVisual);
            return finalImage;
        }
    }
}
