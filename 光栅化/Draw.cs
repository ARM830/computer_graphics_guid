using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace 光栅化
{
    public class PointInt
    {

        public PointInt(Point p) : this(p.X, p.Y)
        {

        }
        public PointInt(double x, double y)
        {
            this.X = Convert.ToInt32(Math.Round(x, 0, MidpointRounding.ToEven));
            this.Y = Convert.ToInt32(Math.Round(y, 0, MidpointRounding.ToEven));
        }

        public int X
        {
            get; set;
        }
        public int Y
        {
            get; set;
        }
    }
    public static class Draw
    {
        public static int bitmapheight = 800;
        public static int bitmapwidth = 800;
        public static Dictionary<double, Double> Interpolate(double i0, double d0, double i1, double d1)
        {
            Dictionary<double, Double> list = new Dictionary<double, Double>();
            if (i0 == i1)
            {
                list.Add(i0, d0);
                return list;
            }
            var a = (d1 - d0) / (i1 - i0);
            var d = d0;
            for (double i = i0; i <= i1; i++)
            {
                list.Add(i, d);
                d += a;
            }
            return list;
        }
        public static void DrawTriangle(this WriteableBitmap bitmap, Point p0, Point p1, Point p2, Color color)
        {
            bitmap.DrawLine(p0, p1, color);
            bitmap.DrawLine(p1, p2, color);
            bitmap.DrawLine(p2, p0, color);
        }
        public static void DrawLine(this WriteableBitmap bitmap, Point start, Point end, Color color)
        {
            bitmap.Lock();
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;
            byte[] colorData = { color.B, color.G, color.R, color.A };
            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (start.X > end.X)
                {
                    Swap(ref start, ref end);

                }

                var y = Interpolate(start.X, start.Y, end.X, end.Y);
                for (double x = start.X; x < end.X; x++)
                {
                    var p = new Point(x, y[x]).ConverterPointInt();
                    bitmap.WritePixels(new Int32Rect(p.X, p.Y, 1, 1), colorData, stride, 0);

                }
            }
            else
            {
                if (start.Y > end.Y)
                {
                    Swap(ref start, ref end);

                }
                var x = Interpolate(start.Y, start.X, end.Y, end.X);
                for (double y = start.Y; y < end.Y; y++)
                {

                    var p = new Point(x[y], y).ConverterPointInt();
                    bitmap.WritePixels(new Int32Rect(p.X, p.Y, 1, 1), colorData, stride, 0);

                }
            }
            bitmap.Unlock();
        }
        public static void DrawLineNormal(this WriteableBitmap bitmap, Point start, Point end, Color color)
        {
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                //接近水平
                bitmap.Line2(start, end, color);
            }
            else
            {
                //接近垂直
                bitmap.Line4(start, end, color);
            }
        }

        public static void Line4(this WriteableBitmap bitmap, Point start, Point end, Color color)
        {
            bitmap.Lock();
            if (start.Y > end.Y)
            {
                Swap(ref start, ref end);
            }
            var a = (end.X - start.X) / (end.Y - start.Y);
            byte[] colorData = { color.B, color.G, color.R, color.A };
            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
            var x = start.X;
            for (double y = start.Y; y < end.Y; y++)
            {
                var p = new Point(x, y).ConverterPoint();
                bitmap.WritePixels(new Int32Rect((int)p.X, (int)p.Y, 1, 1), colorData, stride, 0);
                x += a;
            }
            bitmap.Unlock();
        }
        /// <summary>
        /// x=f(x)
        /// <para>x=x0+(1/(y1-y0))*(y-y0)*(x1-x0)</para>
        /// <para>x=x0+(y-y0)*((x1-x0)/(y1-y0))</para>
        /// <para>((x1-x0)/(y1-y0))=a</para>
        /// <para>x=x0+(y-y0)*a</para>
        /// <para>x=x0+y*a-y0*a</para>
        /// <para>x=y*a+(x0-y0*a)</para>
        /// <para>(x0-y0*a)=b</para>
        ///  <para>x=ay+b</para>
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        public static void Line3(this WriteableBitmap bitmap, Point start, Point end, Color color)
        {
            bitmap.Lock();
            var a = (end.X - start.X) / (end.Y - start.Y);
            var b = start.X - a * start.Y;
            byte[] colorData = { color.B, color.G, color.R, color.A };
            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
            for (double y = start.Y; y < end.Y; y++)
            {
                var x = a * y + b;
                var p = new Point(x, y).ConverterPointInt();
                bitmap.WritePixels(new Int32Rect(p.X, p.Y, 1, 1), colorData, stride, 0);
            }
            bitmap.Unlock();
        }

        /// <summary>
        /// y(x)=ax+b
        /// <para>y(x+1)=a(x+1)+b</para>
        /// <para>y(x+1)=ax+a+b</para>
        /// <para>y(x+1)=(ax+b)+a</para>
        /// <para>y(x+1)=y(x)+a</para>
        /// 递归
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="end">end</param>
        /// <param name="color">color</param>
        public static void Line2(this WriteableBitmap bitmap, Point start, Point end, Color color)
        {
            bitmap.Lock();
            if (start.X > end.X)
            {
                Swap(ref start, ref end);
            }
            var a = (end.Y - start.Y) / (end.X - start.X);
            byte[] colorData = { color.B, color.G, color.R, color.A };
            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
            var y = start.Y;
            for (double x = start.X; x < end.X; x++)
            {
                var p = new Point(x, y).ConverterPoint();
                bitmap.WritePixels(new Int32Rect((int)p.X, (int)p.Y, 1, 1), colorData, stride, 0);
                y += a;
            }
            bitmap.Unlock();
        }
        public static void Line1(this WriteableBitmap bitmap, Point start, Point end, Color color)
        {
            bitmap.Lock();
            var a = (end.Y - start.Y) / (end.X - start.X);
            var b = start.Y - a * start.X;
            byte[] colorData = { color.B, color.G, color.R, color.A };
            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel) / 8;
            for (double x = start.X; x < end.X; x++)
            {
                var y = a * x + b;
                var p = new Point(x, y).ConverterPointInt();
                bitmap.WritePixels(new Int32Rect(p.X, p.Y, 1, 1), colorData, stride, 0);

            }
            bitmap.Unlock();
        }

        public static Point ConverterPoint(this Point point)
        {
            return new Point(point.X + bitmapwidth / 2, bitmapheight / 2 - point.Y);
        }
        public static PointInt ConverterPointInt(this Point point)
        {
            return new PointInt(point.X + bitmapwidth / 2, bitmapheight / 2 - point.Y);
        }
        public static void Swap(ref Point start, ref Point end)
        {

            (start, end) = (end, start);
        }
    }
}
