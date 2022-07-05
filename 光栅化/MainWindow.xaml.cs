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
        WriteableBitmap wb;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            wb = new WriteableBitmap(bitmapwidth, bitmapheight, 96, 96, PixelFormats.Bgra32, BitmapPalettes.WebPaletteTransparent);
            img.Source = wb;
            Start();
        }

        private void Start()
        {
            // WriteableBitmap.DrawLineNormal(new Point(100, -200), new Point(60, 240), Colors.Red);
            //  WriteableBitmap.DrawLineNormal(new Point(200, -300), new Point(160, 340), Colors.Red);
            // wb.DrawLine(new Point(0, -300), new Point(0, 300), Colors.Black);
            // wb.DrawLine(new Point(-300, 0), new Point(300, 0), Colors.Black);
            // wb.DrawTriangle(new Point(-200, 130), new Point(200, 130), new Point(200, 250), Colors.Black);
            // wb.DrawFillTriangle(new Point(-200, 0), new Point(200, 0), new Point(200, 120), Colors.Red);
            // wb.DrawShadedTriagngle(new Point(-100, 0), new Point(100, 0), new Point(100, 120), Colors.Red);
            // wb.DrawShadedTriagngle(new Point(-200, 100), new Point(100, 100), new Point(100, 240), Colors.Green);
            // WriteableBitmap.DrawShadedTriagngle(new Point(-200, 0), new Point(200, 0), new Point(200, 120), Colors.Red);
          // var vaf = new Vector3D(-1, 1, 1);
          // var vbf = new Vector3D(1, 1, 1);
          // var vcf = new Vector3D(1, -1, 1);
          // var vdf = new Vector3D(-1, -1, 1);
          //
          // var vab = new Vector3D(-1, 1, 2);
          // var vbb = new Vector3D(1, 1, 2);
          // var vcb = new Vector3D(1, -1, 2);
          // var vdb = new Vector3D(-1, -1, 2);

            var vaf = new Vector3D(-2, -0.5, 5);
            var vbf = new Vector3D(-2, 0.5, 5);
            var vcf = new Vector3D(-1, 0.5, 5);
            var vdf = new Vector3D(-1, -0.5, 5);

            var vab = new Vector3D(-2, -0.5, 6);
            var vbb = new Vector3D(-2, 0.5, 6);
            var vcb = new Vector3D(-1, 0.5, 6);
            var vdb = new Vector3D(-1, -0.5, 6);
            wb.DrawLine(wb.ProjectVertex(vaf), wb.ProjectVertex(vbf), Colors.Blue);
            wb.DrawLine(wb.ProjectVertex(vbf), wb.ProjectVertex(vcf), Colors.Blue);
            wb.DrawLine(wb.ProjectVertex(vcf), wb.ProjectVertex(vdf), Colors.Blue);
            wb.DrawLine(wb.ProjectVertex(vdf), wb.ProjectVertex(vaf), Colors.Blue);

            wb.DrawLine(wb.ProjectVertex(vab), wb.ProjectVertex(vbb), Colors.Red);
            wb.DrawLine(wb.ProjectVertex(vbb), wb.ProjectVertex(vcb), Colors.Red);
            wb.DrawLine(wb.ProjectVertex(vcb), wb.ProjectVertex(vdb), Colors.Red);
            wb.DrawLine(wb.ProjectVertex(vdb), wb.ProjectVertex(vab), Colors.Red);

            wb.DrawLine(wb.ProjectVertex(vaf), wb.ProjectVertex(vab), Colors.Green);
            wb.DrawLine(wb.ProjectVertex(vbf), wb.ProjectVertex(vbb), Colors.Green);
            wb.DrawLine(wb.ProjectVertex(vcf), wb.ProjectVertex(vcb), Colors.Green);
            wb.DrawLine(wb.ProjectVertex(vdf), wb.ProjectVertex(vdb), Colors.Green);
        }

    }
}
