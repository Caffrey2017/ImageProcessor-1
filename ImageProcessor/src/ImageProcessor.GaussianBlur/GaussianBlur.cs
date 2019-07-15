using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.GaussianBlur
{
    public class GaussianBlurEffect
    {
        private Bitmap bitmap;
        private Bitmap bitmapOld;
        private int imgWidth;
        private int imgHeight;

        // Kernel stuff
        public double[,] kernel { get; private set; }
        public uint kernelSize { get; private set; }
        public float sigma { get; private set; }

        public GaussianBlurEffect(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            bitmapOld = new Bitmap(bitmap);
            imgWidth = bitmap.Width;
            imgHeight = bitmap.Height;

            this.kernelSize = 3;
            this.sigma = 5.5f;
        }

        public void CalculateKernel(uint kernelSize, float sigma)
        {
            this.kernelSize = kernelSize;
            this.kernel = new double[kernelSize, kernelSize];
            this.sigma = sigma;

            uint x = 0 - ((kernelSize - 1) / 2);
            uint y = 0 - ((kernelSize - 1) / 2);
            double sum = 0;

            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    kernel[i, j] = 1 / (2 * Math.PI * sigma * sigma) * Math.Pow(Math.E, -(((x * x) + (y * y)) / (2 * sigma * sigma)));
                    sum += kernel[i, j];
                    x += 1;
                }
                x = 0 - ((kernelSize - 1) / 2);
                y += 1;
            }

            if (sum != 1)
            {
                double tmp = 1 / sum;
                for (int i = 0; i < kernelSize; i++)
                {
                    for (int j = 0; j < kernelSize; j++)
                        kernel[i, j] *= tmp;
                }
            }
        }

        public void ApplyEffect()
        {
            for (int i = (int)kernelSize - 1; i < imgHeight - (kernelSize - 1); i++)
            {
                for (int j = (int)kernelSize - 1; j < imgWidth - (kernelSize - 1); j++)
                {
                    // img.setPixelAt(j, i, Colour(calcPixel(j, i, 0), calcPixel(j, i, 1), calcPixel(j, i, 2))); ||| Old, library specific code
                    bitmap.SetPixel(j, i, Color.FromArgb(CalculatePixel(j, i, 0), CalculatePixel(j, i, 1), CalculatePixel(j, i, 2)));
                }
            }
        }

        private int CalculatePixel(int x, int y, int color)
        {
            double sum = 0;
            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    switch (color)
                    {
                        case 0:
                            sum += bitmapOld.GetPixel((int)(x - (kernelSize - 1) + j), (int)(y - (kernelSize - 1) + i)).R * kernel[i, j];
                            break;
                        case 1:
                            sum += bitmapOld.GetPixel((int)(x - (kernelSize - 1) + j), (int)(y - (kernelSize - 1) + i)).G * kernel[i, j];
                            break;
                        case 2:
                            sum += bitmapOld.GetPixel((int)(x - (kernelSize - 1) + j), (int)(y - (kernelSize - 1) + i)).B * kernel[i, j];
                            break;
                    }
                }
            }
            return (int)sum;
        }

    }
}
