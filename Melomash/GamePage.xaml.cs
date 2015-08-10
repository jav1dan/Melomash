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
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using Melomash.Resources;
using System.Windows.Media;

namespace Melomash
{
    public partial class GamePage : PhoneApplicationPage
    {
        sound_effects snd = new sound_effects();
        Core core = new Core();
        settings_core s_core = new settings_core();
        public Artist tmp_art;
        public string wrd;
        public int art_index;
        public int artist_index;
        public int words_count;
        bool started_to_type;
        coins cash = new coins();
        public Char[] char_razd = { ' ', '.' };
        public GamePage()
        {
            InitializeComponent();
            cShowCorrectAnswer.Text = Convert.ToString(cash.hint_show_correct_answer);
            cRemoveExtraSymb.Text = Convert.ToString(cash.hint_remove_extra_symbols);
            cShowFirstSymb.Text = Convert.ToString(cash.hint_show_first_symbol);
            cUnlockThirdTrack.Text = Convert.ToString(cash.hint_unlock_third_track);
        }
        bool playing;
        int playing_index;
        int move_order = 0;
        int[] moves;
        string[] split;
        private int key_played;
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string level_ident;
            level_ident = NavigationContext.QueryString["level_ident"];
            core.level_ident = level_ident;
            core.LoadByIdent();
            level_name.Text = core.main_level.name;
            coins cash = new coins();
            coins_count.Text = Convert.ToString(cash.count);
            if(core.get_tracks_count()==1)
            {
                song2.Visibility = Visibility.Collapsed;
                song3.Visibility = Visibility.Collapsed;
                btnHintUnlockTrack.Visibility = Visibility.Collapsed;
            }
            preload();
            load_artist(core.sorted[0]);
        }
        public void preload()
        {
            core.randomize_list();            
            load_artist(0);
        }
        public void load_artist(int artist_indexer)
        {
            started_to_type = false;
            tmp_art = core.sorted[artist_indexer];
            artist_index = artist_indexer;
            b1.Visibility = Visibility.Visible;
            b2.Visibility = Visibility.Visible;
            b3.Visibility = Visibility.Visible;
            b4.Visibility = Visibility.Visible;
            b5.Visibility = Visibility.Visible;
            b6.Visibility = Visibility.Visible;
            b7.Visibility = Visibility.Visible;
            b8.Visibility = Visibility.Visible;
            b9.Visibility = Visibility.Visible;
            b10.Visibility = Visibility.Visible;
            b11.Visibility = Visibility.Visible;
            b12.Visibility = Visibility.Visible;
            b13.Visibility = Visibility.Visible;
            b14.Visibility = Visibility.Visible;
            b15.Visibility = Visibility.Visible;
            word1.Visibility = Visibility.Visible;
            word2.Visibility = Visibility.Visible;
            word3.Visibility = Visibility.Visible;
            if (tmp_art.song1.Length > 0)
            {
                song1.Visibility = Visibility.Visible;
            }
            else
            {
                song1.Visibility = Visibility.Collapsed;
            }
            if (tmp_art.song2.Length > 0)
            {
                song2.Visibility = Visibility.Visible;
            }
            else
            {
                song2.Visibility = Visibility.Collapsed;
            }
            if (tmp_art.song3.Length > 0)
            {
                song3.Visibility = Visibility.Visible;
            }
            else
            {
                song3.Visibility = Visibility.Collapsed;
            }
            timing.Begin();
            switch (core.main_level.tracks_count)
            {
                case "1":
                    play1.SetValue(Grid.ColumnProperty, 2);
                    play2.Visibility = Visibility.Collapsed;
                    play3.Visibility = Visibility.Collapsed;
                    break;
                case "2":
                    play1.Visibility = Visibility.Visible;
                    play2.Visibility = Visibility.Visible;
                    play3.Visibility = Visibility.Collapsed;
                    break;
                case "3":
                    play1.Visibility = Visibility.Visible;
                    play2.Visibility = Visibility.Visible;
                    play3.Visibility = Visibility.Visible;
                    if (core.third_track_is_avail(core.makeArtistWord(tmp_art)))
                    {
                        play3.Source = play2.Source;
                    }
                    else
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
                    }
                    break;
                default:
                    play1.Visibility = Visibility.Visible;
                    play2.Visibility = Visibility.Visible;
                    play3.Visibility = Visibility.Visible;
                    if (core.third_track_is_avail(core.makeArtistWord(tmp_art)))
                    {
                        play3.Source = play2.Source;
                    }
                    else
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
                    }
                    break;
            }
            int i;
            i = core.index_in_original(tmp_art);
            art_index = i;
            moves = new int[60];
            move_order = 0;
            bool sz;
            sz = core.is_latin(core.makeArtistWord(tmp_art));
            string kbd;
            if (sz == true) { kbd = core.keyboard_en; } else { kbd = core.keyboard_ru; }
            Random rx = new Random();
            wrd = core.makeArtistWord(tmp_art);
            for (int j = wrd.Length + 1; j <= 15; j++)
            {
                wrd += kbd.Substring(rx.Next(0, kbd.Length), 1);
            }
            string[] symbs = new string[wrd.Length];
            for (int j = 0; j < wrd.Length; j++)
            {
                symbs[j] = wrd.Substring(j, 1);
            }
            Random rnd = new Random();
            string[] MyRand = symbs.OrderBy(x => rnd.Next()).ToArray();
            for (int j = 0; j < 15; j++)
            {
                set_char_for_button(j + 1, MyRand[j]);
            }
            if (tmp_art.word3.Length > 0)
            {
                words_count = 3;
            }
            else
            {
                if (tmp_art.word2.Length > 0)
                {
                    words_count = 2;
                }
                else
                {
                    words_count = 1;
                }
            }
            split = tmp_art.answerFormat.Split(new Char[] { ' ', '.', ',' });
            word1.Text = "";
            word2.Text = "";
            word3.Text = "";
            if (split.Length > 0)
            {
                word1.Text = split[0];
            }
            if (!(tmp_art.word2.Length > 0))
            {
                word2.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (split.Length > 1)
                {
                    word2.Text = split[1];
                }
            }
            if (!(tmp_art.word3.Length > 0))
            {
                word3.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (split.Length > 2)
                {
                    word3.Text = split[2];
                }
            }
        }
        public void load_artist(Artist art)
        {
            started_to_type = false;
            tmp_art = art;
            b1.Visibility = Visibility.Visible;
            b2.Visibility = Visibility.Visible;
            b3.Visibility = Visibility.Visible;
            b4.Visibility = Visibility.Visible;
            b5.Visibility = Visibility.Visible;
            b6.Visibility = Visibility.Visible;
            b7.Visibility = Visibility.Visible;
            b8.Visibility = Visibility.Visible;
            b9.Visibility = Visibility.Visible;
            b10.Visibility = Visibility.Visible;
            b11.Visibility = Visibility.Visible;
            b12.Visibility = Visibility.Visible;
            b13.Visibility = Visibility.Visible;
            b14.Visibility = Visibility.Visible;
            b15.Visibility = Visibility.Visible;
            word1.Visibility = Visibility.Visible;
            word2.Visibility = Visibility.Visible;
            word3.Visibility = Visibility.Visible;
            switch (core.main_level.tracks_count)
            {
                case "1":
                    play1.SetValue(Grid.ColumnProperty, 2);
                    play2.Visibility = Visibility.Collapsed;
                    play3.Visibility = Visibility.Collapsed;
                    break;
                case "2":
                    play1.Visibility = Visibility.Visible;
                    play2.Visibility = Visibility.Visible;
                    play3.Visibility = Visibility.Collapsed;
                    break;
                case "3":
                    play1.Visibility = Visibility.Visible;
                    play2.Visibility = Visibility.Visible;
                    play3.Visibility = Visibility.Visible;
                    if (core.third_track_is_avail(core.makeArtistWord(art)))
                    {
                        play3.Source = play2.Source;
                    }
                    else
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
                    }
                    break;
                default:
                    play1.Visibility = Visibility.Visible;
                    play2.Visibility = Visibility.Visible;
                    play3.Visibility = Visibility.Visible;
                    if (core.third_track_is_avail(core.makeArtistWord(art)))
                    {
                        play3.Source = play2.Source;
                    }
                    else
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
                    }
                    break;
            }
            int i;
            i = core.index_in_original(art);
            art_index = i;
            moves = new int[60];
            move_order = 0;
            bool sz;
            sz = core.is_latin(core.makeArtistWord(tmp_art));
            string kbd;
            if (sz == true) { kbd = core.keyboard_en; } else { kbd = core.keyboard_ru; }
            Random rx = new Random();
            wrd = core.makeArtistWord(tmp_art);
            for(int j=wrd.Length+1;j<=15;j++)
            {
                wrd += kbd.Substring(rx.Next(0, kbd.Length),1);
            }
            string[] symbs = new string[wrd.Length];
            for( int j=0; j<wrd.Length;j++)
            {
                symbs[j] = wrd.Substring(j, 1);
            }
            Random rnd = new Random();
            string[] MyRand = symbs.OrderBy(x => rnd.Next()).ToArray();
            for(int j=0;j<15;j++)
            {
                set_char_for_button(j + 1, MyRand[j]);
            }
            if (tmp_art.word3.Length > 0)
            {
                words_count = 3;
            }
            else
            {
                if(tmp_art.word2.Length>0)
                {
                    words_count = 2;
                }
                else
                {
                    words_count = 1;
                }
            }
            split = tmp_art.answerFormat.Split(new Char[] { ' ', '.', ',' });
            word1.Text = "";
            word2.Text = "";
            word3.Text = "";
            if(split.Length>0)
            {
                word1.Text = split[0];
            }            
            if(!(tmp_art.word2.Length>0))
            {
                word2.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (split.Length > 1)
                {
                    word2.Text = split[1];
                }
            }
            if(!(tmp_art.word3.Length>0))
            {
                word3.Visibility = Visibility.Collapsed;
            }
            else
            {
                if(split.Length>2)
                {
                    word3.Text = split[2];
                }                
            }
        }
        private void play1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(playing_index==1)
            {
                Media1.Stop();
                playing_index = 0;
                play1.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                playing = false;
            }
            else
            {
                if (playing == true)
                {
                    Media1.Stop();
                    play1.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                    play2.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                    if (core.third_track_is_avail(core.makeArtistWord(tmp_art)))
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                    }
                    else
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
                    }
                }
                using (IsolatedStorageFile myIS = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fS = myIS.OpenFile(String.Format("/{0}/{1}_{2}.mp3", core.level_ident, Convert.ToString(art_index + 1), "1"), System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        Media1.SetSource(fS);
                        Media1.Play();
                        playing = true;
                        playing_index = 1;
                        play1.Source = new BitmapImage(new Uri("/Assets/pausebutton.png", UriKind.Relative));
                    }
                }
            }            
        }
        private void play2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (playing_index == 2)
            {
                Media1.Stop();
                playing_index = 0;
                play2.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                playing = false;
            }
            else
            {
                if (playing == true)
                {
                    Media1.Stop();
                    play1.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                    play2.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                    if (core.third_track_is_avail(core.makeArtistWord(tmp_art)))
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                    }
                    else
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
                    }
                }
                using (IsolatedStorageFile myIS = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fS = myIS.OpenFile(String.Format("/{0}/{1}_{2}.mp3", core.level_ident, Convert.ToString(art_index + 1), "2"), System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        Media1.SetSource(fS);
                        Media1.Play();
                        playing = true;
                        playing_index = 2;
                        play2.Source = new BitmapImage(new Uri("/Assets/pausebutton.png", UriKind.Relative));
                    }
                }
            }
        }
        private void play3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(!core.third_track_is_avail(core.makeArtistWord(tmp_art)))
            {
                MessageBox.Show(AppResources.ThirdTrackLocked, AppResources.ApplicationTitle,MessageBoxButton.OK);
            }
            else
            {
                if(playing==true)
                {
                    if(playing_index==3)
                    {
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                        Media1.Pause();
                        playing = false;
                        playing_index = 0;
                    }
                    else
                    {
                        Media1.Stop();
                        play1.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                        play2.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                        play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
                        using (IsolatedStorageFile myIS = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            using (IsolatedStorageFileStream fS = myIS.OpenFile(String.Format("/{0}/{1}_{2}.mp3", core.level_ident, Convert.ToString(art_index + 1), "3"), System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            {
                                Media1.SetSource(fS);
                                Media1.Play();
                                playing = true;
                                playing_index = 3;
                                play3.Source = new BitmapImage(new Uri("/Assets/pausebutton.png", UriKind.Relative));
                            }
                        }
                    }
                }
                else
                {
                    using (IsolatedStorageFile myIS = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream fS = myIS.OpenFile(String.Format("/{0}/{1}_{2}.mp3", core.level_ident, Convert.ToString(art_index + 1), "3"), System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            Media1.SetSource(fS);
                            Media1.Play();
                            playing = true;
                            playing_index = 3;
                            play3.Source = new BitmapImage(new Uri("/Assets/pausebutton.png", UriKind.Relative));
                        }
                    }
                }
            }
        }
        private void Media1_MediaEnded(object sender, RoutedEventArgs e)
        {
            play1.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            play2.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            if (core.third_track_is_avail(core.makeArtistWord(tmp_art)))
            {
                play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            }
            else
            {
                play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
            }
            playing_index = 0;
            playing = false;
        }
        private void skip_button_Click(object sender, RoutedEventArgs e)
        {
            Media1.Stop();
            playing = false;
            playing_index = 0;
            play1.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            play2.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            if (core.third_track_is_avail(core.makeArtistWord(tmp_art)))
            {
                play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            }
            else
            {
                play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
            }
            SkipAnimation.Begin();
            if (core.sorted.Count > 1)
            {
                if (artist_index == core.sorted.Count - 1)
                {
                    load_artist(0);
                }
                else
                {
                    load_artist(artist_index + 1);
                }
            }
            else
            {                
                load_artist(0);
            }
        }
        public void set_char_for_button(int xfa, string xfax)
        {
            switch (xfa)
            {
                case 1:
                    b1.Content = xfax;
                    break;
                case 2:
                    b2.Content = xfax;
                    break;
                case 3:
                    b3.Content = xfax;
                    break;
                case 4:
                    b4.Content = xfax;
                    break;
                case 5:
                    b5.Content = xfax;
                    break;
                case 6:
                    b6.Content = xfax;
                    break;
                case 7:
                    b7.Content = xfax;
                    break;
                case 8:
                    b8.Content = xfax;
                    break;
                case 9:
                    b9.Content = xfax;
                    break;
                case 10:
                    b10.Content = xfax;
                    break;
                case 11:
                    b11.Content = xfax;
                    break;
                case 12:
                    b12.Content = xfax;
                    break;
                case 13:
                    b13.Content = xfax;
                    break;
                case 14:
                    b14.Content = xfax;
                    break;
                case 15:
                    b15.Content = xfax;
                    break;
                default:
                    break;
            }
        }
        private void b1_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b1.Content;
            symbol_paste(1, symbol);
        }
        private void b2_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b2.Content;
            symbol_paste(2, symbol);
        }
        private void b3_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b3.Content;
            symbol_paste(3, symbol);
        }
        private void b4_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b4.Content;
            symbol_paste(4, symbol);
        }
        private void b5_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b5.Content;
            symbol_paste(5, symbol);
        }
        private void b6_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b6.Content;
            symbol_paste(6, symbol);
        }
        private void b7_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b7.Content;
            symbol_paste(7, symbol);
        }
        private void b8_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b8.Content;
            symbol_paste(8, symbol);
        }
        private void b9_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b9.Content;
            symbol_paste(9, symbol);
        }
        private void b10_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b10.Content;
            symbol_paste(10, symbol);
        }
        private void b11_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b11.Content;
            symbol_paste(11, symbol);
        }
        private void b12_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b12.Content;
            symbol_paste(12, symbol);
        }
        private void b13_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b13.Content;
            symbol_paste(13, symbol);
        }
        private void b14_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b14.Content;
            symbol_paste(14, symbol);
        }
        private void b15_Click(object sender, RoutedEventArgs e)
        {
            string symbol;
            symbol = (string)b15.Content;
            symbol_paste(15, symbol);
        }
        public string get_button_content(int id)
        {
            switch(id)
            {
                case 1:
                    return (string)b1.Content;
                case 2:
                    return (string)b2.Content;
                case 3:
                    return (string)b3.Content;
                case 4:
                    return (string)b4.Content;
                case 5:
                    return (string)b5.Content;
                case 6:
                    return (string)b6.Content;
                case 7:
                    return (string)b7.Content;
                case 8:
                    return (string)b8.Content;
                case 9:
                    return (string)b9.Content;
                case 10:
                    return (string)b10.Content;
                case 11:
                    return (string)b11.Content;
                case 12:
                    return (string)b12.Content;
                case 13:
                    return (string)b13.Content;
                case 14:
                    return (string)b14.Content;
                case 15:
                    return (string)b15.Content;
                default:
                    return (string)b1.Content;
            }
        }
        public void set_visibility_of_button(int visibility_of_button, int number_of_button)
        {
            Visibility mg = new Visibility();
            if (visibility_of_button == 1)
            {
                mg = Visibility.Visible;
            }
            else
            {
                mg = Visibility.Collapsed;
            }
            switch (number_of_button)
            {
                case 1:
                    b1.Visibility = mg;
                    break;
                case 2:
                    b2.Visibility = mg;
                    break;
                case 3:
                    b3.Visibility = mg;
                    break;
                case 4:
                    b4.Visibility = mg;
                    break;
                case 5:
                    b5.Visibility = mg;
                    break;
                case 6:
                    b6.Visibility = mg;
                    break;
                case 7:
                    b7.Visibility = mg;
                    break;
                case 8:
                    b8.Visibility = mg;
                    break;
                case 9:
                    b9.Visibility = mg;
                    break;
                case 10:
                    b10.Visibility = mg;
                    break;
                case 11:
                    b11.Visibility = mg;
                    break;
                case 12:
                    b12.Visibility = mg;
                    break;
                case 13:
                    b13.Visibility = mg;
                    break;
                case 14:
                    b14.Visibility = mg;
                    break;
                case 15:
                    b15.Visibility = mg;
                    break;
                default:
                    break;
            }
        }
        private void make_move(int x_move)
        {
            btn_sfx("keyboard");
            set_visibility_of_button(0, x_move);
            moves[move_order] = x_move;
            move_order++;
        }
        private void symbol_paste(int id, string symb)
        {
            if(!started_to_type)
            { 
                word1.Text = "";
                word2.Text = "";
                word3.Text = "";
                started_to_type = true;
            }
            switch(words_count)
            {
                case 1:
                    if(word1.Text.Length==(tmp_art.word1.Length-1))
                    {
                        word1.Text += symb;
                        make_move(id);
                        checker();
                    }
                    else
                    {
                        if(word1.Text.Length!=tmp_art.word1.Length)
                        {
                            word1.Text += symb;
                            make_move(id);
                        }                        
                    }                    
                    break;
                case 2:
                    if(word1.Text.Length!=tmp_art.word1.Length)
                    {
                        word1.Text += symb;
                        make_move(id);
                    }
                    else 
                    {
                        if(word2.Text.Length==(tmp_art.word2.Length-1))
                        {
                            word2.Text += symb;
                            make_move(id);
                            checker();
                        }
                        else
                        {
                            if(word2.Text.Length!=tmp_art.word2.Length)
                            {
                                word2.Text += symb;
                                make_move(id);
                            }                            
                        }
                    }
                    break;
                case 3:
                    if(word2.Text.Length!=tmp_art.word2.Length)
                    {
                        if(word1.Text.Length!=tmp_art.word1.Length)
                        {
                            word1.Text += symb;
                            make_move(id);
                        }
                        else
                        {
                            word2.Text += symb;
                            make_move(id);
                        }
                    }
                    else
                    {
                        if (word3.Text.Length == (tmp_art.word3.Length - 1))
                        {
                            word3.Text += symb;
                            make_move(id);
                            checker();
                        }
                        else
                        {
                            if (word3.Text.Length != tmp_art.word3.Length)
                            {
                                word3.Text += symb;
                                make_move(id);
                            }                            
                        }
                    }
                    break;
            }
        }
        private void checker()
        {
            string sd;       
            int z;
            string sd1;
            string time;
            switch (words_count)
            {
                case 1:
                    sd = tmp_art.word1;
                    sd1 = word1.Text;
                    break;
                case 2:
                    sd = tmp_art.word1 + tmp_art.word2;
                    sd1 = word1.Text + word2.Text;
                    break;
                case 3:
                    sd = tmp_art.word1 + tmp_art.word2 + tmp_art.word3;
                    sd1 = word1.Text + word2.Text + word3.Text;
                    break;
                default:
                    sd = tmp_art.word1;
                    sd1 = word1.Text;
                    break;
            }
            z = check_identity(sd1, sd);
            if (z == sd.Length - 1)
            {
                OneMistake.Begin();
                btn_sfx("one_mistake");
            }
            else
            {
                if(sd1!=sd)
                {
                    WrongAnswer.Begin();
                    btn_sfx("wrong");                    
                }
                else
                {
                    time = timer.Text;
                    timing.Stop();
                    timer.Text = time;
                    core.set_guessed(tmp_art);
                    if(core.level_guessed==core.main_level.artists.Count)
                    {
                        core.set_level_fully_guessed();
                        Congratulations.Begin();
                    }
                    else
                    {
                        if (tmp_art.song1.Length > 0)
                        {
                            song1.Content = tmp_art.song1;
                            song1.Visibility = Visibility.Visible;
                        }
                        if (tmp_art.song2.Length > 0)
                        {
                            song2.Content = tmp_art.song2;
                            song2.Visibility = Visibility.Visible;
                        }
                        if (tmp_art.song3.Length > 0)
                        {
                            song3.Content = tmp_art.song3;
                            song3.Visibility = Visibility.Visible;
                        }
                        artist_name.Text = word1.Text + " " + word2.Text + " " + word3.Text;
                        CorrectAnswer.Begin();
                    }                    
                }
            }
        }
        private int check_identity(string s1, string s2)
        {
            int check = 0;
            for(int i = 0;i<s2.Length;i++)
            {
                if(s2.Substring(i,1)==s1.Substring(i,1))
                {
                    check++;
                }
            }
            return check;
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int tem1;
            switch(move_order)
            {
                case 0:                    
                    return;
                case 1:
                    switch(split.Length)
                    {
                        case 2:
                            word1.Text = split[0];
                            word2.Text = split[1];
                            break;
                        case 3:
                            word1.Text = split[0];
                            word2.Text = split[1];
                            word3.Text = split[2];
                            break;
                        default:
                            word1.Text = split[0];
                            break;
                    }
                    tem1 = moves[move_order - 1];
                    set_visibility_of_button(1, tem1);
                    word1.Foreground = new SolidColorBrush(Colors.White);
                    word2.Foreground = new SolidColorBrush(Colors.White);
                    word3.Foreground = new SolidColorBrush(Colors.White);
                    started_to_type = false;
                    move_order--;
                    break;
                default:
                    switch(words_count)
                    {
                        case 1:
                            word1.Text = word1.Text.Substring(0, word1.Text.Length - 1);
                            break;
                        case 2:
                            if(word2.Text=="")
                            {
                                word1.Text = word1.Text.Substring(0, word1.Text.Length - 1);
                            }
                            else
                            {
                                word2.Text = word2.Text.Substring(0, word2.Text.Length - 1);
                            }
                            break;
                        case 3:
                            if(word3.Text=="")
                            {
                                if(word2.Text=="")
                                {
                                    word1.Text = word1.Text.Substring(0, word1.Text.Length - 1);
                                }
                                else
                                {
                                    word2.Text = word2.Text.Substring(0, word2.Text.Length - 1);
                                }
                            }
                            else
                            {
                                word3.Text = word3.Text.Substring(0, word3.Text.Length - 1);
                            }
                            break;
                    }
                    tem1 = moves[move_order - 1];
                    set_visibility_of_button(1, tem1);
                    word1.Foreground = new SolidColorBrush(Colors.White);
                    word2.Foreground = new SolidColorBrush(Colors.White);
                    word3.Foreground = new SolidColorBrush(Colors.White);
                    move_order--;
                    break;
            }            
        }
        private void hints_button_Click(object sender, RoutedEventArgs e)
        {
            hints.Visibility = Visibility.Visible;
        }
        void btn_sfx(string effect)
        {
            if(!s_core.sfx_enabled())
            {
                return;
            }
            if(playing)
            {
                return;
            }
            switch (effect)
            {
                case "keyboard":
                    snd.playClick();
                    break;
                case "correct":
                    snd.playCorrect();
                    break;
                case "onemistake":
                    snd.playOneMistake();
                    break;
                case "one_mistake":
                    snd.playOneMistake();
                    break;
                case "wrong":
                    snd.playWrong();
                    break;
                default:
                    snd.playClick();
                    break;
            }
        }
        private void pushme2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            hints.Visibility = Visibility.Collapsed;
        }
        private void btnHintShowFirstSymbol_Click(object sender, RoutedEventArgs e)
        {
            if(cash.count<cash.hint_show_first_symbol)
            {
                MessageBox.Show(String.Format(AppResources.NotEnoughCoins,Convert.ToString(cash.hint_show_first_symbol-cash.count)),AppResources.ApplicationTitle,MessageBoxButton.OKCancel);
                //TODO:INAPP
                return;
            }
            string tmp;
            string first;
            first = tmp_art.word1.Substring(0, 1);
            for(int i=1;i<=15;i++)
            {
                tmp = get_button_content(i);
                if(first==tmp)
                {
                    symbol_paste(i, first);
                    hints.Visibility = Visibility.Collapsed;
                    cash.count -= cash.hint_show_first_symbol;
                    coins_count.Text = Convert.ToString(cash.count);
                    return;
                }
            }
        }
        private void btnHintRemoveExtraLettes_Click(object sender, RoutedEventArgs e)
        {  
            if(cash.count<cash.hint_remove_extra_symbols)
            {
                MessageBox.Show(String.Format(AppResources.NotEnoughCoins,Convert.ToString(cash.hint_remove_extra_symbols-cash.count)),AppResources.ApplicationTitle,MessageBoxButton.OKCancel);
                //TODO:INAPP
                return;
            }
            if(core.makeArtistWord(tmp_art).Length==15)
            {
                MessageBox.Show(AppResources.NoExtraSymbs);
            }
            else
            {
                string extra_part;
                string btn_content;
                int pos;
                extra_part = wrd.Replace(core.makeArtistWord(tmp_art), "");
                for(int i=1; i<16; i++)
                {
                    btn_content = get_button_content(i);
                    pos = extra_part.IndexOf(btn_content);
                    if(pos!=-1)
                    {
                        set_visibility_of_button(0, i);
                        make_move(i);
                        if(pos!=0)
                        { 
                            extra_part = extra_part.Substring(0, pos) + extra_part.Substring(pos + 1);
                        }
                        else
                        {
                            extra_part = extra_part.Substring(1);
                        }
                    }
                }
                hints.Visibility = Visibility.Collapsed;
                cash.count -= cash.hint_remove_extra_symbols;
                coins_count.Text = Convert.ToString(cash.count);
            }
        }
        private void btnHintUnlockTrack_Click(object sender, RoutedEventArgs e)
        {
            if(cash.count<cash.hint_unlock_third_track)
            {
                MessageBox.Show(String.Format(AppResources.NotEnoughCoins,Convert.ToString(cash.hint_unlock_third_track-cash.count)),AppResources.ApplicationTitle,MessageBoxButton.OKCancel);
                //TODO: INAPP
                return;
            }
            core.set_third_track_avail(core.makeArtistWord(tmp_art));
            play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            cash.count -= cash.hint_unlock_third_track;
            coins_count.Text = Convert.ToString(cash.count);
        }
        private void btnHintCorrectAnswer_Click(object sender, RoutedEventArgs e)
        {
            if(cash.count<cash.hint_show_correct_answer)
            {
                MessageBox.Show(String.Format(AppResources.NotEnoughCoins,Convert.ToString(cash.hint_show_correct_answer-cash.count)),AppResources.ApplicationTitle,MessageBoxButton.OKCancel);
                //TODO:INAPP
                return;
            }
            MessageBox.Show(String.Format("{0} - {1}",AppResources.CorrectAnswer, tmp_art.word1 + " " + tmp_art.word2 + " " + tmp_art.word3));
            cash.count -= cash.hint_show_correct_answer;
            coins_count.Text = Convert.ToString(cash.count);
        }
        private void hints_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            hints.Visibility = Visibility.Collapsed;
        }
        private void next_artist_Click(object sender, RoutedEventArgs e)
        {
            correct.Visibility = Visibility.Collapsed;
            Media1.Stop();
            playing = false;
            playing_index = 0;
            play1.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            play2.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            if (core.third_track_is_avail(core.makeArtistWord(tmp_art)))
            {
                play3.Source = new BitmapImage(new Uri("/Assets/playbutton.png", UriKind.Relative));
            }
            else
            {
                play3.Source = new BitmapImage(new Uri("/Assets/playbutton_inactive.png", UriKind.Relative));
            }
            SkipAnimation.Begin();
            if (core.sorted.Count > 1)
            {
                if (artist_index == core.sorted.Count - 1)
                {
                    load_artist(0);
                }
                else
                {
                    load_artist(artist_index + 1);
                }
            }
            else
            {
                load_artist(0);
            }
        }




        //Удалить при релизе!
        private void coins_count_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            cash.count += 1000;
            coins_count.Text = Convert.ToString(cash.count);
        }

      
    }
}