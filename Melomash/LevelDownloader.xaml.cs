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
using Newtonsoft.Json;
using System.IO.IsolatedStorage;
using Barbadoz.Melomany.Engine;

namespace Melomash
{
    public partial class LevelDownloader : PhoneApplicationPage
    {
        Core core = new Core();
        coins cash = new coins();
        string[] links;
        string[] localFileNames;
        string level_ident;
        bool cancel_operation = false;
        public LevelDownloader()
        {
            InitializeComponent();
        }

        private  void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            operation_declare.Text = String.Format(AppResources.DownloadOperationDeclare, NavigationContext.QueryString["name"]);
            WebClient client = new WebClient();
            Level json_tmp_level;
            int artist_count;
            int files_count;
            string localFileName;
            string base_dir;
            level_ident = NavigationContext.QueryString["ident"];
            bool[] downloadedFiles;
            int iter = 0;
            base_dir = String.Format("http://meloman.javidan.ru/levels/{0}/", level_ident);
            Uri uri = new Uri(String.Format(base_dir+"declare.txt", NavigationContext.QueryString["ident"]), UriKind.Absolute);
            client.DownloadStringCompleted += (sender_1, e_1) =>
              {
                  json_tmp_level = JsonConvert.DeserializeObject<Level>(e_1.Result);
                  artist_count = json_tmp_level.artists.Count;
                  files_count = artist_count * Convert.ToInt32(json_tmp_level.tracks_count);
                  files_count += 1;
                  links = new string[files_count];
                  localFileNames = new string[files_count];
                  downloadedFiles = new bool[files_count];
                  links[0] = base_dir + "declare.txt";
                  downloadedFiles[0] = false;
                  localFileNames[0] = "/"+level_ident+"/declare.txt";
                  int counter;
                  counter = 1;
                  for(int j=1;j<=artist_count;j++)
                  {
                      for (int i = 1; i <= Convert.ToInt32(json_tmp_level.tracks_count); i++)
                      {
                          links[counter] = base_dir + String.Format("{0}_{1}.mp3", Convert.ToString(j), Convert.ToString(i));
                          localFileNames[counter] = String.Format("/{0}/{1}_{2}.mp3", level_ident, Convert.ToString(j), Convert.ToString(i));
                          downloadedFiles[counter] = false;
                          counter++;
                      }
                  }
                  MessageBoxResult mx = MessageBox.Show(String.Format(AppResources.ConfirmToDownload, Convert.ToString(files_count * 128)), String.Format(AppResources.DownloadOperationDeclare, NavigationContext.QueryString["name"]), MessageBoxButton.OKCancel);
                  if(mx==MessageBoxResult.Cancel)
                  {
                      NavigationService.Navigate(new Uri("/LevelStore.xaml", UriKind.Relative));
                  }
                  else
                  {
                      //Начать скачивание
                      //Для начала проверим есть ли папка в IsolatedStorage?
                      //Если нет - создадим!
                      IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                      if(!fileStorage.DirectoryExists(level_ident))
                      {
                          fileStorage.CreateDirectory(level_ident);
                      }
                      WebClient client2 = new WebClient();
                      localFileName = "";
                      int progress_iterator;
                      progress_iterator = (int)files_count/100;
                      progress_iterator++;
                      client2.OpenReadCompleted += (sender_2, e_2) =>
                        {
                            operation_title.Text = AppResources.IsLoading;
                            operation_declare.Text = String.Format(AppResources.DownloadFinish, localFileName);
                            if (fileStorage.FileExists(localFileName))
                            {
                                fileStorage.DeleteFile(localFileName);
                            }
                            using (var fileStream = fileStorage.CreateFile(localFileName))
                            {
                                e_2.Result.CopyTo(fileStream);
                                fileStream.Close();
                                downloadedFiles[iter] = true;
                                Progress.Value += progress_iterator;
                                iter++;
                            }
                            if (iter >= files_count)
                            {
                                btnDownload.Content = AppResources.NavigateToLevelsList;
                                operation_title.Text = AppResources.LoadingComplete;
                                cash.count -= Convert.ToInt32(NavigationContext.QueryString["coins"]);
                                //cash.count -= Convert.ToInt32(NavigationContext.QueryString["coins"]);
                                //core.addLevel(json_tmp_level);
                                core.add_level(level_ident, json_tmp_level.name, Convert.ToString(artist_count));
                                return;
                            }
                            if(cancel_operation==true)
                            {
                                return;
                            }
                            localFileName = localFileNames[iter];
                            client2.OpenReadAsync(new Uri(links[iter], UriKind.Absolute));
                        };
                      iter = 0;
                      localFileName = localFileNames[iter];
                      client2.OpenReadAsync(new Uri(links[iter], UriKind.Absolute));
                  }
              };
            client.DownloadStringAsync(uri);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            if((string)btnDownload.Content==AppResources.CancelOperation)
            {
                cancel_operation = true;
            }
            else
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}