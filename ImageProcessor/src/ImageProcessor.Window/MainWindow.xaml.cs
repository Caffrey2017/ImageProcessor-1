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

using ImageProcessor.ImageUtil;
using ImageProcessor.GaussianBlur;

namespace ImageProcessor
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap img;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                Console.WriteLine("File loaded.");
                ImageLabel.Height = 0;
                saveMenuBtn.IsEnabled = true;

                img = new Bitmap(dialog.FileName);

                // Setting display image
                CachedImage.Source = new IPImage(img).BitmapToImageSource();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            GaussianBlurEffect gauss = new GaussianBlurEffect(img);
            gauss.CalculateKernel(3, 5.5f);
            gauss.ApplyEffect();

            CachedImage.Source = new IPImage(img).BitmapToImageSource();
        }
    }
}
