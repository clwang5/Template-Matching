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
using System.Windows.Forms;

namespace templateMatching
{
    class Program
    {
        private static int width = 640;
        private static int height = 480;

        [STAThread]
        static void Main(string[] args)
        {

            StaticImageTemplate();
        }

        public enum Selection { template, srcImg }
        public static string BrowseFile(Selection selection)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (selection == Selection.template)
                {
                    openFileDialog.Title = "Select the template img you want to match";
                }
                else
                {

                    openFileDialog.Title = "Select the src static img you want to match the template from";
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    return openFileDialog.FileName;

                }
            }
            return string.Empty;
        }

        public static bool ValidImg(string path)
        {
            using (Mat templateImg = new Mat(path))
            {
                if (templateImg.Height > height || templateImg.Width > width)
                {
                    return false;
                }
                return true;
            }
        }
        public static void LiveCameraTemplate()
        {
            var tempImgPath = BrowseFile(Selection.template);

            using (VideoCapture capture = new VideoCapture(0))
            using (Window webcam = new Window("Webcam"))
            using (Mat srcImage = new Mat())
            using (Mat templateImg = new Mat(tempImgPath))
            {
                while (capture.IsOpened())
                {
                    capture.Read(srcImage);
                    width = srcImage.Width;
                    height = srcImage.Height;
                    if (!ValidImg(tempImgPath))
                    {
                        System.Windows.Forms.MessageBox.Show("File selection error, make sure you select a valid template image that has smaller dimensions than the source image");
                        break;
                    }

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
                            Cv2.FloodFill(res, maxloc, new Scalar(0), out outRect, new Scalar(0.1), new Scalar(1.0));
                        }
                        else
                            break;
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
        public static void StaticImageTemplate()
        {
            var srcImgPath = BrowseFile(Selection.srcImg);
            var templateImgPath = BrowseFile(Selection.template);
            using (Mat srcImage = new Mat(srcImgPath))
            using (Mat templateImg = new Mat(templateImgPath))
            //using (Mat res = new Mat(srcImage.Rows - templateImg.Rows + 1, srcImage.Cols - templateImg.Cols + 1, MatType.CV_32FC1))
            using (Mat res = new Mat())
            {
                width = srcImage.Width;
                height = srcImage.Height;
                if (!ValidImg(templateImgPath))
                {
                    System.Windows.Forms.MessageBox.Show("File selection error, make sure you select a valid template image that has smaller dimensions than the source image");
                }
                else
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
                            //System.Console.WriteLine($"MinVal={minval.ToString()} MaxVal={maxval.ToString()} MinLoc={minloc.ToString()} MaxLoc={maxloc.ToString()} Rect={r.ToString()}");

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
}