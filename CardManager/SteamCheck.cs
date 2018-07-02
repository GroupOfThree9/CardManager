using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardManager
{
    class SteamCheck
    {
        MainWindow mw;
        bool steamIsRunning;

        public SteamCheck(MainWindow mw)
        {
            this.mw = mw;
        }

        // Check if steam is running
        private bool IsSteamRunning()
        {
            if (Process.GetProcessesByName("steam").Length == 0)
            {
                mw.Dispatcher.Invoke(() =>
                {
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(@"Pictures\Delete.png", UriKind.Relative);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    mw.steamIsRunningImage.Source = src;
                    mw.steamStatus.Content = "Steam client ISN'T running";
                });
                return false;
            }
            else
            {
                mw.Dispatcher.Invoke(() =>
                {
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(@"Pictures\Checkmark.png", UriKind.Relative);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    mw.steamIsRunningImage.Source = src;
                    mw.steamStatus.Content = "Steam client IS running";
                });
                return true;
            }
        }

        public void AllTimeSteamCheck()
        {
            steamIsRunning = IsSteamRunning();
            Console.WriteLine(steamIsRunning);
            if (steamIsRunning)
            {
                Thread.Sleep(3000);
                AllTimeSteamCheck();
            }
            while (!steamIsRunning)
            {
                steamIsRunning = IsSteamRunning();
                Console.WriteLine(steamIsRunning);
                if (steamIsRunning)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            AllTimeSteamCheck();
            Console.WriteLine("breyk prazuie");
        }
    }
}
