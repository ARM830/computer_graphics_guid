using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 光栅化
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        int bitmapheight = 800;
        int bitmapwidth = 800;
        WriteableBitmap WriteableBitmap;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WriteableBitmap = new WriteableBitmap(bitmapwidth, bitmapheight, 96, 96, PixelFormats.Bgra32, BitmapPalettes.WebPaletteTransparent);
            img.Source = WriteableBitmap;
            Start();
        }

        private void Start()
        {
            WriteableBitmap.Lock();
            DrawLine(new Point(100, -200), new Point(60, 240), Colors.Red);
            WriteableBitmap.Unlock();
        }
        Point ConverterPoint(Point p) => new Point(p.X + bitmapwidth / 2, bitmapheight / 2 - p.Y);
        private void DrawLine(Point start, Point end, Color color)
        {
            if (start.X > end.X)
            {
                Swap(ref start, ref end);
            }
            var a = (end.Y - start.Y) / (end.X - start.X);
            var b = start.Y - a * start.X;
            byte[] colorData = { color.B, color.G, color.R, color.A };
            int stride = (WriteableBitmap.PixelWidth * WriteableBitmap.Format.BitsPerPixel) / 8;
            var y = start.Y;
            for (double x = start.X; x < end.X; x++)
            {
                var p = ConverterPoint(new Point(x, y));
                WriteableBitmap.WritePixels(new Int32Rect((int)p.X, (int)p.Y, 1, 1), colorData, stride, 0);
                y += a;
            }
        }

        private static void Swap(ref Point start, ref Point end)
        {
            (end, start) = (start, end);
        }
    }
}
