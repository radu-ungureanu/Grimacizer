using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Grimacizer7.Views.GeneralGameplay
{
    public partial class Adventure : NotifyPhoneApplicationPage
    {
        private DateTime _lastMomentOfDeath;
        private int _lastLevelPassed;
        private int _numberOfLives;
        public int NumberOfLives
        {
            get
            {
                return _numberOfLives;
            }
            set
            {
                if (_numberOfLives != value)
                {
                    _numberOfLives = value;
                    NotifyPropertyChanged(() => NumberOfLives);
                }
            }
        }
        private string _timeLeft;
        public string TimeLeft
        {
            get
            {
                return _timeLeft;
            }
            set
            {
                if (_timeLeft != value)
                {
                    _timeLeft = value;
                    NotifyPropertyChanged(() => TimeLeft);
                }
            }
        }
        public ObservableCollection<ButtonWrapper> Buttons { get; set; }
        DispatcherTimer livesRespanTimer = new DispatcherTimer();

        public Adventure()
        {
            InitializeComponent();
            Buttons = new ObservableCollection<ButtonWrapper>();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var lastLevel = db.Levels.OrderByDescending(t => t.Level).FirstOrDefault(t => t.Stars > 0);
                if (lastLevel == null)
                    _lastLevelPassed = 2;
                else
                    _lastLevelPassed = Math.Max(2, lastLevel.Level + 1);

                _lastMomentOfDeath = db.Settings.FirstOrDefault().LastMomentOfDeath;
                NumberOfLives = db.Settings.FirstOrDefault()._20_NumberLivesLeft;
            }
            InitializeButtons();

            if (NumberOfLives == Constants.MaxLives)
                return;
            if (NumberOfLives > Constants.MaxLives)
            {
                NumberOfLives = Constants.MaxLives;
                using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                {
                    db.Settings.FirstOrDefault()._20_NumberLivesLeft = Constants.MaxLives;
                    db.Settings.FirstOrDefault().LastMomentOfDeath = DateTime.Now;
                }
                return;
            }

            var difference = DateTime.Now - _lastMomentOfDeath;
            try
            {
                var mustBeWaitedSeconds = Constants.SecondsToWaitForLife * (Constants.MaxLives - NumberOfLives);
                if (difference.TotalSeconds >= mustBeWaitedSeconds)
                {
                    NumberOfLives = Constants.MaxLives;
                    using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                    {
                        db.Settings.FirstOrDefault()._20_NumberLivesLeft = Constants.MaxLives;
                        db.Settings.FirstOrDefault().LastMomentOfDeath = DateTime.Now;
                    }
                    return;
                }
            }
            catch (Exception)
            {
                using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                {
                    db.Settings.FirstOrDefault()._20_NumberLivesLeft = Constants.MaxLives;
                    db.Settings.FirstOrDefault().LastMomentOfDeath = DateTime.Now;
                }
                return;
            }

            var livesEarned = (int)Math.Floor(difference.TotalSeconds / Constants.SecondsToWaitForLife);
            NumberOfLives = Math.Min(NumberOfLives + livesEarned, Constants.MaxLives);
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                db.Settings.FirstOrDefault()._20_NumberLivesLeft = NumberOfLives;
                db.Settings.FirstOrDefault().LastMomentOfDeath = DateTime.Now;
            }

            if (NumberOfLives < Constants.MaxLives)
            {
                livesRespanTimer.Tick += new EventHandler(livesRespanTimer_Tick);
                livesRespanTimer.Interval = new TimeSpan(0, 0, 1);
                livesRespanTimer.Start();
            }
        }

        private void livesRespanTimer_Tick(object sender, EventArgs e)
        {
            if (NumberOfLives < Constants.MaxLives)
            {
                var difference = DateTime.Now - _lastMomentOfDeath;
                TimeSpan t = TimeSpan.FromSeconds(
                    Constants.SecondsToWaitForLife
                    - difference.Seconds
                    - difference.Minutes * 60
                    - difference.Hours * 3600
                    - difference.Days * 3600 * 24
                    );
                var display = string.Format("{0:D2}s", t.Seconds);
                TimeLeft = "Time left until next life: " + display;
            }
            else
            {
                NumberOfLives = Constants.MaxLives;
                livesRespanTimer.Stop();
                TimeLeft = string.Empty;
                using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
                {
                    db.Settings.FirstOrDefault()._20_NumberLivesLeft = Constants.MaxLives;
                    db.Settings.FirstOrDefault().LastMomentOfDeath = DateTime.Now;
                }
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri(Pages.MainPage, UriKind.RelativeOrAbsolute));
        }

        private void InitializeButtons()
        {
            Buttons.Clear();
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                for (int i = 1; i <= Constants.AvailableLevels; i++)
                {
                    var level = db.Levels.FirstOrDefault(t => t.Level == i);
                    Buttons.Add(new ButtonWrapper
                    {
                        Content = i.ToString(),
                        IsEnabled = i <= _lastLevelPassed ? true : false,
                        Stars = level.Stars
                    });
                }
            }
        }

        private void PlayLevel_Click(object sender, RoutedEventArgs e)
        {
            if (NumberOfLives <= 0)
            {
                MessageBox.Show("Sorry, you have no lives left! Wait until a new one is generated");
                return;
            }

            var level = (sender as Button).DataContext as ButtonWrapper;
            livesRespanTimer.Stop();

            var path = string.Format("{0}?level={1}", Pages.AdventureLevels_ShowPicture, level.Content);
            NavigationService.Navigate(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            livesRespanTimer.Stop();
            livesRespanTimer.Tick -= livesRespanTimer_Tick;
            base.OnNavigatedFrom(e);
        }
    }

    public class ButtonWrapper
    {
        public string Content { get; set; }
        public bool IsEnabled { get; set; }
        public int Stars { get; set; }
    }
}