using FaceAlignmentDemo.Wp7;
using Grimacizer7.DAL;
using Microsoft.FaceSdk.Common;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Linq;
using Grimacizer7.DAL.Tables;
using System.Text;

namespace Grimacizer7.Utils
{
    public class Calculus
    {
        public static FaceCalculationsItem ReadDefaultFaceCalculations()
        {
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                return db.FaceCalculations.FirstOrDefault();
            }
        }

        public static void WriteDefaultFaceCalculation(PointF[] shape)
        {
            var calculatedFaceAlignment = GetFaceCalculationsFromShape(shape);
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var faceCalculations = db.FaceCalculations.FirstOrDefault();

                faceCalculations._0_TamplaStanga = calculatedFaceAlignment._0_TamplaStanga;
                faceCalculations._1_TamplaDreapta = calculatedFaceAlignment._1_TamplaDreapta;
                faceCalculations._2_Barbie = calculatedFaceAlignment._2_Barbie;
                faceCalculations._3_SpranceanaDreapta = calculatedFaceAlignment._3_SpranceanaDreapta;
                faceCalculations._4_ArieOchiStang = calculatedFaceAlignment._4_ArieOchiStang;
                faceCalculations._5_ArieOchiDrept = calculatedFaceAlignment._5_ArieOchiDrept;
                faceCalculations._6_MarimeOchiStang = calculatedFaceAlignment._6_MarimeOchiStang;
                faceCalculations._7_MarimeOchiDrept = calculatedFaceAlignment._7_MarimeOchiDrept;
                faceCalculations._8_InaltimeGura = calculatedFaceAlignment._8_InaltimeGura;
                faceCalculations._9_Unghi_60_67 = calculatedFaceAlignment._9_Unghi_60_67;
                faceCalculations._10_Unghi_64_65 = calculatedFaceAlignment._10_Unghi_64_65;
                faceCalculations._11_LungimeGuraExterior = calculatedFaceAlignment._11_LungimeGuraExterior;
                faceCalculations._12_UnghiNasStanga = calculatedFaceAlignment._12_UnghiNasStanga;
                faceCalculations._13_UnghiNasDreapta = calculatedFaceAlignment._13_UnghiNasDreapta;
                faceCalculations._14_ArieFata = calculatedFaceAlignment._14_ArieFata;
                faceCalculations._15_ArieGura = calculatedFaceAlignment._15_ArieGura;

                db.SubmitChanges();
            }
        }

        public static FaceCalculationsItem GetFaceCalculationsFromShape(PointF[] shape)
        {
            float leftEyeArea =
                AreaForTriangle(shape[0], shape[1], shape[7]) +
                AreaForTriangle(shape[1], shape[2], shape[7]) +
                AreaForTriangle(shape[2], shape[6], shape[7]) +
                AreaForTriangle(shape[2], shape[3], shape[6]) +
                AreaForTriangle(shape[3], shape[5], shape[6]) +
                AreaForTriangle(shape[3], shape[4], shape[5]);

            float rightEyeArea =
                AreaForTriangle(shape[8], shape[9], shape[15]) +
                AreaForTriangle(shape[9], shape[10], shape[15]) +
                AreaForTriangle(shape[10], shape[14], shape[15]) +
                AreaForTriangle(shape[10], shape[11], shape[14]) +
                AreaForTriangle(shape[11], shape[13], shape[14]) +
                AreaForTriangle(shape[11], shape[12], shape[13]);

            float mouthArea =
                AreaForTriangle(shape[60], shape[61], shape[67]) +
                AreaForTriangle(shape[61], shape[62], shape[67]) +
                AreaForTriangle(shape[62], shape[66], shape[67]) +
                AreaForTriangle(shape[62], shape[63], shape[66]) +
                AreaForTriangle(shape[63], shape[65], shape[66]) +
                AreaForTriangle(shape[63], shape[64], shape[65]);

            //float faceArea =
            //    AreaForTriangle(shape[68], new PointF(shape[68].X, shape[77].Y), shape[86]) +
            //    AreaForTriangle(shape[86], new PointF(shape[68].X, shape[77].Y), new PointF(shape[86].X, shape[77].Y));

            var faceCalculations = new FaceCalculationsItem();
            faceCalculations._0_TamplaStanga = DistanceBetweenPoints(shape[68], shape[69]);
            faceCalculations._1_TamplaDreapta = DistanceBetweenPoints(shape[86], shape[85]);
            faceCalculations._2_Barbie = DistanceBetweenPoints(shape[73], shape[81]);
            faceCalculations._3_SpranceanaDreapta = AngleMadeByPoints(shape[26], shape[27]);
            faceCalculations._4_ArieOchiStang = leftEyeArea.RoundTwoDigits();
            faceCalculations._5_ArieOchiDrept = rightEyeArea.RoundTwoDigits();
            faceCalculations._6_MarimeOchiStang = DistanceBetweenPoints(shape[2], shape[6]);
            faceCalculations._7_MarimeOchiDrept = DistanceBetweenPoints(shape[10], shape[14]);
            faceCalculations._8_InaltimeGura = DistanceBetweenPoints(shape[62], shape[66]);
            faceCalculations._9_Unghi_60_67 = DistanceBetweenPoints(shape[60], shape[67]);
            faceCalculations._10_Unghi_64_65 = AngleMadeByPoints(shape[64], shape[65]);
            faceCalculations._11_LungimeGuraExterior = DistanceBetweenPoints(shape[48], shape[54]);
            faceCalculations._12_UnghiNasStanga = AngleMadeByPoints(shape[39], shape[38]);
            faceCalculations._13_UnghiNasDreapta = AngleMadeByPoints(shape[45], shape[44]);
            faceCalculations._14_ArieFata = FaceArea(shape, 69);
            faceCalculations._15_ArieGura = mouthArea.RoundTwoDigits();

            return faceCalculations;
        }

        private static float Axis(PointF p1, PointF p2)
        {
            float x = (p1.X - p2.X) * (p1.X - p2.X);
            float y = (p2.Y - p1.Y) * (p2.Y - p1.Y);
            return (float)Math.Sqrt(x + y);
        }

        private static float SemiPerimeter(float side1, float side2, float side3)
        {
            return (side1 + side2 + side3) / 2;
        }

        private static float AreaBySemiPerimeter(float side1, float side2, float side3)
        {
            var sp = SemiPerimeter(side1, side2, side3);
            return Math.Sqrt(sp * (sp - side1) * (sp - side2) * (sp - side3)).RoundTwoDigits();
        }

        private static float AreaForRectangle(PointF a, PointF b, PointF c)
        {
            var sideA = DistanceBetweenPoints(a, b);
            var sideB = DistanceBetweenPoints(b, c);
            return (sideA * sideB).RoundTwoDigits();
        }

        private static float AreaForTriangle(PointF a, PointF b, PointF c)
        {
            return AreaBySemiPerimeter(a.DistanceTo(b), b.DistanceTo(c), c.DistanceTo(a));
        }

        private static float AngleMadeByPoints(PointF a, PointF b)
        {
            double hypotenuse = a.DistanceTo(b);
            double cathetus = Math.Abs(a.Y - b.Y);
            return Math.Asin(cathetus / hypotenuse).RoundTwoDigits();
        }

        private static float DistanceBetweenPoints(PointF a, PointF b)
        {
            return a.DistanceTo(b).RoundTwoDigits();
        }

        private static float FaceArea(PointF[] shape, int i)
        {
            if (i == 86) return 0;
            float dr1 = shape[68].DistanceTo(shape[i]);
            float dr2 = shape[68].DistanceTo(shape[i + 1]);
            float dr3 = shape[i].DistanceTo(shape[i + 1]);
            return AreaBySemiPerimeter(dr1, dr2, dr3) + FaceArea(shape, i + 1);
        }
    }

    public static class CalculusExtensions
    {
        public static float RoundTwoDigits(this float number)
        {
            return (float)Math.Round(number, 2);
        }

        public static float RoundTwoDigits(this double number)
        {
            return (float)Math.Round(number, 2);
        }
    }
}

