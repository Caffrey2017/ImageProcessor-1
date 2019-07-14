using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private void LoadFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine("File loaded.");
                ImageLabel.Height = 0;

                Bitmap img = new Bitmap(dialog.FileName);

                // Converting bitmap to BitmapSource object
                BitmapSource imgSource;
                var hBitmap = img.GetHbitmap();
                imgSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(hBitmap);

                //BitmapImage bitmapImg = new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute));
                //BitmapSource src = new FormatConvertedBitmap(bitmapImg, PixelFormats.Rgb24, null, 0);
                //WriteableBitmap writeableBitmap = new WriteableBitmap(src);

                //int[] pixelData = new int[writeableBitmap.PixelWidth * writeableBitmap.PixelHeight];
                //writeableBitmap.CopyPixels(pixelData, 3 * writeableBitmap.PixelWidth, 0);
                //Console.WriteLine(pixelData[0].ToString() + " " + pixelData[1].ToString() + " " + pixelData[2].ToString() + " " + pixelData[3].ToString());

                // Setting display image
                // CachedImage.Source = writeableBitmap;
                CachedImage.Source = imgSource;
            }
        }
    }
}
