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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 光线追综
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        class sharp
        {
            public Vector3D center { get; set; }
            public double radius { get; set; } = 1;
            public Color color { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cw = 600.0 / 2;
                
            ch = 600.0 / 2;

            WriteableBitmap = new WriteableBitmap(800, 800, 96, 96, PixelFormats.Bgr32, BitmapPalettes.WebPaletteTransparent);
            img.Source = WriteableBitmap;
            Start();
        }

        WriteableBitmap WriteableBitmap;
        List<sharp> sharplist = new List<sharp>
        {
        new sharp() { center = new Vector3D(0, -1, 3), color = Colors.Red },
        new sharp() { center = new Vector3D(2, 0, 4),  color = Colors.Blue },
        new sharp() { center = new Vector3D(-2, 0, 4), color = Colors.Green }
        };
        Point MidPoint(Point p) => new Point(p.X + 800.0 / 2, 800.0 / 2 - p.Y);
        Vector3D canvastoviewport(Point p)
        {

            return new Vector3D(p.X * (vw / cw), p.Y * (vh / ch), d);
        }
        Color tracrray(Vector3D origin, Vector3D dline, double min, double max)
        {
            double closet = double.PositiveInfinity;
            sharp claset_sharp = null;
            foreach (var item in sharplist)
            {
                var gp = IntersectRayShere(origin, dline, item);
                if (gp.X >= min && gp.X <= max && gp.X < closet)
                {
                    closet = gp.X;
                    claset_sharp = item;
                }
                if (gp.Y >= min && gp.Y <= max && gp.Y < closet)
                {
                    closet = gp.Y;
                    claset_sharp = item;
                }
            }
            if (claset_sharp == null)
            {
                return Colors.White;
            }
            return claset_sharp.color;
        }
        Point IntersectRayShere(Vector3D origin, Vector3D dline, sharp sharp)
        {
            double r = sharp.radius;
            var co = origin - sharp.center;
            var a = Vector3D.DotProduct(dline, dline);
            var b = 2 * Vector3D.DotProduct(co, dline);
            var c = Vector3D.DotProduct(co, co) - r * r;
            var discrinimant = b * b - 4 * a * c;
            if (discrinimant < 0)
            {
                return new Point();
            }
            return new Point((-b + discrinimant * discrinimant) / (2 * a), (-b - discrinimant * discrinimant) / (2 * a));
        }
        double cw = 0;
        double ch = 0;
        double vw = 1.0;
        double vh = 1.0;
        double d = 1.0;
        void Start()
        {
            WriteableBitmap.Lock();
            for (double x = 0 - cw / 2; x < cw / 2; x++)
            {
                for (double y = 0 - ch / 2; y < ch / 2; y++)
                {
                    var D = canvastoviewport(new Point(x, y));
                    var color = tracrray(new Vector3D(), D, 1, double.PositiveInfinity);
                    var p = MidPoint(new Point(x, y));
                    byte[] colorData = { color.B, color.G, color.R, color.A };
                    int stride = (WriteableBitmap.PixelWidth * WriteableBitmap.Format.BitsPerPixel) / 8;
                    WriteableBitmap.WritePixels(new Int32Rect((int)p.X, (int)p.Y, 1, 1), colorData, stride, 0);
                }
            }
            WriteableBitmap.Unlock();

        }
    }
}
