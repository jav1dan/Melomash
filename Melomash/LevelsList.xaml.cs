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
using Barbadoz.Melomany.Engine;
using Barbadoz.Melomany.Engine.Json;
namespace Melomash
{
    public partial class LevelsList : PhoneApplicationPage
    {
        public LevelsList()
        {
            InitializeComponent();
        }
        Core core = new Core();
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn;
            levels_list.Items.Clear();
            foreach (LevelBase lvb in core.lvls.levels)
            {
                btn = new Button();
                btn.Content = lvb.name;
                btn.Width = 480;
                btn.Height = 150;
                btn.Tag = lvb.ident;
                btn.Click += (sender1, e1) =>
                  {
                      Button btn1;
                      btn1 = (Button)sender1;
                      core.level_ident = (string)btn1.Tag;
                      if(!core.level_is_fully_guessed())
                      {
                          NavigationService.Navigate(new Uri("/GamePage.xaml?level_ident=" + core.level_ident, UriKind.Relative));
                      }
                  };
                if (lvb.ident != "null")
                {
                    levels_list.Items.Add(btn);
                }
            }
        }


        private void add_level_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LevelStore.xaml", UriKind.Relative));
        }
    }
}