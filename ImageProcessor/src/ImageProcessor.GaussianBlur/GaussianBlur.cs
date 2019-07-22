using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
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

        private const int id = 1;


        // Kernel stuff
        public double[,] kernel { get; private set; }
        public uint kernelSize { get; private set; }
        public float sigma { get; private set; }

        public GaussianBlurEffect(ref Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.bitmapOld = new Bitmap(bitmap);
            this.imgWidth = bitmap.Width;
            this.imgHeight = bitmap.Height;

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

        public void ApplyEffect(int lastUsedEffect)
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

        public void ApplyEffectLockBits(int lastUsedEffect)
        {
            if (lastUsedEffect != id && lastUsedEffect != 0)
                bitmapOld = new Bitmap(bitmap);

            // Locking bitmap
            Rectangle rect = new Rectangle(0, 0, imgWidth, imgHeight);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            BitmapData bmpDataOld = bitmapOld.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // Getting pointer to beginning of bitmap
            IntPtr ptr = bmpData.Scan0;
            IntPtr ptrOld = bmpDataOld.Scan0;

            // Declating an array of bytes of bitmap
            int bytes = Math.Abs(bmpData.Stride) * imgHeight;
            byte[] rgbValues = new byte[bytes];
            byte[] rgbValuesOld = new byte[bytes];

            // Copying data to array
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            Marshal.Copy(ptrOld, rgbValuesOld, 0, bytes);

            // Calculating pixels values
            for (int i = 3 * ((int)kernelSize - 1) * imgWidth; i < rgbValues.Length - 3*(kernelSize - 1) * imgWidth; i += 3)
            {
                if (((i/3)%imgWidth < ((int)kernelSize - 1)) || ((i/3)%imgWidth >= imgWidth - ((int)kernelSize - 1)) )
                    continue;

                else
                {
                    rgbValues[i]   = CalculatePixelLockBits(i, 0, rgbValuesOld);
                    rgbValues[i+1] = CalculatePixelLockBits(i, 1, rgbValuesOld);
                    rgbValues[i+2] = CalculatePixelLockBits(i, 2, rgbValuesOld);
                }
            }

            // Copy RGB values back to bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            bitmap.UnlockBits(bmpData);
            bitmapOld.UnlockBits(bmpDataOld);

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

        private byte CalculatePixelLockBits(int pos, int color, byte[] rgbValues)
        {
            double sum = 0;
            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    switch (color)
                    {
                        case 0:
                            sum += rgbValues[(((pos/3)/imgWidth - (kernelSize - 1) + i)* imgWidth * 3 + ((pos / 3) % imgWidth - (kernelSize - 1) + j) * 3)] * kernel[i, j];
                            break;
                        case 1:
                            sum += rgbValues[(((pos / 3) / imgWidth - (kernelSize - 1) + i) * imgWidth * 3 + ((pos / 3) % imgWidth - (kernelSize - 1) + j) * 3) + 1] * kernel[i, j];
                            break;
                        case 2:
                            sum += rgbValues[(((pos / 3) / imgWidth - (kernelSize - 1) + i) * imgWidth * 3 + ((pos / 3) % imgWidth - (kernelSize - 1) + j) * 3) + 2] * kernel[i, j];
                            break;
                    }
                }
            }
            return (byte)sum;
        }

    }
}
