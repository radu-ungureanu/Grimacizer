using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.DAL.Tables;
using Grimacizer7.Utils;
using Microsoft.Phone.Shell;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace Grimacizer7.Views.CreateProfile
{
    public partial class CreateProfile : NotifyPhoneApplicationPage
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

        public CreateProfile()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;
            FacebookClientHelpers.SaveToken(null);
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if (PlayerName.IsNullOrEmptyOrWhiteSpace())
            {
                MessageBox.Show("Please insert a name");
                return;
            }

            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var settings = db.Settings.FirstOrDefault();
                settings.PlayerName = PlayerName;
                settings.Sex = GetSexType();
                settings.Race = GetHumanRaceType();
                db.SubmitChanges();                
            }
            NavigationService.Navigate(new Uri(Pages.CreateProfile_TakePhotoDefault, UriKind.Relative));
        }

        private SexType GetSexType()
        {
            if (male.IsChecked.Value)
                return SexType.Male;
            return SexType.Female;
        }

        private HumanRaceType GetHumanRaceType()
        {
            if (caucasian.IsChecked.Value)
                return HumanRaceType.Caucasian;
            if (african.IsChecked.Value)
                return HumanRaceType.African;
            return HumanRaceType.Asian;
        }
    }
}