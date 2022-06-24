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
            public Vector3D center
            {
                get; set;
            }
            public double radius { get; set; } = 1;
            public Color color
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
        /// <summary>
        /// 光线追踪
        /// </summary>
        /// <param name="origin">起点</param>
        /// <param name="dline">方向</param>
        /// <param name="min">射线最小值</param>
        /// <param name="max">射线最大值</param>
        Color tracrray(Vector3D origin, Vector3D dline, double min, double max)
        {
            double closet = double.PositiveInfinity;
            sharp claset_sharp = null;
            //遍历模型
            foreach (var item in sharplist)
            {
                //求解是否与圆相交
                var gp = IntersectRayShere(origin, dline, item);
                //小于判断
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
            //不存在返回空白值
            if (claset_sharp == null)
            {
                return Colors.White;
            }
            //
            return claset_sharp.color;
        }
        /// <summary>
        /// 求解一元二次
        /// </summary>
        /// <param name="origin">起点</param>
        /// <param name="dline">向量</param>
        /// <param name="sharp">球体</param>
        /// <returns></returns>
        Point IntersectRayShere(Vector3D origin, Vector3D dline, sharp sharp)
        {
            //半径
            double r = sharp.radius;
            //起点到圆心
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
            //偷懒
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
            //画布开始
            for (double x = 0 - cw / 2; x < cw / 2; x++)
            {
                for (double y = 0 - ch / 2; y < ch / 2; y++)
                {
                    //画布到视口
                    var D = canvastoviewport(new Point(x, y));
                    //获取像素颜色
                    var color = tracrray(new Vector3D(), D, 1, double.PositiveInfinity);
                    //坐标系转换
                    var p = MidPoint(new Point(x, y));
                    //定义颜色BGRA
                    byte[] colorData = { color.B, color.G, color.R, color.A };
                    //跨距
                    int stride = (WriteableBitmap.PixelWidth * WriteableBitmap.Format.BitsPerPixel) / 8;
                    //绘制
                    WriteableBitmap.WritePixels(new Int32Rect((int)p.X, (int)p.Y, 1, 1), colorData, stride, 0);
                }
            }
            WriteableBitmap.Unlock();

        }
    }
}
