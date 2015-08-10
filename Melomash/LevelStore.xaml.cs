 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Melomash.Resources;
using Barbadoz.Melomany.Engine;
namespace Melomash
{
    public partial class LevelStore : PhoneApplicationPage
    {
        string url;
        get_levels_in_category j_1;
        int tracks_count;
        Core core = new Core();
        coins cash = new coins();
        public LevelStore()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //задаем переменные
            WebClient client = new WebClient();
            Button bt;
            url = "http://meloman.javidan.ru/action.php?method={0}&device=wp&category={1}";
            json_input j_i = new json_input();
            string temp;
            //Запускаем base
            base.OnNavigatedTo(e);
            //Показываем слой "загрузки"
            loader.Visibility = Visibility.Visible;
            ProgressString.Text = AppResources.ServerConnect;

            //Задаем uri
            Uri uri = new Uri(String.Format(url, "showcats","default"), UriKind.Absolute);
            //Задаем события для WebClient
            //Задаем событие для изменения ПрогрессБара
            client.DownloadProgressChanged += (sender, e_3) =>
              {
                  Progress_Show.Value = e_3.ProgressPercentage;
              };
            //Задаем событие для парсинга JSON
            client.DownloadStringCompleted += (sender, e_2) =>
            {
                try
                {
                    ProgressString.Text = AppResources.Loading;
                    temp = e_2.Result;
                    j_i = JsonConvert.DeserializeObject<json_input>(temp);
                    groups.Visibility = Visibility.Visible;
                    categorys.Items.Clear();
                    foreach (category cat in j_i.list)
                    {
                        bt = new Button();
                        bt.Content = cat.rutext;
                        bt.Tag = cat.name;
                        bt.Click += category_Click;
                        categorys.Items.Add(bt);
                    }
                    loader.Visibility = Visibility.Collapsed;
                    groups.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    MessageBox.Show(AppResources.InternetIsNotWorking);
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            };
            //Асинхронно запускаем загрузку строки
            client.DownloadStringAsync(uri);
        }

        private void category_Click(object sender, RoutedEventArgs e)
        {
            //задаем переменные
            Button bt;
            string temp;
            string category_name;
            j_1 = new get_levels_in_category();
            Button btn = (Button)sender;
            category_name = (string)btn.Tag;
            groups.Visibility = Visibility.Collapsed;
            WebClient client = new WebClient();
            loader.Visibility = Visibility.Visible;            
            Uri uri = new Uri(String.Format(url, "show_category", category_name));
            client.DownloadProgressChanged += (sender2, e_1) =>
              {
                  Progress_Show.Value = e_1.ProgressPercentage;
              };
            client.DownloadStringCompleted += (sender2, e_2) =>
              {
                  try
                  {
                      ProgressString.Text = AppResources.Loading;
                      temp = e_2.Result;
                      j_1 = JsonConvert.DeserializeObject<get_levels_in_category>(temp);
                      levels_list.Items.Clear();
                      foreach (Server_Level lvl in j_1.list)
                      {
                          if(!core.level_exists(lvl.ident))
                          {
                              bt = new Button();
                              bt.Content = lvl.name;
                              bt.Tag = lvl.ident;
                              bt.Click += Bt_Click;
                              levels_list.Items.Add(bt);
                          }
                      }
                      levels.Visibility = Visibility.Visible;
                      loader.Visibility = Visibility.Collapsed;
                  }
                  catch(Exception)
                  {
                      MessageBox.Show(AppResources.InternetIsNotWorking);
                      NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                  }                                   
              };
            client.DownloadStringAsync(uri);
        }

        private void Bt_Click(object sender, RoutedEventArgs e)
        {
            //Загрузка информации об уровне
            string level_ident;
            Button bt;
            bt = (Button)sender;
            level_ident = (string)bt.Tag;
            foreach(Server_Level sx in j_1.list)
            {
                if(sx.ident==level_ident)
                {
                    level.Visibility = Visibility.Visible;
                    levels.Visibility = Visibility.Collapsed;
                    server_level_coast.Text = sx.coast;
                    server_level_desc.Text = sx.desc;
                    server_level_name.Text = sx.name;
                    buy_level.Tag = level_ident;
                    tracks_count = Convert.ToInt32(sx.tracks_count);
                    switch(sx.stars)
                    {
                        case "1":
                            star1.Source = preloaded_star.Source;
                            star2.Source = preloaded_star_bg.Source;
                            star3.Source = preloaded_star_bg.Source;
                            break;
                        case "2":
                            star1.Source = preloaded_star.Source;
                            star2.Source = preloaded_star.Source;
                            star3.Source = preloaded_star_bg.Source;
                            break;
                        case "3":
                            star1.Source = preloaded_star.Source;
                            star2.Source = preloaded_star.Source;
                            star3.Source = preloaded_star.Source;
                            break;
                        default:
                            star1.Source = preloaded_star_bg.Source;
                            star2.Source = preloaded_star_bg.Source;
                            star3.Source = preloaded_star_bg.Source;
                            break;
                    }
                }
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(loader.Visibility!=Visibility.Visible)
            {
                if(groups.Visibility==Visibility.Visible)
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
                if(levels.Visibility==Visibility.Visible)
                {
                    groups.Visibility = Visibility.Visible;
                    levels.Visibility = Visibility.Collapsed;
                    e.Cancel = true;
                }
                if(level.Visibility==Visibility.Visible)
                {
                    levels.Visibility = Visibility.Visible;
                    level.Visibility = Visibility.Collapsed;
                    e.Cancel = true;
                }
            }
            else
            {
                MessageBox.Show(AppResources.ImposToAbort);
            }
        }

        private void buy_level_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mx=MessageBox.Show(String.Format(AppResources.ConfirmBuyMessage, server_level_name.Text, server_level_coast.Text), AppResources.ConfirmBuyTitle, MessageBoxButton.OKCancel);
            if(mx==MessageBoxResult.OK)
            {
                if(cash.count<Convert.ToInt32(server_level_coast.Text))
                {
                    MessageBox.Show(AppResources.NotEnoughCoins);
                }
                else
                {
                    if (core.level_exists((string)buy_level.Tag))
                    {
                        MessageBox.Show(AppResources.LevelExists);
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri(String.Format("/LevelDownloader.xaml?ident={0}&tracks_count={1}&name={2}&coins={3}", (string)buy_level.Tag, Convert.ToString(tracks_count), server_level_name.Text,server_level_coast.Text), UriKind.Relative));
                    }
                }
            }
        }
    }
}