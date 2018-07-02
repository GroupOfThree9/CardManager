using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CardManager
{
    class CheckBoxList
    {
        MainWindow mw;

        public List<CheckBox> CB_Llist;
        List<string> checkedGames;
        public Dictionary<string, List<string>> cards = new Dictionary<string, List<string>>();

        public CheckBoxList(MainWindow mw)
        {
            this.mw = mw;
        }

        private void DownloadGamesIcons(Login login)
        {
            foreach (string key in cards.Keys)
            {
                login.GetGamesIcons(key.TrimStart('_'));
            }
        }

        // Sort Games in gameBox by descending
        public void FillCheckBoxList(Login login, bool nightModeStyle)
        {            
            cards = login.GetBadges();
            CB_Llist = new List<CheckBox>();
            var result = cards.OrderByDescending(d => d.Value[2]).Select(d => new
            {
                Word = d.Key,
                Count = d.Value
            });
            Console.WriteLine(result.ToString());

            foreach (var item in result)
            {
                mw.Dispatcher.Invoke(() =>
                {
                    var cb = new CheckBox();
                    cb.Content = item.Count[0] + " - " + item.Count[1];
                    cb.Name = '_' + item.Word;
                    cb.Checked += new RoutedEventHandler(mw.CheckBox_Checked);
                    cb.Unchecked += new RoutedEventHandler(mw.CheckBox_UnChecked);
                    if (nightModeStyle)
                    {
                        cb.Foreground = new SolidColorBrush(Color.FromRgb(230, 230, 230));
                    }
                    CB_Llist.Add(cb);
                    mw.gameBox.Items.Add(cb);
                });
            }
            DownloadGamesIcons(login);
        }

        // Get games with checked CheckBoxes
        public List<string> GetCheckedGames()
        {
            checkedGames = new List<string>();
            foreach (var item in CB_Llist)
            {
                if (item.IsChecked == true)
                {
                    Console.WriteLine(item.Name.TrimStart('_'));
                    checkedGames.Add(item.Name.TrimStart('_'));
                }
            }
            return checkedGames;
        }

        public void UnMarkAll()
        {
            try
            {
                foreach (var checkbox in CB_Llist)
                {
                    checkbox.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void MarkAll()
        {
            try
            {
                UnMarkAll();
                if (CB_Llist.Count < 30)
                {
                    for (int i = 0; i < CB_Llist.Count; i++)
                    {
                        CB_Llist[i].IsChecked = true;
                    }
                }
                else
                {
                    for (int i = 0; i < 30; i++)
                    {
                        CB_Llist[i].IsChecked = true;
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void MakeUnable(int numberOfCheckedGames)
        {
            foreach (var item in CB_Llist)
            {
                if (item.IsChecked == false)
                {
                    item.IsEnabled = false;
                }
            }
        }

        public void MakeEnable(int numberOfCheckedGames)
        {
            foreach (var item in CB_Llist)
            {
                if (item.IsEnabled == false)
                {
                    item.IsEnabled = true;
                }
            }
        }

        public void UpdateGamesCounter(bool state)
        {
            var resultString_games = Regex.Match(mw.countOfSelectedGames.Content.ToString(), @"\d+").Value;
            int temp_games;
            if (state)
            {               
                temp_games = Convert.ToInt32(resultString_games) + 1;
            }
            else
            {                
                temp_games = Convert.ToInt32(resultString_games) - 1;
            }
            mw.countOfSelectedGames.Content = "Selected games: " + temp_games;
        }

        public void UpdateCardsCounter(bool state, object sender)
        {
            var resultString_cards = Regex.Match(mw.countOfCards.Content.ToString(), @"\d+").Value;
            var resultString_current_cards = Regex.Match((sender as CheckBox).Content.ToString().Split('-')[1], @"\d+").Value;
            int temp_cards;
            if (state)
            {
                temp_cards = Convert.ToInt32(resultString_cards) + Convert.ToInt32(resultString_current_cards);
            }
            else
            {
                temp_cards = Convert.ToInt32(resultString_cards) - Convert.ToInt32(resultString_current_cards);
            }
            mw.countOfCards.Content = "Possible cards drop:  " + temp_cards;
        }
    }
}
