using Microsoft.Devices;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Grimacizer7.Common
{
    public static class Extensions
    {
        public static Task BeginWorkerAsync(this BackgroundWorker worker)
        {
            var taskSource = new TaskCompletionSource<object>();
            RunWorkerCompletedEventHandler completed = null;
            completed += (s, e) =>
            {
                worker.RunWorkerCompleted -= completed;
                taskSource.SetResult(null);
            };
            worker.RunWorkerCompleted += completed;
            worker.RunWorkerAsync();
            return taskSource.Task;
        }


        public static Task CaptureImageAsync(this PhotoCamera camera)
        {
            var taskSource = new TaskCompletionSource<object>();
            EventHandler<ContentReadyEventArgs> completed = null;
            completed += (s, e) =>
            {
                camera.CaptureImageAvailable -= completed;
                taskSource.SetResult(null);
            };
            camera.CaptureImageAvailable += completed;
            camera.CaptureImage();
            return taskSource.Task;
        }

        public static async Task WaitAsync(int miliseconds)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (a, b) =>
            {
                Thread.Sleep(miliseconds);
            }; ;
            await worker.BeginWorkerAsync();
        }
    }

    public static class ExpressionExtensions
    {
        public static string GetBeforeLastMemberName(this LambdaExpression selector)
        {
            return selector.Body.AsMemberExpression().Expression.AsMemberExpression().Member.Name;
        }

        public static string GetLastMemberName(this LambdaExpression selector)
        {
            return selector.Body.AsMemberExpression().Member.Name;
        }

        private static MemberExpression AsMemberExpression(this Expression expression)
        {
            return (MemberExpression)expression;
        }
    }

    public static class StringExtensions
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }
    }

    public static class BitmapImageExtensions
    {
        public static Stream GetStream(this WriteableBitmap image)
        {
            var memoryStream = new MemoryStream();
            image.SaveJpeg(memoryStream, image.PixelWidth, image.PixelHeight, 0, 100);
            return memoryStream;
        }
    }
}
