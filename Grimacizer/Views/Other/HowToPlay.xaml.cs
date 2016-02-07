using Grimacizer7.Common;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;

namespace Grimacizer7.Views.Other
{
    public partial class HowToPlay : NotifyPhoneApplicationPage
    {
        public HowToPlay()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;

            SystemTray.IsVisible = true;
        }
    }
}