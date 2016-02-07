using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.DAL.Tables;
using Grimacizer7.Utils;
using Microsoft.Devices;
using Microsoft.FaceSdk.ImageHelper;
using Microsoft.Phone.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Grimacizer7.Views.AdventureLevels
{
    public partial class TakePhoto : NotifyPhoneApplicationPage
    {
        private double _cameraHeight, _cameraWidth;
        private double _phoneHeight, _phoneWidth;

        private BackgroundWorker worker;
        private SdkHelper FaceSdkHelper;
        private WriteableBitmap imageBitmap;
        private PhotoCamera cam;
        private DispatcherTimer counter = new DispatcherTimer();
        private DateTime startMoment;

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (_Message != value)
                {
                    _Message = value;
                    NotifyPropertyChanged(() => Message);
                }
            }
        }
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
        private int _level;
        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    NotifyPropertyChanged(() => Level);
                }
            }
        }
        private string _quote;
        public string Quote
        {
            get
            {
                return _quote;
            }
            set
            {
                if (_quote != value)
                {
                    _quote = value;
                    NotifyPropertyChanged(() => Quote);
                }
            }
        }
        private string _timer;
        public string Timer
        {
            get
            {
                return _timer;
            }
            set
            {
                if (_timer != value)
                {
                    _timer = value;
                    NotifyPropertyChanged(() => Timer);
                }
            }
        }

        public TakePhoto()
        {
            InitializeComponent();
            InitScene();
            FaceSdkHelper = new SdkHelper();
            DataContext = this;

            Message = string.Empty;
            IsCalculating = false;
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
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Message = "A Camera is not available on this device.";
                });
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Level = Convert.ToInt32(NavigationContext.QueryString["level"]);

            _phoneHeight = Application.Current.Host.Content.ActualHeight;
            _phoneWidth = Application.Current.Host.Content.ActualWidth;
            OnOrientationChanged(new OrientationChangedEventArgs(Orientation));

            startMoment = DateTime.Now;
            counter.Tick += counter_Tick;
            counter.Interval = new TimeSpan(0, 0, 1);
            counter.Start();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                cam.Dispose();
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
            }
        }

        private void cam_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var bmp = new BitmapImage();
                bmp.SetSource(e.ImageStream);
                var writeableBmp = BitmapFactory.New(bmp.PixelWidth, bmp.PixelHeight).FromStream(e.ImageStream);
                imageBitmap = WriteableBitmapHelpers.TransformBitmapByCameraTypeAndPageOrientation(
                    writeableBmp, cam.CameraType, Orientation);

                var randomFileName = string.Format("{0}.jpg", Guid.NewGuid().ToString());
                LocalImagesHelper.WriteImageToIsolatedStorage(randomFileName, imageBitmap);
                using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                {
                    db.SavedPictures.InsertOnSubmit(new PictureItem
                    {
                        Name = randomFileName,
                        Width = imageBitmap.PixelWidth,
                        Height = imageBitmap.PixelHeight
                    });
                    db.SubmitChanges();
                }
                e.ImageStream.Close();
            });
        }

        private void counter_Tick(object sender, EventArgs e)
        {
            var availbleSeconds = 6 - (DateTime.Now - startMoment).Seconds;
            Timer = availbleSeconds.ToString();

            if (availbleSeconds <= 0)
            {
                counter.Stop();

                Dispatcher.BeginInvoke(async () =>
                    {
                        IsCalculating = true;
                        Timer = string.Empty;
                        Quote = Quotes.GetRandomQuote();

                        if (cam != null)
                        {
                            try
                            {
                                await cam.CaptureImageAsync();
                                await startHardWork();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Can't calculate at this time! Sorry, try again!");
                                NavigationService.Navigate(new Uri(Pages.GeneralGameplay_Adventure, UriKind.RelativeOrAbsolute));
                            }
                        }
                    });
            }
        }

        private async Task startHardWork()
        {
            worker = new BackgroundWorker();
            worker.DoWork += (sender, arg) =>
            {
                FaceSdkHelper.Initialize();
            };
            await worker.BeginWorkerAsync();

            var sdkImg = ImageConverter.SystemToSdk(imageBitmap);
            var result = FaceSdkHelper.Detect(sdkImg);

            if (SdkHelper.foundFace == true && SdkHelper.results.Count == 1)
            {
                var shape = SdkHelper.results.FirstOrDefault();

                var defaultFaceCalculations = Calculus.ReadDefaultFaceCalculations();
                var array = FaceCalculationsHelpers.FromFaceCalculationsToVector(defaultFaceCalculations);

                var calculatedFaceCalculations = Calculus.GetFaceCalculationsFromShape(shape);
                var resultArray = FaceCalculationsHelpers.FromFaceCalculationsToVector(calculatedFaceCalculations);

                var conditions = new Conditions();
                var resultFromCalculus = conditions.GetLevelVerifierFunction(Level).Invoke(array, resultArray);

                if (resultFromCalculus != 0)
                {
                    using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                    {
                        var passedLevels = db.Settings.FirstOrDefault()._16_PassedLevels;
                        db.Settings.FirstOrDefault()._16_PassedLevels = Math.Max(passedLevels, Level);

                        var level = db.Levels.FirstOrDefault(t => t.Level == Level);
                        level.Stars = Math.Max(resultFromCalculus, level.Stars);

                        db.SubmitChanges();
                    }
                    MessageBox.Show("Level passed. You won " + resultFromCalculus + " point" + (resultFromCalculus > 1 ? "s!" : "!"));
                }
                else
                {
                    InGameLifeHelpers.LoseLife();
                    MessageBox.Show("Level failed");
                }
            }
            else
            {
                MessageBox.Show("Camera did not detect your face! Try again!");
            }

            NavigationService.Navigate(new Uri(Pages.GeneralGameplay_Adventure, UriKind.RelativeOrAbsolute));
            FaceSdkHelper = null;
            GC.Collect();
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            InGameLifeHelpers.LoseLife();
            e.Cancel = true;
            NavigationService.Navigate(new Uri(Pages.GeneralGameplay_Adventure, UriKind.RelativeOrAbsolute));
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                viewfinderBrush.RelativeTransform = CameraHelpers.GetCameraTransformByOrientation(cam.CameraType, Orientation);
            }
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

            base.OnOrientationChanged(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            counter.Stop();
            counter.Tick -= counter_Tick;
            FaceSdkHelper = null;
            GC.Collect();
            base.OnNavigatedFrom(e);
        }
    }
}