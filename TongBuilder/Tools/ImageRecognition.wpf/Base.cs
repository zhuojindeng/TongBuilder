using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System.Diagnostics;
using System.Drawing;

namespace ImageRecognition.wpf
{
    public static class Base
    {
        /// <summary>
        /// 灰度
        /// </summary>
        /// <param name="imgpath"></param>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public static Mat Gray(string imgpath, out long runTime)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();
            Mat image = CvInvoke.Imread(imgpath, ImreadModes.Grayscale);
            runTime = sp.ElapsedMilliseconds;
            return image;
        }
        /// <summary>
        /// 边缘
        /// </summary>
        /// <param name="imgpath"></param>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public static Mat Canny(string imgpath, out long runTime)
        {
            Mat image = CvInvoke.Imread(imgpath, ImreadModes.Color);
            Stopwatch sp = new Stopwatch();
            sp.Start();

            Mat result = new Mat();
            CvInvoke.Canny(image, result, 100, 60);
            runTime = sp.ElapsedMilliseconds;
            return result;
        }

        public static Mat FindContours(Mat image, out long runTime)
        {
            Mat result = new Mat(image.Size, DepthType.Cv8S, 3);// new Bitmap(image.Size.Width, image.Size.Height);

            Stopwatch sp = new Stopwatch();
            sp.Start();
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(image, contours, null, RetrType.List, ChainApproxMethod.ChainApproxNone);
                int count = contours.Size;
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        if (contour.ToArray().Max(x => x.Y) < 200)
                            continue;
                        MCvScalar color = new Bgr(Color.Green).MCvScalar;
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        //if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
                        //{
                        if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                        {
                            color = new Bgr(Color.Red).MCvScalar;
                        }
                        else if (approxContour.Size == 4) //The contour has 4 vertices.
                        {
                            color = new Bgr(Color.Yellow).MCvScalar;
                        }
                        else
                        {
                            color = new Bgr(Color.Orange).MCvScalar;
                        }
                        //}
                        //画出所有点
                        //for (int j=0;j< contour.Size;j++)
                        //{

                        //}
                        CvInvoke.Polylines(result, contour, true, color, 1);
                    }
                }

            }
            runTime = sp.ElapsedMilliseconds;
            return result;
        }
    }
}
