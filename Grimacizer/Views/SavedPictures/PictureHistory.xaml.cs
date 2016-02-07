using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using Microsoft.Phone.Shell;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views.SavedPictures
{
    public partial class PictureHistory : NotifyPhoneApplicationPage
    {
        public ObservableCollection<HistoryImage> History { get; set; }

        public PictureHistory()
        {
            InitializeComponent();
            History = new ObservableCollection<HistoryImage>();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;

            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                History.Clear();
                var cartoon = LocalImagesHelper.ReadImageFromIsolatedStorage(Constants.DEFAULT_CARTOON_PHOTO, Constants.CartoonWidth, Constants.CartoonHeight);
                History.Add(new HistoryImage
                {
                    Name = Constants.DEFAULT_CARTOON_PHOTO,
                    Image = cartoon,
                    Width = Constants.CartoonWidth,
                    Height = Constants.CartoonHeight
                });
                var savedPictures = db.SavedPictures;
                foreach (var picture in savedPictures)
                {
                    var image = LocalImagesHelper.ReadImageFromIsolatedStorage(picture.Name, picture.Width, picture.Height);
                    History.Add(new HistoryImage
                    {
                        Name = picture.Name,
                        Image = image,
                        Width = picture.Width,
                        Height = picture.Height
                    });
                }
                NotifyPropertyChanged(() => History);
            }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var image = (sender as Image).DataContext as HistoryImage;
            var path = string.Format(
                "{0}?image={1}&width={2}&height={3}",
                Pages.PictureFullSize,
                HttpUtility.UrlEncode(image.Name),
                image.Width,
                image.Height);
            NavigationService.Navigate(new Uri(path, UriKind.RelativeOrAbsolute));
        }
    }

    public class HistoryImage
    {
        public string Name { get; set; }
        public WriteableBitmap Image { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}