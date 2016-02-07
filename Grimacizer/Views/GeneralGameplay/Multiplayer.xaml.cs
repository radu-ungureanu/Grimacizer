using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Grimacizer7.Common;

namespace Grimacizer7.Views.GeneralGameplay
{
    public partial class Multiplayer : NotifyPhoneApplicationPage
    {
        public Multiplayer()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;
        }
    }
}