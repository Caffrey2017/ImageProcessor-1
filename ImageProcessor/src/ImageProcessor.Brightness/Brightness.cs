using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Brightness
{
    class BrightnessEffect
    {
        private Bitmap bitmap;
        private Bitmap bitmapOld;
        private int imgWidth;
        private int imgHeight;

        public double multiplier { get; private set; }

        public BrightnessEffect(ref Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.bitmapOld = new Bitmap(bitmap);
            this.imgHeight = bitmap.Height;
            this.imgWidth = bitmap.Width;
        }

        public void ApplyEffect(int brightness)
        {
            //if (brightness != 0)
            //    multiplier = 1 + (double)brightness / 100;
            //else
            //    return;

            //Console.WriteLine(multiplier);


            for (int i = 0; i < imgHeight; i++)
            {
                for (int j = 0; j < imgWidth; j++)
                {
                    Color col = bitmapOld.GetPixel(j, i);
                    bitmap.SetPixel(j, i, Color.FromArgb(
                        Math.Min(255, col.R + brightness),
                        Math.Min(255, col.G + brightness),
                        Math.Min(255, col.B + brightness)));
                }
            }
        }
    }
}
