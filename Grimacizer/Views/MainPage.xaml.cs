using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Grimacizer7.Views
{
    public partial class MainPage : NotifyPhoneApplicationPage
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
        private WriteableBitmap _cartoon;
        public WriteableBitmap Cartoon
        {
            get
            {
                return _cartoon;
            }
            set
            {
                if (_cartoon != value)
                {
                    _cartoon = value;
                    NotifyPropertyChanged(() => Cartoon);
                }
            }
        }
        private int _adventure;
        public int Adventure
        {
            get
            {
                return _adventure;
            }
            set
            {
                if (_adventure != value)
                {
                    _adventure = value;
                    NotifyPropertyChanged(() => Adventure);
                }
            }
        }
        private int _survival;
        public int Survival
        {
            get
            {
                return _survival;
            }
            set
            {
                if (_survival != value)
                {
                    _survival = value;
                    NotifyPropertyChanged(() => Survival);
                }
            }
        }
        private int _multiplayerWin;
        public int MultiplayerWin
        {
            get
            {
                return _multiplayerWin;
            }
            set
            {
                if (_multiplayerWin != value)
                {
                    _multiplayerWin = value;
                    NotifyPropertyChanged(() => MultiplayerWin);
                }
            }
        }
        private int _multiplayerLose;
        public int MultiplayerLose
        {
            get
            {
                return _multiplayerLose;
            }
            set
            {
                if (_multiplayerLose != value)
                {
                    _multiplayerLose = value;
                    NotifyPropertyChanged(() => MultiplayerLose);
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = true;

            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();
                PlayerName = settings.PlayerName;

                Adventure = Constants.AvailableLevels - settings._16_PassedLevels;
                Survival = settings._17_SurvivalScore;
                MultiplayerWin = settings._18_MultiplayerWinScore;
                MultiplayerLose = settings._19_MultiplayerLoseScore;
            }

            Cartoon = LocalImagesHelper.ReadImageFromIsolatedStorage(Constants.DEFAULT_CARTOON_PHOTO, Constants.CartoonWidth, Constants.CartoonHeight);
        }

        private void HowToPlay_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(Pages.Other_HowToPlay, UriKind.RelativeOrAbsolute));
        }

        private void Ranks_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(Pages.Other_Ranks, UriKind.RelativeOrAbsolute));
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Pages.Other_EditProfile, UriKind.RelativeOrAbsolute));
        }

        private void AdventurePlay_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Pages.GeneralGameplay_Adventure, UriKind.RelativeOrAbsolute));
        }

        private void SurvivalPlay_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Pages.GeneralGameplay_Survival, UriKind.RelativeOrAbsolute));
        }

        private void MultiplayerPlay_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Pages.GeneralGameplay_Multiplayer, UriKind.RelativeOrAbsolute));
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(Pages.PictureHistory, UriKind.Relative));
        }

        private void Profile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            profileContext.IsOpen = !profileContext.IsOpen;
        }        
    }
}