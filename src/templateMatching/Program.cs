using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
/*using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.VideoSurveillance;
using Emgu.Util;
using NUnit.Framework;
using Emgu.CV.VideoStab;*/
using OpenCvSharp;
using NumSharp;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace templateMatching
{
    class Program
    {
        static void Main(string[] args)
        {
            liveCameraTemplate();

        }
        public static void liveCameraTemplate()
        {
            var color = Scalar.FromRgb(0, 255, 0);

            using (VideoCapture capture = new VideoCapture(0))
            using (Window webcam = new Window("Webcam"))
            using (Window resWin = new Window("RES"))
            using (Mat srcImage = new Mat())
            using (Mat templateImg = new Mat("C:\\Users\\cwang\\Documents\\Downloads\\apple.jpg"))
            {
                while (capture.IsOpened())
                {
                    capture.Read(srcImage);

                    Mat res = new Mat(srcImage.Rows - templateImg.Rows + 1, srcImage.Cols - templateImg.Cols + 1, MatType.CV_32FC1);

                    Cv2.MatchTemplate(srcImage, templateImg, res, TemplateMatchModes.CCoeffNormed);
                    Cv2.Threshold(res, res, 0.1, 1.0, ThresholdTypes.Tozero);

                    while (true)
                    {
                        double minval, maxval, threshold = 0.7;
                        Point minloc, maxloc;
                        Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                        if (maxval >= threshold)
                        {
                            //Setup the rectangle to draw
                            Rect r = new Rect(new Point(maxloc.X, maxloc.Y), new Size(templateImg.Width, templateImg.Height));
                            //System.Console.WriteLine($"MinVal={minval.ToString()} MaxVal={maxval.ToString()} MinLoc={minloc.ToString()} MaxLoc={maxloc.ToString()} Rect={r.ToString()}");

                            //Draw a rectangle of the matching area
                            Cv2.Rectangle(srcImage, r, Scalar.LimeGreen, 2);

                            //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                            Rect outRect;
                            resWin.ShowImage(res);
                            Cv2.FloodFill(res, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));
                        }
                        else
                            break;
                        //int key1 = Cv2.WaitKey(1);
                    }
                    webcam.ShowImage(srcImage);
                    int key = Cv2.WaitKey(1);
                    if (key == 27)
                    {
                        break;
                    }

                }
            }
        }
        public static void staticImageTemplate()
        {

            var color = Scalar.FromRgb(0, 255, 0);

            using (Mat srcImage = new Mat("C:\\Users\\cwang\\Documents\\Downloads\\skyline.png"))
            using (Mat templateImg = new Mat("C:\\Users\\cwang\\Documents\\Downloads\\tower.png"))
            //using (Mat res = new Mat(srcImage.Rows - templateImg.Rows + 1, srcImage.Cols - templateImg.Cols + 1, MatType.CV_32FC1))
            using (Mat res = new Mat())
            {
                Cv2.MatchTemplate(srcImage, templateImg, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        //Setup the rectangle to draw
                        Rect r = new Rect(new Point(maxloc.X, maxloc.Y), new Size(templateImg.Width, templateImg.Height));
                        System.Console.WriteLine($"MinVal={minval.ToString()} MaxVal={maxval.ToString()} MinLoc={minloc.ToString()} MaxLoc={maxloc.ToString()} Rect={r.ToString()}");

                        //Draw a rectangle of the matching area
                        Cv2.Rectangle(srcImage, r, Scalar.LimeGreen, 2);

                        //Fill in the res Mat so you don't find the same area again in the MinMaxLoc
                        Rect outRect;
                        Cv2.FloodFill(res, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));
                    }
                    else
                        break;
                }

                Cv2.ImShow("Template Matching", srcImage);
                Cv2.WaitKey(0);

            }
        }
    }
}