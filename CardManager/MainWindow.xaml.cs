using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace CardManager
{    
    public partial class MainWindow : Window
	{
        #region Private fields       

        string currentVersion = "1.0.0";

        int numberOfCheckedGames;
        string datePlusCards;
        string numCards;
        string cookiesPath = @"cookies.dat";

        NightMode night;
        CheckBoxList checkboxlist;
        SteamCheck steam;
        GameEmulation emulate;
        Timer timer;
        Serialization serialize = new Serialization();
        Login login;

        bool nightModeStyle = false;
        bool increase = true;
        bool decrease = false;
        List<string> checkedGames;
        Thread thread;
        Thread thread1;
        Thread thread2;
        Thread thread3;

        //Process main = new Process();

        #endregion

        public MainWindow()
		{
            login = new Login(this);
            night = new NightMode(this);
            checkboxlist = new CheckBoxList(this);
            steam = new SteamCheck(this);
            emulate = new GameEmulation(this);
            timer = new Timer(this);
            InitializeComponent();            
        }	

        private void DisableAllButtons()
        {
            startButton.IsEnabled = false;
            stopButton.IsEnabled = false;
            startAbuseButton.IsEnabled = false;
            markAll.IsEnabled = false;
            unMarkAll.IsEnabled = false;
            chooseCycles.IsEnabled = false;
            logout.IsEnabled = false;
        }

        private void EnableAllButtons()
        {
            startButton.IsEnabled = true;
            stopButton.IsEnabled = true;
            startAbuseButton.IsEnabled = true;
            markAll.IsEnabled = true;
            unMarkAll.IsEnabled = true;
            chooseCycles.IsEnabled = true;
            logout.IsEnabled = true;
        }

		// Set user image 
        private void SetUserImage()
        {
            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(@"Pictures\User1.png", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            usernameIcon.Source = src;
        }

		// Fast login, if we have cookies
        private void FastLogin()
        {            
            login.GetPage();
            this.Dispatcher.Invoke(() =>
            {
                if (login.GetUserIcon())
                {
                    usernameLable.Content = "Hi, " + login.GetUserName();
                    checkboxlist.FillCheckBoxList(login, nightModeStyle);
                    SetUserImage();
                    EnableAllButtons();
                    helpTips.Text = "Enjoy idling";
                }
                else
                {
                    System.IO.File.Delete(@"cookies.dat");
                    DisableAllButtons();
                    loginButton.IsEnabled = true;
                    helpTips.Text = "Login into Steam to continue";
                }
                
            });
        }

        private void DefaultLogin()
        {
            login.SetChromeDriver();
            if (!login.CheckLogin())
            {
                this.Dispatcher.Invoke(() =>
                {
                    loginButton.IsEnabled = false;
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    helpTips.Text = "Login into Steam to continue";
                });
                return;
            }
                
            login.GetPage();

            this.Dispatcher.Invoke(() =>
            {
                usernameLable.Content = "Hi, " + login.GetUserName();
            });

            checkboxlist.FillCheckBoxList(login, nightModeStyle);
            login.GetUserIcon();
            
            this.Dispatcher.Invoke(() =>
            {
                SetUserImage();
                EnableAllButtons();
                helpTips.Text = "Enjoy idling";
            });
        }

        // !!!!!!!! Button to do ALL from Login class
        private void loginButton_Click(object sender, RoutedEventArgs e)
		{
            helpTips.Text = "Loading badges....";
            thread3 = new Thread(DefaultLogin);
            thread3.IsBackground = true;
            thread3.Start();            
        } 

        // start abuse in Abuse class
        private void startAbuseButton_Click(object sender, RoutedEventArgs e)
        {            
            checkedGames = checkboxlist.GetCheckedGames();            
            if (checkedGames.Count != 0)
            {
                thread1 = new Thread(() => emulate.StartAllGames(checkboxlist, checkedGames, timer));
                thread1.Start();                
            }
            else
            {
                MessageBox.Show("0 games to start, please, mark some games to start", "CardManager");
            }
        }

		// Button to do fast login
        private void startButton_Click(object sender, RoutedEventArgs e)
		{
            stopButton_Click(sender, e);
            login.GetPage();
            checkboxlist.FillCheckBoxList(login, nightModeStyle);
        }

		// Button to clear window
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            gameBox.Items.Clear();
            checkboxlist.CB_Llist.Clear();
            countOfSelectedGames.Content = "Selected games: " + 0;
            countOfCards.Content = "Possible cards drop:  " + 0;
        }

        // wait until window is full rendered -> then check if Steam is running, load settings DONE
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            currentVersionText.Content = "Version: " + currentVersion;
            DisableAllButtons();

            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(@"Pictures\User.png", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            usernameIcon.Source = src;

            src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(@"Pictures\Medal.png", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            medal.Source = src;

            src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(@"Pictures\support-us-on-patreon-large.jpg", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            patreon.Source = src;

            if (!File.Exists("settings.dat"))
            {                
                serialize.SerializeSettings(true);
            }

            nightModeStyle = serialize.DeSerializeSettings();

            if (nightModeStyle)
            {                
                nightMode.IsChecked = true;
            }

            if (!File.Exists("todaystat.dat"))
            {
                serialize.SaveNumberOfCardsToday(DateTime.Now.Date.ToString(), 0);
            }
            if (!File.Exists("stat.dat"))
            {
                serialize.SaveNumberOfCards(0);
            }

            datePlusCards = serialize.GetNumberOfCardsToday();
            numCards = serialize.GetNumberOfCards();

            if (datePlusCards.Split('-')[0] == DateTime.Now.Date.ToString())
            {
                todayCardStat.Content = "Cards have dropped today: " + datePlusCards.Split('-')[1];
            }
            else
            {
                todayCardStat.Content = "Cards have dropped today: " + "0";
                serialize.SaveNumberOfCardsToday(DateTime.Now.Date.ToString(), 0);
            }

            allTimeCardStat.Content = "Cards dropped totally: " + numCards;
                        
            currentGame.Source = null;

            thread = new Thread(steam.AllTimeSteamCheck);
            thread.IsBackground = true;
            thread.Start();

            if (File.Exists(cookiesPath))
            {
                loginButton.IsEnabled = false;
                helpTips.Text = "Loading badges....";
                thread2 = new Thread(FastLogin);
                thread2.IsBackground = true;
                thread2.Start();                
            }
            else
            {
                DisableAllButtons();
            }            
        }

        //unmark all games DONE
        private void unMarkAll_Click(object sender, RoutedEventArgs e)
        {
            checkboxlist.UnMarkAll();
        }

        //mark all games in list but not more than 30 DONE
        private void markAll_Click(object sender, RoutedEventArgs e)
        {
            checkboxlist.MarkAll();
        }

        // handler if checkbox is checked DONE
        internal void CheckBox_Checked(object sender, EventArgs e)
        {
            numberOfCheckedGames++;
            if (numberOfCheckedGames == 30)
            {
                checkboxlist.MakeUnable(numberOfCheckedGames);
            }

            // update counter of checked games
            checkboxlist.UpdateGamesCounter(increase);

            //update count of checked cards
            checkboxlist.UpdateCardsCounter(increase, sender);
        }

        // handler if checkbox is unchecked DONE
        internal void CheckBox_UnChecked(object sender, EventArgs e)
        {
            numberOfCheckedGames--;
            if (numberOfCheckedGames == 29)
            {
                checkboxlist.MakeEnable(numberOfCheckedGames);
            }

            // update counter of checked games
            checkboxlist.UpdateGamesCounter(decrease);

            //update count of checked cards
            checkboxlist.UpdateCardsCounter(decrease, sender);
        }

        // event for switching on night mode DONE
        private void nightMode_Checked(object sender, RoutedEventArgs e)
        {
            nightModeStyle = true;
            night.NightModeOn();
            serialize.SerializeSettings(nightModeStyle);
        }

        // event for switching off night mode DONE
        private void nightMode_UnChecked(object sender, RoutedEventArgs e)
        {
            nightModeStyle = false;
            night.NightModeOff();
            serialize.SerializeSettings(nightModeStyle);
        }

        // exit function from menu DONE
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {            
            System.Windows.Application.Current.Shutdown();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.Delete(@"cookies.dat");
            double x = Top;
            double y = Left;

            MainWindow win2 = new MainWindow();
            win2.Top = x; win2.Left = y;
            win2.Show();
            Close();

            //main.StartInfo.FileName = "CardManager.exe";
            //main.Start();
            //System.Windows.Application.Current.Shutdown();
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("CardManager was created by GroupOfThree", "Credits");
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This program is free software: you can redistribute it and/or" +
                " modify it under the terms of the GNU General Public License as published by" +
                " the Free Software Foundation. A copy of the GNU General Public License can" +
                " be found at http://www.gnu.org/licenses/. For your convenience, a copy of this" +
                " license is included.", "License");
        }

        private void Version_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(currentVersion, "Version");
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
