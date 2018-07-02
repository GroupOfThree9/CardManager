using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CardManager
{
    class GameEmulation
    {
        MainWindow mw;
        Serialization serialize = new Serialization();
        Login login = new Login();

        public GameEmulation(MainWindow mw)
        {
            this.mw = mw;
        }

        int step;
        int primal_step;
        int tail;
        int amount;
        int NumOfCardsBefore;
        int NumOfCardsAfter;
        int NumOfCardsEnd;
        int temp_cardstoday;
        int temp_cardsall;
        string timeToWait = "900";
        string timeToWaitNew = "4";
        string datePlusCards;
        string numCards;
        bool IsFirstTurn = true;

        Dictionary<string, List<string>> cards;

        Process cardManager = new Process();

        private void StartProcess(string item, string timeWait)
        {
            cardManager.StartInfo.FileName = "card_small.exe";
            cardManager.StartInfo.Arguments = item + " " + timeWait;
            cardManager.StartInfo.CreateNoWindow = true;
            cardManager.StartInfo.UseShellExecute = false;
            cardManager.Start();
            Thread.Sleep(1000);
        }

        private void GetAmount()
        {
            mw.Dispatcher.Invoke(() =>
            {
                amount = Convert.ToInt32(mw.chooseCycles.Text);
                Console.WriteLine(amount);
            });
        }

        private void UpdateStatLables(List<string> checkedGames, bool IsFirstTurn)
        {
            mw.Dispatcher.Invoke(() =>
            {
                mw.numberOfGamesNow.Content = "0";
                mw.abuseProgress.Value = 0;
                mw.numberOfGames.Content = checkedGames.Count;
                mw.percentProgress.Content = "0%";

                if (IsFirstTurn)
                {
                    mw.cyclesLeft.Content = amount - 1;
                    mw.currentPeriod.Content = "First";
                }

                else
                {
                    mw.currentPeriod.Content = "Second";
                }       
            });
        }

        private void UpdateGameLables(CheckBoxList checkboxlist, string item)
        {
            mw.Dispatcher.Invoke(() =>
            {
                mw.gameName.Content = checkboxlist.cards[item][0];

                var gameResult = Regex.Match(mw.numberOfGamesNow.Content.ToString(), @"\d+").Value;
                var temp_game = Convert.ToInt32(gameResult) + 1;
                mw.numberOfGamesNow.Content = temp_game;

                mw.abuseProgress.Value += step;

                var persentResult = Regex.Match(mw.percentProgress.Content.ToString(), @"\d+").Value;
                var temp_percent = Convert.ToInt32(persentResult) + step;
                mw.percentProgress.Content = temp_percent + "%";

                var src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(@"Pictures\" + item + ".jpg", UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                mw.currentGame.Source = src;
            });            
        }

        private void OpenTimer(Timer timer)
        {
            mw.Dispatcher.Invoke(() =>
            {
                timer.SetTimer();
            });
        }

        public void StartAllGames(CheckBoxList checkboxlist, List<string> checkedGames, Timer timer)
        {
            GetAmount();            

            Console.WriteLine(amount + "Before WHILE");
            while (amount != 0)
            {
                Console.WriteLine(amount + "IN WHILE");
                if (IsFirstTurn)
                {
                    NumOfCardsBefore = GetNumOfCadsBefore(checkedGames);
                    Console.WriteLine("Cards before " + NumOfCardsBefore);

                    UpdateStatLables(checkedGames, IsFirstTurn);

                    primal_step = 100 / checkedGames.Count;
                    tail = 100 % checkedGames.Count;

                    foreach (var item in checkedGames)
                    {
                        if (tail != 0)
                        {
                            step = primal_step + 1;
                            tail--;
                        }
                        else
                        {
                            step = primal_step;
                        }

                        UpdateGameLables(checkboxlist, item);

                        var thread2 = new Thread(() => StartProcess(item, timeToWait));
                        thread2.Start();
                        Thread.Sleep(3000);
                    }

                    OpenTimer(timer);

                    IsFirstTurn = false;
                    Thread.Sleep(905000);
                }
                else
                {
                    UpdateStatLables(checkedGames, IsFirstTurn);

                    primal_step = 100 / checkedGames.Count;
                    tail = 100 % checkedGames.Count;

                    foreach (var item in checkedGames)
                    {
                        if (tail != 0)
                        {
                            step = primal_step + 1;
                            tail--;
                        }
                        else
                        {
                            step = primal_step;
                        }

                        UpdateGameLables(checkboxlist, item);

                        var thread2 = new Thread(() => StartProcess(item, timeToWaitNew));
                        thread2.Start();
                        Thread.Sleep(8000);
                    }

                    IsFirstTurn = true;
                    amount--;
                    Console.WriteLine(amount + "AFTER -- IN WHILE");

                    NumOfCardsAfter = GetNumOfCadsAfter(checkedGames);
                    Console.WriteLine("Cards after " + NumOfCardsAfter);
                    NumOfCardsEnd = NumOfCardsBefore - NumOfCardsAfter;
                    Console.WriteLine("Cards end " + NumOfCardsEnd);

                    datePlusCards = serialize.GetNumberOfCardsToday();
                    numCards = serialize.GetNumberOfCards();

                    if(datePlusCards.Split('-')[0] == DateTime.Now.Date.ToString())
                    {                        
                        temp_cardstoday = Convert.ToInt32(datePlusCards.Split('-')[1]) + NumOfCardsEnd;
                        mw.Dispatcher.Invoke(() =>
                        {
                            mw.todayCardStat.Content = "Cards have dropped today: " + temp_cardstoday;
                        });
                        serialize.SaveNumberOfCardsToday(DateTime.Now.Date.ToString(), temp_cardstoday);
                    }
                    else
                    {
                        mw.Dispatcher.Invoke(() =>
                        {
                            mw.todayCardStat.Content = "Cards have dropped today: " + NumOfCardsEnd;
                        });
                        serialize.SaveNumberOfCardsToday(DateTime.Now.Date.ToString(), NumOfCardsEnd);
                    }

                    temp_cardsall = Convert.ToInt32(numCards) + NumOfCardsEnd;
                    mw.Dispatcher.Invoke(() =>
                    {
                        mw.allTimeCardStat.Content = "Cards dropped totally: " + temp_cardsall;
                    });
                    serialize.SaveNumberOfCards(temp_cardsall);
                }
            }
        }

        private int GetNumOfCadsAfter(List<string> checkedGames)
        {
            int NumOfCardsAfter = 0;
            login.GetPage();
            cards = login.GetBadges();
            foreach (var item in checkedGames)
            {
                if (!cards.ContainsKey(item))
                {
                    NumOfCardsAfter += 0;
                }
                else
                {
                    NumOfCardsAfter += Convert.ToInt32(cards[item][2]);
                }                
            }
            return NumOfCardsAfter;
        }

        private int GetNumOfCadsBefore(List<string> checkedGames)
        {
            int NumOfCardsBefore = 0;
            login.GetPage();
            cards = login.GetBadges();
            foreach (var item in checkedGames)
            {
                if (!cards.ContainsKey(item))
                {
                    NumOfCardsBefore += 0;
                }
                else
                {
                    NumOfCardsBefore += Convert.ToInt32(cards[item][2]);
                }
            }         
            return NumOfCardsBefore;
        }
    }
}
