using Grimacizer7.Common;
using Grimacizer7.DAL;
using Grimacizer7.DAL.Tables;
using Grimacizer7.Utils;
using Microsoft.Phone.Controls;
using System;
using System.Linq;
using System.Windows.Navigation;

namespace Grimacizer7.Views
{
    public partial class Intro : PhoneApplicationPage
    {
        public Intro()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            await Extensions.WaitAsync(3000);

            var settingsExists = false;
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                var count = db.Settings.Count();
                if (count == 0)
                {
                    db.Settings.InsertOnSubmit(new SettingsItem 
                    { 
                        Initialized = false, 
                        LastMomentOfDeath = DateTime.Now
                    });
                    db.FaceCalculations.InsertOnSubmit(new FaceCalculationsItem());
                    for (int i = 1; i <= Constants.AvailableLevels; i++)
                    {
                        db.Levels.InsertOnSubmit(new LevelItem { Level = i, Stars = 0 });
                    }
                    db.SubmitChanges();
                }
                var settings = db.Settings.FirstOrDefault();
                settingsExists = settings.Initialized;
            }

            if (settingsExists)
            {
                NavigationService.Navigate(new Uri(Pages.MainPage, UriKind.RelativeOrAbsolute));
            }
            else
            {
                NavigationService.Navigate(new Uri(Pages.CreateProfile_CreateProfile, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
