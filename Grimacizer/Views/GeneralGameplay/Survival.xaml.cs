using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using Microsoft.Devices;
using Microsoft.FaceSdk.ImageHelper;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Grimacizer7.Views.GeneralGameplay
{
    public partial class Survival : NotifyPhoneApplicationPage
    {
        private double _cameraHeight, _cameraWidth;
        private double _phoneHeight, _phoneWidth;
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

        private SdkHelper FaceSdkHelper;
        private int savedCounter = 1;
        PhotoCamera cam;
        BitmapImage bmi;
        List<int> selectedPhotos = new List<int>();
        DispatcherTimer timer = new DispatcherTimer();
        DateTime startDate = new DateTime();
        private WriteableBitmap bitmap;

        public Survival()
        {
            InitializeComponent();
            FaceSdkHelper = new SdkHelper();
            DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;
            _phoneHeight = Application.Current.Host.Content.ActualHeight;
            _phoneWidth = Application.Current.Host.Content.ActualWidth;

            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                {
                    cam = new PhotoCamera(CameraType.FrontFacing);
                }
                else
                {
                    cam = new PhotoCamera(CameraType.Primary);
                }

                _cameraHeight = cam.AvailableResolutions.FirstOrDefault().Height;
                _cameraWidth = cam.AvailableResolutions.FirstOrDefault().Width;

                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);
                cam.CaptureImageAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureImageAvailable);
                viewfinderBrush.SetSource(cam);
                OnOrientationChanged(new OrientationChangedEventArgs(Orientation));

                bmi = new BitmapImage(new Uri("/Images/Levels/L2.JPG", UriKind.Relative));
                this.grimacePattern.Source = bmi;

                startDate = DateTime.Now;

                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
            else
            {
                Message = "A Camera is not available on this phone.";
            }
        }
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                cam.Dispose();
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
            }
        }

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            // Increments the savedCounter variable used for generating JPEG file names.
            savedCounter++;
        }

        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            string fileName = savedCounter + ".jpg";
            try
            {
                e.ImageStream.Seek(0, SeekOrigin.Begin);

                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var targetStream = isoStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the image to the local folder. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }
            }
            finally
            {
                e.ImageStream.Close();
            }

        }

        async void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                var availableSeconds = (DateTime.Now - this.startDate).Seconds;
                Timer = availableSeconds.ToString();

                if (availableSeconds % 5 == 0)
                {
                    Dispatcher.BeginInvoke(async delegate()
                    {
                        if (cam != null)
                        {
                            await cam.CaptureImageAsync();
                        }
                    });

                    var rnd = new Random();
                    int randomLevel = rnd.Next(0, Constants.AvailableLevels + 1);

                    while (selectedPhotos.Contains(randomLevel))
                    {
                        randomLevel = rnd.Next(1, Constants.AvailableLevels + 1);
                    }
                    selectedPhotos.Add(randomLevel);

                    string pic = "/Images/Levels/L" + randomLevel + ".JPG";
                    bmi = new BitmapImage(new Uri(pic, UriKind.Relative));
                    grimacePattern.Source = bmi;
                }

                if (availableSeconds > 19)
                {
                    timer.Stop();

                    IsCalculating = true;
                    Timer = string.Empty;
                    Message = string.Empty;

                    var worker = new BackgroundWorker();
                    DoWorkEventHandler workerHandler = null;

                    this.FaceSdkHelper.Initialize();

                    for (int i = 1; i < 5; i++)
                    {
                        workerHandler = (a, b) =>
                        {
                            startHardWork(i);
                        };
                        worker.DoWork -= workerHandler;
                        worker.DoWork += workerHandler;
                        await worker.BeginWorkerAsync();
                    }

                    IsCalculating = false;

                    NavigationService.Navigate(new Uri(Pages.MainPage, UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something bad appeared! Going back..");
                NavigationService.Navigate(new Uri(Pages.MainPage, UriKind.RelativeOrAbsolute));
            }
        }

        void startHardWork(int currentPhoto)
        {
            try
            {
                Dispatcher.BeginInvoke(delegate()
                {
                    var source = new BitmapImage();

                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var fileStream = isoStore.OpenFile(currentPhoto + ".jpg", FileMode.Open, FileAccess.Read))
                        {
                            source.SetSource(fileStream);
                        }
                    }

                    this.bitmap = new WriteableBitmap(source);

                    var sdkImg = ImageConverter.SystemToSdk(this.bitmap);
                    var result = this.FaceSdkHelper.Detect(sdkImg);

                    if (SdkHelper.foundFace == true)
                    {
                        foreach (var shape in SdkHelper.results)
                        {
                            var defaultFaceCalculations = Calculus.ReadDefaultFaceCalculations();
                            var array = FaceCalculationsHelpers.FromFaceCalculationsToVector(defaultFaceCalculations);

                            var calculatedFaceCalculations = Calculus.GetFaceCalculationsFromShape(shape);
                            var resultArray = FaceCalculationsHelpers.FromFaceCalculationsToVector(calculatedFaceCalculations);

                            Conditions c = new Conditions();
                            var resultFromCalculus = c.GetLevelVerifierFunction(selectedPhotos[currentPhoto]).Invoke(array, resultArray);
                            if (resultFromCalculus != 0)
                            {
                                using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                                {
                                    db.Settings.FirstOrDefault()._17_SurvivalScore += resultFromCalculus;
                                    db.SubmitChanges();
                                }
                                MessageBox.Show("Picture number " + currentPhoto + " correct! You won " + resultFromCalculus + " points!");
                            }
                            else
                            {
                                MessageBox.Show("Picture number " + currentPhoto + " did not match!");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Picture number " + currentPhoto + "! Camera did not detect your face!");
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something bad appeared! Going back..");
                NavigationService.Navigate(new Uri(Pages.MainPage, UriKind.RelativeOrAbsolute));
            }
            finally
            {
                GC.Collect();
            }
        }

        protected override void OnOrientationChanged(Microsoft.Phone.Controls.OrientationChangedEventArgs e)
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
    }
}