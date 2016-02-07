using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views.CreateProfile
{
    public partial class TakePhoto : NotifyPhoneApplicationPage
    {
        private double _cameraHeight, _cameraWidth;
        private double _phoneHeight, _phoneWidth;
        private PhotoCamera cam;
        private ApplicationBarIconButton nextButton, backButton, cameraButton;
        private PageOrientation orientation;
        private CameraType cameraType;

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    NotifyPropertyChanged(() => ErrorMessage);
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
        private bool _pictureIsTaken;
        public bool PictureIsTaken
        {
            get
            {
                return _pictureIsTaken;
            }
            set
            {
                if (_pictureIsTaken != value)
                {
                    _pictureIsTaken = value;
                    NotifyPropertyChanged(() => PictureIsTaken);
                }
            }
        }

        public TakePhoto()
        {
            InitializeComponent();
            InitButtons();
            InitScene();
            DataContext = this;
            PictureIsTaken = false;
        }

        private void InitScene()
        {
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                    cam = new PhotoCamera(CameraType.FrontFacing);
                else
                    cam = new PhotoCamera(CameraType.Primary);

                _cameraHeight = cam.AvailableResolutions.FirstOrDefault().Height;
                _cameraWidth = cam.AvailableResolutions.FirstOrDefault().Width;

                cam.CaptureImageAvailable += cam_CaptureImageAvailable;
                viewfinderBrush.SetSource(cam);
                PictureIsToBeTakenScenario();
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    ErrorMessage = "A Camera is not available on this device.";
                });
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _phoneHeight = Application.Current.Host.Content.ActualHeight;
            _phoneWidth = Application.Current.Host.Content.ActualWidth;
            InitScene();
            OnOrientationChanged(new OrientationChangedEventArgs(Orientation));
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                cam.Dispose();
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
            }
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                viewfinderBrush.RelativeTransform = CameraHelpers.GetCameraTransformByOrientation(cam.CameraType, Orientation);
                if (Orientation == PageOrientation.LandscapeRight || Orientation == PageOrientation.LandscapeLeft)
                {
                    var ratio = _cameraHeight / _cameraWidth;
                    cameraGrid.Width = ratio * _phoneHeight;
                    cameraGrid.Height = _phoneWidth;
                }
                else if (Orientation == PageOrientation.PortraitUp)
                {
                    var ratio = _cameraHeight / _cameraWidth;
                    cameraGrid.Width = _phoneWidth;
                    cameraGrid.Height = ratio * _phoneHeight;
                }
            }

            base.OnOrientationChanged(e);
        }

        private void CaptureImage_Click(object sender, EventArgs e)
        {
            if (cam != null)
            {
                cam.CaptureImage();
            }
        }

        void cam_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                orientation = Orientation;
                cameraType = cam.CameraType;

                var bmp = new BitmapImage();
                bmp.SetSource(e.ImageStream);
                var writeableBmp = BitmapFactory.New(bmp.PixelWidth, bmp.PixelHeight).FromStream(e.ImageStream);
                ImageSource = WriteableBitmapHelpers.TransformBitmapByCameraTypeAndPageOrientation(writeableBmp, cameraType, orientation);

                PictureAvailableScenario();
            });
        }

        private void next_Click(object sender, EventArgs e)
        {
            LocalImagesHelper.WriteImageToIsolatedStorage(Constants.DEFAULT_FACE_PHOTO, ImageSource);

            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();

                settings.DefaultImagePixelHeight = ImageSource.PixelHeight;
                settings.DefaultImagePixelWidth = ImageSource.PixelWidth;
                settings.PhoneOrientation = orientation;

                db.SubmitChanges();
            }

            NavigationService.Navigate(new Uri(Pages.CreateProfile_ProfileCreated, UriKind.RelativeOrAbsolute));
        }

        private void InitButtons()
        {
            nextButton = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Assets/AppBar/next.png", UriKind.RelativeOrAbsolute),
                IsEnabled = true,
                Text = "next"
            };
            nextButton.Click += next_Click;

            backButton = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Assets/AppBar/back.png", UriKind.RelativeOrAbsolute),
                IsEnabled = true,
                Text = "back"
            };
            backButton.Click += back_Click;

            cameraButton = new ApplicationBarIconButton
            {
                IconUri = new Uri("/Assets/AppBar/camera.png", UriKind.RelativeOrAbsolute),
                IsEnabled = true,
                Text = "snapshot"
            };
            cameraButton.Click += CaptureImage_Click;
        }

        private void PictureIsToBeTakenScenario()
        {
            PictureIsTaken = false;
            ApplicationBar.Buttons.Clear();
            ApplicationBar.Buttons.Add(cameraButton);
        }

        private void PictureAvailableScenario()
        {
            PictureIsTaken = true;
            ApplicationBar.Buttons.Clear();
            ApplicationBar.Buttons.Add(backButton);
            ApplicationBar.Buttons.Add(nextButton);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (_pictureIsTaken)
            {
                e.Cancel = true;
                PictureIsToBeTakenScenario();
            }
        }

        private void back_Click(object sender, EventArgs e)
        {
            PictureIsToBeTakenScenario();
        }
    }
}