using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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

        private const int id = 2;

        public double multiplier { get; private set; }

        public BrightnessEffect(ref Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.bitmapOld = new Bitmap(bitmap);
            this.imgHeight = bitmap.Height;
            this.imgWidth = bitmap.Width;
        }

        /// <summary>
        /// METHOD IS DEPRECATED due to low performance, use ApplyEffectLockBits instead 
        /// </summary>
        public void ApplyEffect(int brightness, int lastUsedEffect)
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

        /// <summary>
        /// Applies BrigtnessEffect to image.
        /// </summary>
        /// <param name="brightness">Specify how much brigther or darker output image should be.</param>
        public void ApplyEffectLockBits(int brightness, int lastUsedEffect)
        {
            Console.WriteLine(id);
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
            for (int i = 0; i < rgbValues.Length; i += 3)
            {
                int colR = rgbValuesOld[i] + brightness;
                int colG = rgbValuesOld[i+1] + brightness;
                int colB = rgbValuesOld[i+2] + brightness;

                if (colR < 0) colR = 0;
                if (colG < 0) colG = 0;
                if (colB < 0) colB = 0;

                rgbValues[i] = (byte)Math.Min(255, colR);
                rgbValues[i+1] = (byte)Math.Min(255, colG);
                rgbValues[i+2] = (byte)Math.Min(255, colB);
            }

            // Copy RGB values back to bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            bitmap.UnlockBits(bmpData);
            bitmapOld.UnlockBits(bmpDataOld);
        }

    }
}
