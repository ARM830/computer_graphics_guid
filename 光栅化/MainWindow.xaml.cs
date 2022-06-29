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
            // WriteableBitmap.DrawLineNormal(new Point(100, -200), new Point(60, 240), Colors.Red);
            //  WriteableBitmap.DrawLineNormal(new Point(200, -300), new Point(160, 340), Colors.Red);
            WriteableBitmap.DrawLine(new Point(0, -300), new Point(0,300), Colors.Black);
            WriteableBitmap.DrawLine(new Point(-300, 0), new Point(300, 0), Colors.Black);
            WriteableBitmap.DrawTriangle(new Point(-200,-250), new Point(200, 50), new Point(20, 250),Colors.Red);


        }

    }
}
