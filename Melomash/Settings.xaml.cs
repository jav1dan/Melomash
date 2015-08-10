using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Barbadoz.Melomany.Engine;
namespace Melomash
{
    public partial class Settings : PhoneApplicationPage
    {
        settings_core s_core = new settings_core();
        public Settings()
        {
            InitializeComponent();
            sounds_checkbox.IsChecked = s_core.sfx_enabled();
        }

        private void sounds_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            s_core.set_sfx_enabled();
        }

        private void sounds_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            s_core.set_sfx_disabled();
        }
    }
}