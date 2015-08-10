using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Melomash.Resources;
using System.IO.IsolatedStorage;
using System.IO;
using Newtonsoft.Json;
using Barbadoz.Melomany.Engine;
namespace Melomash
{ 
    public partial class LevelItem : UserControl
    {
        Core core = new Core();
        public LevelItem()
        {
            InitializeComponent();
        }
        public string LevelName
        {
            get
            {
                return level_name.Text;
            }
            set
            {
                level_name.Text = value;
            }
        }
        public string level_ident { get; set; }
        private void btnDelete_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult ms = MessageBox.Show(AppResources.ConfirmToDelete, AppResources.ApplicationTitle, MessageBoxButton.OKCancel);
            if(ms==MessageBoxResult.OK)
            {
                core.remove_level(level_ident);
            }
            else
            {
                return;
            }
        }
    }
}
