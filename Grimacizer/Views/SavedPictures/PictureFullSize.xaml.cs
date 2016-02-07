using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using Microsoft.Phone.Shell;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views.SavedPictures
{
    public partial class PictureFullSize : NotifyPhoneApplicationPage
    {
        private string _pictureName;
        private int _pictureWidth;
        private int _pictureHeight;
        private WriteableBitmap _image;
        public WriteableBitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    NotifyPropertyChanged(() => Image);
                }
            }
        }

        public PictureFullSize()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;
            _pictureName = HttpUtility.UrlDecode(NavigationContext.QueryString["image"]);
            _pictureWidth = int.Parse(NavigationContext.QueryString["width"]);
            _pictureHeight = int.Parse(NavigationContext.QueryString["height"]);
            Image = LocalImagesHelper.ReadImageFromIsolatedStorage(_pictureName, _pictureWidth, _pictureHeight);

            if (_pictureName == Constants.DEFAULT_CARTOON_PHOTO)
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
        }

        private void DeleteImage_Click(object sender, EventArgs e)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(_pictureName))
                {
                    isoStore.DeleteFile(_pictureName);
                }
            }
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var picture = db.SavedPictures.FirstOrDefault(t => t.Name == _pictureName);
                db.SavedPictures.DeleteOnSubmit(picture);
                db.SubmitChanges();
            }
            NavigationService.GoBack();
        }

        private void ShareImage_Click(object sender, EventArgs e)
        {
            var path = string.Format(
                "{0}?image={1}&width={2}&height={3}",
                Pages.ShareFacebook,
                HttpUtility.UrlEncode(_pictureName),
                _pictureWidth,
                _pictureHeight);
            NavigationService.Navigate(new Uri(path, UriKind.Relative));
        }
    }
}