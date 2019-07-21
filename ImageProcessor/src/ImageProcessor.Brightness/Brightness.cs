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
                    int colR = col.R + brightness;
                    int colG = col.G + brightness;
                    int colB = col.B + brightness;

                    if (colR < 0) colR = 0;
                    if (colG < 0) colG = 0;
                    if (colB < 0) colB = 0;
                    bitmap.SetPixel(j, i, Color.FromArgb(
                        Math.Min(255, colR),
                        Math.Min(255, colG),
                        Math.Min(255, colB)));
                }
            }
        }
    }
}
