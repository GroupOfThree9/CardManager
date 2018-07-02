using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CardManager
{
    class Timer
    {
        MainWindow mw;

        private DispatcherTimer timer;

        private int hour;
        private int min;
        private int sec;

        public Timer(MainWindow mw)
        {
            this.mw = mw;
        }

        // set timer
        public void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.IsEnabled = true;
            hour = 0;
            min = 15;
            sec = 0;
        }

        // timer tick (1 sec)
        private void timer_Tick(object sender, EventArgs e)
        {
            sec = sec - 1;

            if (sec == -1)
            {
                min = min - 1;
                sec = 59;
            }

            if (min == -1)
            {
                hour = hour - 1;
                min = 59;
            }

            if (hour == 0 && min == 0 && sec == 0)
            {
                timer.IsEnabled = false;
            }
            mw.timerHour.Content = hour.ToString();
            mw.timerMin.Content = min.ToString();
            mw.timerSec.Content = sec.ToString();
        }
    }
}
