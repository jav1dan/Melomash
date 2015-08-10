using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Melomash.Resources;
using System.IO.IsolatedStorage;
using Barbadoz.Melomany.Engine;
using System.IO;

namespace Melomash
{
    public partial class App : Application
    {
        
        /// <summary>
        /// Обеспечивает быстрый доступ к корневому кадру приложения телефона.
        /// </summary>
        /// <returns>Корневой кадр приложения телефона.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Конструктор объекта приложения.
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync();
            // Глобальный обработчик неперехваченных исключений.
            UnhandledException += Application_UnhandledException;

            // Стандартная инициализация XAML
            InitializeComponent();

            // Инициализация телефона
            InitializePhoneApplication();

            // Инициализация отображения языка
            InitializeLanguage();

            // Отображение сведений о профиле графики во время отладки.
            if (Debugger.IsAttached)
            {
                // Отображение текущих счетчиков частоты смены кадров.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Отображение областей приложения, перерисовываемых в каждом кадре.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Включение режима визуализации анализа нерабочего кода,
                // для отображения областей страницы, переданных в GPU, с цветным наложением.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Предотвратить выключение экрана в режиме отладчика путем отключения
                // определения состояния простоя приложения.
                // Внимание! Используйте только в режиме отладки. Приложение, в котором отключено обнаружение бездействия пользователя, будет продолжать работать
                // и потреблять энергию батареи, когда телефон не будет использоваться.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Код для выполнения при запуске приложения (например, из меню "Пуск")
        // Этот код не будет выполняться при повторной активации приложения
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            IsolatedStorageSettings isos = IsolatedStorageSettings.ApplicationSettings;
            if(!isos.Contains("firstRun"))
            {
                //Выполнение копирования уровней
                Core core = new Core();
                coins cash = new coins();
                core.add_level("rock_basic", "Рок музыка", "10");
                core.add_level("pop_basic", "Поп музыка", "10");
                core.add_level("rap_basic", "Рэп и хип-хоп", "10");
                IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                if (!fileStorage.DirectoryExists("rock_basic"))
                {
                    fileStorage.CreateDirectory("rock_basic");
                }
                if (!fileStorage.DirectoryExists("pop_basic"))
                {
                    fileStorage.CreateDirectory("pop_basic");
                }
                if (!fileStorage.DirectoryExists("rap_basic"))
                {
                    fileStorage.CreateDirectory("rap_basic");
                }
                string DBFile = "/{0}/{1}_{2}.mp3";
                string tmpfile1;
                string tmpfile2;
                string ResFile = "Resources/{0}/{1}_{2}.mp3";
                string dir;
                dir = "rock_basic";
                if (!fileStorage.FileExists("/rock_basic/declare.txt"))
                {
                    CopyFromContentToStorage(fileStorage, "Resources/rock_basic/declare.txt", "/rock_basic/declare.txt");
                }
                if (!fileStorage.FileExists("/pop_basic/declare.txt"))
                {
                    CopyFromContentToStorage(fileStorage, "Resources/pop_basic/declare.txt", "/pop_basic/declare.txt");
                }
                if (!fileStorage.FileExists("/rap_basic/declare.txt"))
                {
                    CopyFromContentToStorage(fileStorage, "Resources/rap_basic/declare.txt", "/rap_basic/declare.txt");
                }
                for (int i = 1; i <= 10; i++)
                {
                    tmpfile1 = String.Format(DBFile,dir, Convert.ToString(i), "1");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "1");
                    if(!fileStorage.FileExists(tmpfile1))
                        {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "2");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "2");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "3");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "3");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                }
                dir = "pop_basic";

                for (int i = 1; i <= 10; i++)
                {
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "1");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "1");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "2");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "2");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "3");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "3");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                }
                dir = "rap_basic";
                for (int i = 1; i <= 10; i++)
                {
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "1");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "1");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "2");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "2");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                    tmpfile1 = String.Format(DBFile, dir, Convert.ToString(i), "3");
                    tmpfile2 = String.Format(ResFile, dir, Convert.ToString(i), "3");
                    if (!fileStorage.FileExists(tmpfile1))
                    {
                        CopyFromContentToStorage(fileStorage, tmpfile2, tmpfile1);
                    }
                }
                //Задаем начальный баланс
                cash.count = 100;
                isos.Add("firstRun", "yes");
                isos.Save();
            }
        }
        private void CopyFromContentToStorage(IsolatedStorageFile ISF, String SourceFile, String DestinationFile)
        {
            Stream Stream = Application.GetResourceStream(new Uri(SourceFile, UriKind.Relative)).Stream;
            IsolatedStorageFileStream ISFS = new IsolatedStorageFileStream(DestinationFile, System.IO.FileMode.Create, System.IO.FileAccess.Write, ISF);
            CopyStream(Stream, ISFS);
            ISFS.Flush();
            ISFS.Close();
            Stream.Close();
            ISFS.Dispose();
        }
        private void CopyStream(Stream Input, IsolatedStorageFileStream Output)
        {
            Byte[] Buffer = new Byte[5120];
            Int32 ReadCount = Input.Read(Buffer, 0, Buffer.Length);
            while (ReadCount > 0)
            {
                Output.Write(Buffer, 0, ReadCount);
                ReadCount = Input.Read(Buffer, 0, Buffer.Length);
            }
        }
        // Код для выполнения при активации приложения (переводится в основной режим)
        // Этот код не будет выполняться при первом запуске приложения
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Код для выполнения при деактивации приложения (отправляется в фоновый режим)
        // Этот код не будет выполняться при закрытии приложения
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Код для выполнения при закрытии приложения (например, при нажатии пользователем кнопки "Назад")
        // Этот код не будет выполняться при деактивации приложения
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Код для выполнения в случае ошибки навигации
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Ошибка навигации; перейти в отладчик
                Debugger.Break();
            }
        }

        // Код для выполнения на необработанных исключениях
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // Произошло необработанное исключение; перейти в отладчик
                Debugger.Break();
            }
        }

        #region Инициализация приложения телефона

        // Избегайте двойной инициализации
        private bool phoneApplicationInitialized = false;

        // Не добавляйте в этот метод дополнительный код
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Создайте кадр, но не задавайте для него значение RootVisual; это позволит
            // экрану-заставке оставаться активным, пока приложение не будет готово для визуализации.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Обработка сбоев навигации
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Обработка запросов на сброс для очистки стека переходов назад
            RootFrame.Navigated += CheckForResetNavigation;

            // Убедитесь, что инициализация не выполняется повторно
            phoneApplicationInitialized = true;
        }

        // Не добавляйте в этот метод дополнительный код
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Задайте корневой визуальный элемент для визуализации приложения
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Удалите этот обработчик, т.к. он больше не нужен
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // Если приложение получило навигацию "reset", необходимо проверить
            // при следующей навигации, чтобы проверить, нужно ли выполнять сброс стека
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Отменить регистрацию события, чтобы оно больше не вызывалось
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Очистка стека только для "новых" навигаций (вперед) и навигаций "обновления"
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // Очистка всего стека страницы для согласованности пользовательского интерфейса
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // ничего не делать
            }
        }

        #endregion

        // Инициализация шрифта приложения и направления текста, как определено в локализованных строках ресурсов.
        //
        // Чтобы убедиться, что шрифт приложения соответствует поддерживаемым языкам, а
        // FlowDirection для каждого из этих языков соответствует традиционному направлению, ResourceLanguage
        // и ResourceFlowDirection необходимо инициализировать в каждом RESX-файле, чтобы эти значения совпадали с
        // культурой файла. Пример:
        //
        // AppResources.es-ES.resx
        //    Значение ResourceLanguage должно равняться "es-ES"
        //    Значение ResourceFlowDirection должно равняться "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     Значение ResourceLanguage должно равняться "ar-SA"
        //     Значение ResourceFlowDirection должно равняться "RightToLeft"
        //
        // Дополнительные сведения о локализации приложений Windows Phone см. на странице http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Задать шрифт в соответствии с отображаемым языком, определенным
                // строкой ресурса ResourceLanguage для каждого поддерживаемого языка.
                //
                // Откат к шрифту нейтрального языка, если отображаемый
                // язык телефона не поддерживается.
                //
                // Если возникла ошибка компилятора, ResourceLanguage отсутствует в
                // файл ресурсов.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Установка FlowDirection для всех элементов в корневом кадре на основании
                // строки ресурса ResourceFlowDirection для каждого
                // поддерживаемого языка.
                //
                // Если возникла ошибка компилятора, ResourceFlowDirection отсутствует в
                // файл ресурсов.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // Если здесь перехвачено исключение, вероятнее всего это означает, что
                // для ResourceLangauge неправильно задан код поддерживаемого языка
                // или для ResourceFlowDirection задано значение, отличное от LeftToRight
                // или RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}