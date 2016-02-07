using Grimacizer7.Common;
using Grimacizer7.Utils;
using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Grimacizer7.Views.AdventureLevels
{
    public partial class ShowPicture : NotifyPhoneApplicationPage
    {
        private DispatcherTimer counter = new DispatcherTimer();
        private DateTime startMoment;

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
        private int _timer;
        public int Timer
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
        private string _condition;
        public string Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                if (_condition != value)
                {
                    _condition = value;
                    NotifyPropertyChanged(() => Condition);
                }
            }
        }
        private BitmapImage _image;
        public BitmapImage Image
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

        public ShowPicture()
        {
            InitializeComponent();
            DataContext = this;
            Timer = 3;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            Level = Convert.ToInt32(NavigationContext.QueryString["level"]);

            Image = new BitmapImage();
            Image.UriSource = new Uri("/Assets/Levels/" + Level + ".png", UriKind.Relative);

            var conditions = new Conditions();
            Condition = conditions.GetConditionByLevel(Level);

            startMoment = DateTime.Now;
            counter.Tick += new EventHandler(counter_Tick);
            counter.Interval = new TimeSpan(0, 0, 1);
            counter.Start();
        }

        private void counter_Tick(object sender, EventArgs e)
        {
            var availableSeconds = Constants.AvailableSecondsForAdventureLevelPreview - (DateTime.Now - startMoment).Seconds;
            Timer = availableSeconds;
            if (availableSeconds <= 0)
            {
                counter.Stop();
                var path = string.Format("{0}?level={1}", Pages.AdventureLevels_TakePhoto, Level);
                NavigationService.Navigate(new Uri(path, UriKind.RelativeOrAbsolute));
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            InGameLifeHelpers.LoseLife();
            NavigationService.GoBack();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            counter.Stop();
            counter.Tick -= counter_Tick;
            base.OnNavigatedFrom(e);
        }
    }
}