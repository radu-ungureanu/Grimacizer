using Grimacizer7.DAL;
using Grimacizer7.DAL.Tables;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Grimacizer7.Utils
{
    public class LocalImagesHelper
    {
        public static WriteableBitmap ReadImageFromIsolatedStorage(string imageName, int width, int height)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var fileStream = isoStore.OpenFile(imageName, FileMode.Open, FileAccess.Read))
                {
                    var bitmap = new WriteableBitmap(width, height);
                    bitmap.SetSource(fileStream);
                    return bitmap;
                }
            }
        }

        public static void WriteImageToIsolatedStorage(string imageName, WriteableBitmap image)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var targetStream = isoStore.OpenFile(imageName, FileMode.Create, FileAccess.Write))
                {
                    image.SaveJpeg(targetStream, image.PixelWidth, image.PixelHeight, 0, 100);
                }
            }
        }
    }

    public class CameraHelpers
    {
        public static CompositeTransform GetCameraTransformByOrientation(CameraType cameraType, PageOrientation orientation)
        {
            var scaleX = 1;
            var scaleY = 1;
            var rotation = 0;

            if (cameraType == CameraType.FrontFacing)
            {
                scaleX = -1;
                if (orientation == PageOrientation.LandscapeRight)
                {
                    rotation = 180;
                }
                else if (orientation == PageOrientation.LandscapeLeft)
                {
                }
                else if (orientation == PageOrientation.PortraitUp)
                {
                    rotation = 90;
                }
            }
            else
            {
                if (orientation == PageOrientation.LandscapeRight)
                {
                    scaleX = -1;
                    scaleY = -1;
                }
                else if (orientation == PageOrientation.LandscapeLeft)
                {
                    // all good
                }
                else if (orientation == PageOrientation.PortraitUp)
                {
                    rotation = 90;
                }
            }

            return new CompositeTransform
            {
                CenterX = 0.5, 
                CenterY = 0.5, 
                ScaleX = scaleX, 
                ScaleY = scaleY, 
                Rotation = rotation 
            };
        }
    }

    public class WriteableBitmapHelpers
    {
        public static WriteableBitmap TransformBitmapByCameraTypeAndPageOrientation(WriteableBitmap writeableBmp, CameraType cameraType, PageOrientation orientation)
        {
            if (cameraType == CameraType.FrontFacing)
            {
                if (orientation == PageOrientation.LandscapeRight)
                {
                }
                else if (orientation == PageOrientation.LandscapeLeft)
                {
                    writeableBmp = writeableBmp.Rotate(180);
                }
                else if (orientation == PageOrientation.PortraitUp)
                {
                    writeableBmp = writeableBmp.Rotate(90);
                }
                writeableBmp = writeableBmp.Flip(WriteableBitmapExtensions.FlipMode.Horizontal);
            }
            else
            {
                if (orientation == PageOrientation.LandscapeRight)
                {
                    writeableBmp = writeableBmp.Rotate(180);
                }
                else if (orientation == PageOrientation.LandscapeLeft)
                {
                }
                else if (orientation == PageOrientation.PortraitUp)
                {
                    writeableBmp = writeableBmp.Rotate(90);
                }
            }

            return writeableBmp;
        }
    }

    public static class FaceCalculationsHelpers
    {
        public static double[] FromFaceCalculationsToVector(FaceCalculationsItem calculations)
        {
            var result = new double[16];

            result[0] = calculations._0_TamplaStanga;
            result[1] = calculations._1_TamplaDreapta;
            result[2] = calculations._2_Barbie;
            result[3] = calculations._3_SpranceanaDreapta;
            result[4] = calculations._4_ArieOchiStang;
            result[5] = calculations._5_ArieOchiDrept;
            result[6] = calculations._6_MarimeOchiStang;
            result[7] = calculations._7_MarimeOchiDrept;
            result[8] = calculations._8_InaltimeGura;
            result[9] = calculations._9_Unghi_60_67;
            result[10] = calculations._10_Unghi_64_65;
            result[11] = calculations._11_LungimeGuraExterior;
            result[12] = calculations._12_UnghiNasStanga;
            result[13] = calculations._13_UnghiNasDreapta;
            result[14] = calculations._14_ArieFata;
            result[15] = calculations._15_ArieGura;

            return result;
        }

        public static string DisplayFaceCalculations(FaceCalculationsItem faceCalculatios)
        {
            var builder = new StringBuilder();

            builder.AppendLine("tampla stanga " + faceCalculatios._0_TamplaStanga);
            builder.AppendLine("tampla dreapta " + faceCalculatios._1_TamplaDreapta);
            builder.AppendLine("marime barbie " + faceCalculatios._2_Barbie);
            builder.AppendLine("unghi sprancene dreapta stanga " + faceCalculatios._3_SpranceanaDreapta);
            builder.AppendLine("arie ochi stang " + faceCalculatios._4_ArieOchiStang);
            builder.AppendLine("arie ochi drept " + faceCalculatios._5_ArieOchiDrept);
            builder.AppendLine("marime ochi stang " + faceCalculatios._6_MarimeOchiStang);
            builder.AppendLine("marime ochi drept " + faceCalculatios._7_MarimeOchiDrept);
            builder.AppendLine("marime gura " + faceCalculatios._8_InaltimeGura);
            builder.AppendLine("unghi gura stanga " + faceCalculatios._9_Unghi_60_67);
            builder.AppendLine("unghi gura dreapta " + faceCalculatios._10_Unghi_64_65);
            builder.AppendLine("lungime gura " + faceCalculatios._11_LungimeGuraExterior);
            builder.AppendLine("unghi nas stg" + faceCalculatios._12_UnghiNasStanga);
            builder.AppendLine("unghi nas dr" + faceCalculatios._13_UnghiNasDreapta);
            builder.AppendLine("arie fata" + faceCalculatios._14_ArieFata);
            builder.AppendLine("arie gura" + faceCalculatios._15_ArieGura);

            return builder.ToString();
        }
    }

    public static class FacebookClientHelpers
    {
        public static void SaveToken(string token)
        {
            // If it is the first save, create the key on ApplicationSettings and save the token, otherwise just modify the key
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("token"))
                IsolatedStorageSettings.ApplicationSettings.Add("token", token);
            else
                IsolatedStorageSettings.ApplicationSettings["token"] = token;

            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static string GetToken()
        {
            //If there's no Token on memory, just return null, otherwise return the token as string
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("token"))
                return null;
            else
                return IsolatedStorageSettings.ApplicationSettings["token"] as string;
        }
    }

    public static class InGameLifeHelpers
    {
        public static void LoseLife()
        {
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();
                settings.LastMomentOfDeath = DateTime.Now;
                settings._20_NumberLivesLeft = Math.Max(0, settings._20_NumberLivesLeft - 1);
                // TODO: uncomment this
                //db.SubmitChanges();
            }
        }
    }
}
