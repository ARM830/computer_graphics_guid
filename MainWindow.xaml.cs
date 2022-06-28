using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        class cop
        {
            public cop(sharp claset_sharp, double closet)
            {
                Claset_sharp = claset_sharp;
                Closet = closet;
            }
            public sharp Claset_sharp
            {
                get;
            }
            public double Closet
            {
                get;
            }
        }
        enum LightEnum
        {
            Ambient,
            Point,
            Directional
        }
        class Light
        {
            public Vector3D Direction
            {
                get; set;
            }
            public Vector3D Position
            {
                get; set;
            }
            public LightEnum LightType
            {
                get; set;
            }
            public double Intenesity
            {
                get; set;
            }
        }
        class sharp
        {
            public Vector3D center
            {
                get; set;
            }
            public double radius { get; set; } = 1;
            public Color color
            {
                get; set;
            }
            public double specular
            {
                get; set;
            }
            public double CloseIntersection
            {
                get; set;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cw = 800.0 / 2;

            ch = 800.0 / 2;

            WriteableBitmap = new WriteableBitmap(800, 800, 96, 96, PixelFormats.Bgra32, BitmapPalettes.WebPaletteTransparent);
            img.Source = WriteableBitmap;
            Start();
        }

        WriteableBitmap WriteableBitmap;
        List<sharp> sharplist = new List<sharp>
        {
        new sharp() { center = new Vector3D(0, -1, 3), color = Colors.Red,specular=500 },
        new sharp() { center = new Vector3D(2, 0, 4),  color = Colors.Blue,specular=500 },
        new sharp() { center = new Vector3D(-2, 0, 4), color = Colors.Green ,specular=10},
        new sharp(){center=new Vector3D(0,-5001,0), radius=5000,color=Color.FromRgb(255,255,0) ,specular=1000},
        };
        List<Light> lightlist = new List<Light> {

        new Light(){  LightType= LightEnum.Ambient,Intenesity=0.2 },
        new Light(){  LightType= LightEnum.Point,Intenesity=0.6,Position=new Vector3D(2,1,0) },
        new Light(){  LightType= LightEnum.Directional,Intenesity=0.2,Direction=new Vector3D(1,4,4) }
        };
        Point MidPoint(Point p) => new Point(p.X + 800.0 / 2, 800.0 / 2 - p.Y);
        Vector3D canvastoviewport(Point p)
        {

            return new Vector3D(p.X * (vw / cw), p.Y * (vh / ch), d);
        }
        List<double> ls = new List<double>();
        cop closestIntersection(Vector3D origin, Vector3D dline, double min, double max)
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
                    claset_sharp.CloseIntersection = closet;
                }
                if (gp.Y >= min && gp.Y <= max && gp.Y < closet)
                {
                    closet = gp.Y;
                    claset_sharp = item;
                    claset_sharp.CloseIntersection = closet;
                }
            }
            if (claset_sharp != null)
            {
                return new cop(claset_sharp, closet);
            }
            return null;
        }
        double ComputeLighting(Vector3D p, Vector3D n, Vector3D v, double s)
        {
            double i = 0;
            Vector3D l = new Vector3D();
            double tmax = 1;
            foreach (var light in lightlist)
            {
                if (light.LightType == LightEnum.Ambient)
                {
                    i += light.Intenesity;
                }
                else
                {
                    if (light.LightType == LightEnum.Point)
                    {
                        l = light.Position - p;
                        tmax =l.Length;//tmax=1;
                    }
                    else
                    {
                        l = light.Direction;
                        tmax =(light.Position-p).Length;//tmax=intf
                    }
                    var sharpx = closestIntersection(p, l, 0.0000000001, tmax);
                    //此时存在，则意味着光源和点之间存在物体
                    if (sharpx != null)
                    {

                        //不计算此刻光源
                        //此点一定会返回环境光
                        continue;
                    }
                    var dot = Vector3D.DotProduct(n, l);
                    if (dot > 0)
                    {
                        i += light.Intenesity * dot / (n.Length * l.Length);
                    }
                    if (s != -1)
                    {
                        var r = 2 * Vector3D.DotProduct(n, l) * n - l;
                        var dotv = Vector3D.DotProduct(r, v);
                        if (dotv > 0)
                        {
                            i += light.Intenesity * Math.Pow(dotv / (r.Length * v.Length), s);
                        }
                    }
                }

            }
            //可以设置大1为1
            //i>1?1:i
            return i;
        }
        Color tracrray(Vector3D origin, Vector3D dline, double min, double max)
        {
            double closet = double.PositiveInfinity;
            var clp = closestIntersection(origin, dline, min, max);

            if (clp == null)
            {
                return Colors.Transparent;
            }
            closet = clp.Closet;
            var claset_sharp = clp.Claset_sharp;
            var p = origin + (closet * dline);
            var n = p - claset_sharp.center;
            n = n / n.Length;
            var cl = ComputeLighting(p, n, -dline, claset_sharp.specular);
            //保证颜色上下限正确
            var M = Color.FromRgb((byte)(Math.Min(255, Math.Max(0, cl * claset_sharp.color.R))),
                (byte)(Math.Min(255, Math.Max(0, (cl * claset_sharp.color.G)))),
                (byte)(Math.Min(255, Math.Max(0, (cl * claset_sharp.color.B)))));

            return M;
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
            var t1 = (-b + Math.Sqrt(discrinimant)) / (2 * a);
            var t2 = (-b - Math.Sqrt(discrinimant)) / (2 * a);
            return new Point(t1, t2);
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
                    var color = tracrray(new Vector3D(), D, 1, double.MaxValue);
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
