using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using Microsoft.Phone.Shell;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views.Other
{
    public partial class EditProfile : NotifyPhoneApplicationPage
    {
        private string _playerName;
        public string PlayerName
        {
            get
            {
                return _playerName;
            }
            set
            {
                if (_playerName != value)
                {
                    _playerName = value;
                    NotifyPropertyChanged(() => PlayerName);
                }
            }
        }
        private WriteableBitmap _defaultPhoto;
        public WriteableBitmap DefaultPhoto
        {
            get
            {
                return _defaultPhoto;
            }
            set
            {
                if (_defaultPhoto != value)
                {
                    _defaultPhoto = value;
                    NotifyPropertyChanged(() => DefaultPhoto);
                }
            }
        }
        private WriteableBitmap _defaultCartoon;
        public WriteableBitmap DefaultCartoon
        {
            get
            {
                return _defaultCartoon;
            }
            set
            {
                if (_defaultCartoon != value)
                {
                    _defaultCartoon = value;
                    NotifyPropertyChanged(() => DefaultCartoon);
                }
            }
        }

        public EditProfile()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;

            int width, height;
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings =  db.Settings.FirstOrDefault();
                PlayerName = settings.PlayerName;
                width = settings.DefaultImagePixelWidth;
                height = settings.DefaultImagePixelHeight;
            }

            DefaultPhoto = LocalImagesHelper.ReadImageFromIsolatedStorage(Constants.DEFAULT_FACE_PHOTO, width, height);
            DefaultCartoon = LocalImagesHelper.ReadImageFromIsolatedStorage(Constants.DEFAULT_CARTOON_PHOTO, Constants.CartoonWidth, Constants.CartoonHeight);
        }

        private void ResetProfile_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset your profile?", "Confirm", MessageBoxButton.OKCancel);
            switch (result)
            {
                case MessageBoxResult.OK:
                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        isoStore.DeleteFile(Constants.DEFAULT_SETTINGS_FILE);
                        isoStore.DeleteFile(Constants.DEFAULT_FACE_PHOTO);
                        isoStore.DeleteFile(Constants.DEFAULT_CARTOON_PHOTO);

                        using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                        {
                            db.Settings.FirstOrDefault().Initialized = false;
                            db.SubmitChanges();
                        }
                    }

                    NavigationService.Navigate(new Uri(Pages.CreateProfile_CreateProfile, UriKind.RelativeOrAbsolute));
                    break;
            }
        }

        private void Details_Click(object sender, EventArgs e)
        {
            MessageBox.Show(FaceCalculationsHelpers.DisplayFaceCalculations(Calculus.ReadDefaultFaceCalculations()));
        }

        private void EditProfile_Click(object sender, EventArgs e)
        {
            var path = string.Format("{0}?editprofile=true", Pages.CreateProfile_CartoonCreator);
            NavigationService.Navigate(new Uri(path, UriKind.RelativeOrAbsolute));
        }
    }
}