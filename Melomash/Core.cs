using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO.IsolatedStorage;
using System.IO;
using System;
using Melomash;
using Barbadoz.Melomany.Engine.Json;
using System.Linq;
using System.Windows;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Resources;
using Microsoft.Xna.Framework;

namespace Barbadoz.Melomany.Engine
{
    class coins
    {
        int coins_count;
        public int hint_remove_extra_symbols = 20;
        public int hint_show_first_symbol = 10;
        public int hint_show_correct_answer = 70;
        public int hint_unlock_third_track = 15;
        public int for_win_15_sec = 10;
        public int for_win_30_sec = 7;
        public int for_win_more_sec = 5;
        public int win_level = 50;
        public int coins_for_day = 100;
        public int count
        {
            set
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                if (appSettings.Contains("coins") == true)
                {
                    appSettings["coins"] = value;
                }
                else
                {
                    appSettings.Add("coins", value);
                }
                appSettings.Save();
            }
            get
            {
                IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
                if (appSettings.Contains("coins") == true)
                {
                    coins_count = (int)appSettings["coins"];
                    return coins_count;
                }
                else
                {
                    coins_count = 100;
                    appSettings.Add("coins", coins_count);
                    appSettings.Save();
                    return coins_count;
                }
            }
        }
    }
    class settings_core
    {
        public bool sfx_enabled()
        {
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains("sfx"))
            {
                if (Convert.ToString(isos["sfx"]) == "disabled")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        public void set_sfx_disabled()
        {
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if(isos.Contains("sfx"))
            {
                isos["sfx"] = "disabled";
            }
            else
            {
                isos.Add("sfx", "disabled");
            }
            isos.Save();
        }
        public void set_sfx_enabled()
        {
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains("sfx"))
            {
                isos["sfx"] = "enabled";
            }
            else
            {
                isos.Add("sfx", "enabled");
            }
            isos.Save();
        }
    }
    class Core
    { 
        public Levels_Base lvls;
        List<LevelBase> lvl;
        bool init;
        public Level main_level;
        private void save_levels()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            string sb;
            StreamWriter writer = null;
            writer = new StreamWriter(new IsolatedStorageFileStream("levels_base.txt", FileMode.Create, isoStore));
            if (init == true)
            {
                lvls.levels.RemoveAt(0);
                init = false;
            }
            sb = JsonConvert.SerializeObject(lvls);
            writer.Write(sb);
            writer.Close();
        }
        private void load_levels()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            string sb;
            StreamReader reader = new StreamReader(new IsolatedStorageFileStream("levels_base.txt", FileMode.OpenOrCreate, isoStore));
            sb = reader.ReadToEnd();
            reader.Close();
            lvls = JsonConvert.DeserializeObject<Levels_Base>(sb);
        }
        public void LoadByIdent()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            string sb;
            StreamReader reader = new StreamReader(new IsolatedStorageFileStream(String.Format("/{0}/declare.txt", level_ident), FileMode.OpenOrCreate, isoStore));
            sb = reader.ReadToEnd();
            reader.Close();
            main_level = JsonConvert.DeserializeObject<Level>(sb);
        }
        public Core()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (isoStore.FileExists("levels_base.txt"))
            {
                load_levels();
            }
            else
            {
                lvl = new List<LevelBase>
                {
                    new LevelBase() {ident="null",name="",artists_count="" }
                };
                lvls = new Levels_Base();
                lvls.levels = lvl;
                init = true;
            }
        }
        public void add_level(LevelBase lvl)
        {
            lvls.levels.Add(lvl);
        }
        public void add_level(string ident, string name, string artists_count)
        {
            LevelBase tmp_lvl;
            tmp_lvl = new LevelBase();
            tmp_lvl.artists_count = artists_count;
            tmp_lvl.name = name;
            tmp_lvl.ident = ident;
            lvls.levels.Add(tmp_lvl);
            save_levels();
        }
        public void remove_level(string ident)
        {
            //Объявление переменных
            string sb;
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            Level tmp_lvl;
            int files_count;
            int tracks_count;
            string[] files;
            int counter;
            counter = 1;
            int toDelete = 0;
            //Генерация списка файлов
            StreamReader reader = new StreamReader(new IsolatedStorageFileStream(String.Format("/{0}/declare.txt", ident), FileMode.OpenOrCreate, isoStore));
            sb = reader.ReadToEnd();
            reader.Close();
            tmp_lvl = JsonConvert.DeserializeObject<Level>(sb);
            tracks_count = Convert.ToInt32(tmp_lvl.tracks_count);
            files_count = tracks_count * tmp_lvl.artists.Count;
            files_count++;
            files = new string[files_count];
            files[0] = String.Format("/{0}/declare.txt", ident);
            for (int j = 1; j <= tmp_lvl.artists.Count; j++)
            {
                for (int i = 1; i <= tracks_count; i++)
                {
                    files[counter] = String.Format("/{0}/{1}_{2}.mp3", ident, Convert.ToString(j), Convert.ToString(i));
                    counter++;
                }
            }
            //Удаление
            foreach (string st in files)
            {
                isoStore.DeleteFile(st);
            }
            isoStore.DeleteDirectory(ident);
            //Удаление из списка уровней
            counter = 0;
            foreach (LevelBase lvl in lvls.levels)
            {
                if (lvl.ident == ident)
                {
                    toDelete = counter;
                }
                counter++;
            }
            lvls.levels.RemoveAt(toDelete);
            save_levels();
        }
        public bool level_exists(string ident)
        {
            bool sz;
            sz = false;
            foreach (LevelBase lvl in lvls.levels)
            {
                if (lvl.ident == ident)
                {
                    sz = true;
                }
            }
            return sz;
        }
        public bool is_russian(string text)
        {
            bool result = false;
            char[] letters = text.ToCharArray();
            for (int i = 0; i < letters.Length; i++)
            {
                int charValue = System.Convert.ToInt32(letters[i]);
                if (charValue > 128)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        public bool is_latin(string text)
        {
            bool result = true;
            char[] letters = text.ToCharArray();
            for (int i = 0; i < letters.Length; i++)
            {
                int charValue = System.Convert.ToInt32(letters[i]);
                if (charValue > 128)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        public int get_tracks_count()
        {
            return Convert.ToInt32(main_level.tracks_count);
        }
        public string keyboard_layout = "QWERTYUIOPASDFGHJKLZXCVBNM";
        public string keyboard_ru = "ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ";
        public string keyboard_en = "QWERTYUIOPASDFGHJKLZXCVBNM";
        public bool artist_is_guessed(string artist_name)
        {
            string checker;
            checker = String.Format("{0}.{1}", level_ident, artist_name);
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int level_guessed
        {
            get
            {
                IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
                string checker;
                checker = String.Format("{0}_guessed", level_ident);
                int level_g;
                if (isos.Contains(checker))
                {
                    level_g = (int)isos[checker];
                    return level_g;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
                string checker;
                checker = String.Format("{0}_guessed", level_ident);
                if (isos.Contains(checker))
                {
                    isos[checker] = value;
                }
                else
                {
                    isos.Add(checker, value);
                }
                isos.Save();
            }
        }
        public string level_ident = "nulle";
        public void set_guessed(Artist art)
        {
            string checker;
            checker = String.Format("{0}.{1}", level_ident, makeArtistWord(art));
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            isos.Add(checker, "true");
            isos.Save();
            level_guessed++;
        }
        public void set_guessed(string artist_name)
        {
            string checker;
            checker = String.Format("{0}.{1}", level_ident, artist_name);
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            isos.Add(checker, "true");
            isos.Save();
            level_guessed++;
        }
        public bool third_track_is_avail(string artist_name)
        {
            string checker;
            checker = String.Format("{0}.{1}.{2}", level_ident, artist_name, "third");
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void set_third_track_avail(string artist_name)
        {
            string checker;
            checker = String.Format("{0}.{1}.third", level_ident, artist_name);
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                isos[checker] = true;
            }
            else
            {
                isos.Add(checker, "true");
            }
            isos.Save();

        }
        public bool level_is_fully_guessed()
        {
            string checker;
            checker = String.Format("{0}.{1}", level_ident, "fg");
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void set_level_fully_guessed()
        {
            string checker;
            checker = String.Format("{0}.{1}", level_ident, "fg");
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker) == false)
            {
                isos.Add(checker, "guessed");
            }
            isos.Save();
        }
        public bool extra_symb_used(string artist_name)
        {
            string checker;
            checker = String.Format("{0}.{1}.{2}", level_ident, artist_name, "extrasymb");
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void set_extra_symb_used(string artist_name)
        {
            string checker;
            checker = String.Format("{0}.{1}.{2}", level_ident, artist_name, "extrasymb");
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                isos[checker] = "true";
            }
            else
            {
                isos.Add(checker, "true");
            }
            isos.Save();
        }
        public bool isFirstRun()
        {
            string checker;
            checker = "FirstRun";
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void setFirstRun()
        {
            string checker;
            checker = "FirstRun";
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                isos[checker] = "true";
            }
            else
            {
                isos.Add(checker, "true");
            }
            isos.Save();
        }
        public bool is_removed_ad()
        {
            string checker;
            checker = "removed_ad";
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void set_removed_ad()
        {
            string checker;
            checker = "removed_ad";
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains(checker))
            {
                isos[checker] = "true";
            }
            else
            {
                isos.Add(checker, "true");
            }
            isos.Save();
        }
        public List<Artist> sorted;
        public void randomize_list()
        {
            sorted = get_sorted();
            int i = 0;
            foreach (Artist art in sorted.ToArray())
            {
                if (artist_is_guessed(makeArtistWord(art)))
                {
                    sorted.RemoveAt(i);
                }
                i++;
            }
        }
        public List<Artist> get_sorted()
        {
            List<Artist> tmp = new List<Artist>();
            Random rnd = new Random();
            foreach(Artist art in main_level.artists)
            {
                if(!artist_is_guessed(makeArtistWord(art)))
                {
                    tmp.Add(art);
                }
            }
            tmp = tmp.OrderBy(x => rnd.Next()).ToList();
            return tmp;
        }
        public int index_in_original(Artist art)
        {
            int i = 0;
            foreach (Artist artx in main_level.artists)
            {
                if (artx == art)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        public string makeArtistWord(Artist art)
        {
            return art.word1 + art.word2 + art.word3;
        }
    }
    class sound_effects
    {
        public SoundEffect click_sound1 { get; set; }
        public SoundEffect click_sound2 { get; set; }
        public SoundEffect click_sound3 { get; set; }
        public SoundEffect click_sound4 { get; set; }
        public SoundEffect click_sound5 { get; set; }
        public SoundEffect click_sound6 { get; set; }
        public SoundEffect click_sound7 { get; set; }
        public SoundEffect click_sound { get; set; }
        public SoundEffect correct_sound { get; set; }
        public SoundEffect wrong_sound { get; set; }
        public SoundEffect cash_sound { get; set; }
        public SoundEffect onemistake { get; set; }
        int click_count;
        public sound_effects()
        {
            click_count = 1;
            this.load_sounds();
        }
        public bool is_sounder()
        {
            int tmp;
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (isos.Contains("sound") == true)
            {
                tmp = Convert.ToInt32(isos["sound"]);
                if (tmp == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        public void set_sound(bool sz)
        {
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if (sz == true)
            {
                if (isos.Contains("sound") == true)
                {
                    isos["sound"] = 1;
                }
                else
                {
                    isos.Add("sound", 1);
                }
            }
            else
            {
                if (isos.Contains("sound") == true)
                {
                    isos["sound"] = 0;
                }
                else
                {
                    isos.Add("sound", 0);
                }
            }
            isos.Save();
        }
        public bool load_sounds()
        {
            SoundEffect Sound;
            Sound = null;
            try
            {
                StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound = Sound;
                SoundFileInfo = null;
                SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click1.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound1 = Sound;
                SoundFileInfo = null;
                SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click2.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound2 = Sound;
                SoundFileInfo = null;
                SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click3.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound3 = Sound;
                SoundFileInfo = null;
                SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click4.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound4 = Sound;
                SoundFileInfo = null;
                SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click5.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound5 = Sound;
                SoundFileInfo = null;
                SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click6.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound6 = Sound;
                SoundFileInfo = null;
                SoundFileInfo = App.GetResourceStream(new Uri("Resources/wav/click7.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                click_sound7 = Sound;
                SoundFileInfo = null;
                StreamResourceInfo SoundFileInfo_c = App.GetResourceStream(new Uri("Resources/wav/correct.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo_c.Stream);
                correct_sound = Sound;
                StreamResourceInfo SounfFileInfo_w = App.GetResourceStream(new Uri("Resources/wav/wrong.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SounfFileInfo_w.Stream);
                wrong_sound = Sound;
                StreamResourceInfo SoundFileInfo_cash = App.GetResourceStream(new Uri("Resources/wav/cash.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo_cash.Stream);
                cash_sound = Sound;
                StreamResourceInfo SoundFileInfo_one_mistake = App.GetResourceStream(new Uri("Resources/wav/onemistake.wav", UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo_one_mistake.Stream);
                onemistake = Sound;
                FrameworkDispatcher.Update();
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        public void playOneMistake()
        {
            if (is_sounder() == true)
            {
                onemistake.Play();
            }
        }
        public void playTwoMistake()
        {
            if (is_sounder() == true) { onemistake.Play(); }
        }
        public void playCash()
        {
            if (is_sounder() == true) { cash_sound.Play(); }
        }
        public void playClick()
        {
            if (is_sounder() == true)
            {
                switch (click_count)
                {
                    case 1:
                        click_sound1.Play();
                        click_count++;
                        break;
                    case 2:
                        click_sound2.Play();
                        click_count++;
                        break;
                    case 3:
                        click_sound3.Play();
                        click_count++;
                        break;
                    case 4:
                        click_sound4.Play();
                        click_count++;
                        break;
                    case 5:
                        click_sound5.Play();
                        click_count++;
                        break;
                    case 6:
                        click_sound6.Play();
                        click_count++;
                        break;
                    case 7:
                        click_sound7.Play();
                        click_count = 1;
                        break;
                    default:
                        click_sound.Play();
                        break;
                }
            }
        }
        public void playCorrect()
        {
            if (is_sounder() == true)
            {
                correct_sound.Play();
            }
        }
        public void playWrong()
        {
            if (is_sounder() == true)
            {
                wrong_sound.Play();
            }
        }
        private bool LoadSound(String SoundFilePath, out SoundEffect Sound)
        {
            Sound = null;
            try
            {
                StreamResourceInfo SoundFileInfo = App.GetResourceStream(new Uri(SoundFilePath, UriKind.Relative));
                Sound = SoundEffect.FromStream(SoundFileInfo.Stream);
                FrameworkDispatcher.Update();
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}

