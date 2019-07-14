using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;

namespace ImageProcessor.ImageUtil
{
    class IPImage
    {
        private Bitmap bitmap;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public IPImage(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }
        public BitmapSource BitmapToImageSource()
        {
            BitmapSource imgSource;

            var hBitmap = bitmap.GetHbitmap();
            try
            {
                imgSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            catch(Exception)
            {
                imgSource = null;
            }

            finally
            {
                DeleteObject(hBitmap);
            }

            return imgSource;
        }
    }
}
