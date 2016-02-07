using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using Microsoft.FaceSdk.ImageHelper;
using Microsoft.Phone.Shell;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views.CreateProfile
{
    public partial class ProfileCreated : NotifyPhoneApplicationPage
    {
        private SdkHelper FaceSdkHelper;

        private bool _isCalculating;
        public bool IsCalculating
        {
            get
            {
                return _isCalculating;
            }
            set
            {
                if (_isCalculating != value)
                {
                    _isCalculating = value;
                    NotifyPropertyChanged(() => IsCalculating);
                }
            }
        }
        private WriteableBitmap _imageSource;
        public WriteableBitmap ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    NotifyPropertyChanged(() => ImageSource);
                }
            }
        }

        public ProfileCreated()
        {
            InitializeComponent();
            FaceSdkHelper = new SdkHelper();
            DataContext = this;
            IsCalculating = true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;

            int width, height;
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();
                width = settings.DefaultImagePixelWidth;
                height = settings.DefaultImagePixelHeight;
            }
            ImageSource = LocalImagesHelper.ReadImageFromIsolatedStorage(Constants.DEFAULT_FACE_PHOTO, width, height);

            var worker = new BackgroundWorker();
            worker.DoWork += (sender, arg) =>
            {
                FaceSdkHelper.Initialize();
            };
            await worker.BeginWorkerAsync();

            var sdkImg = ImageConverter.SystemToSdk(ImageSource);
            var result = FaceSdkHelper.Detect(sdkImg);
            ImageSource = ImageConverter.SdkToSystem(result);

            if (SdkHelper.foundFace == true && SdkHelper.results.Count == 1)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                IsCalculating = false;
            }
            else
            {
                MessageBox.Show("Camera did not detect your face! Try again!");
                NavigationService.GoBack();
            }

            FaceSdkHelper = null;
            GC.Collect();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            Calculus.WriteDefaultFaceCalculation(SdkHelper.results.FirstOrDefault());
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();

                settings._16_PassedLevels = Constants.DefaultAvailableLevels;
                settings._17_SurvivalScore = 0;
                settings._18_MultiplayerWinScore = 0;
                settings._19_MultiplayerLoseScore = 0;
                settings._20_NumberLivesLeft = Constants.MaxLives;
                settings.LastMomentOfDeath = DateTime.Now;

                foreach (var level in db.Levels)
                {
                    level.Stars = 0;
                }
                db.SubmitChanges();
            }

            NavigationService.Navigate(new Uri(Pages.CreateProfile_CartoonCreator, UriKind.RelativeOrAbsolute));
        }

        private void Back_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}