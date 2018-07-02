using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CardManager
{
    [Serializable]
    class Serialization
    {
        string path = @"settings.dat";
        string todaystatpath = @"todaystat.dat";
        string allstatpath = @"stat.dat";
        BinaryFormatter formatter = new BinaryFormatter();

        public void SerializeSettings(bool currentState)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, currentState);
                    Console.WriteLine("NightMode settings are saved");
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool DeSerializeSettings()
        {
            bool currentState = false;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    currentState = (bool)formatter.Deserialize(fs);
                    Console.WriteLine("NightMode settings are loaded");
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return currentState;
        }

        public void SaveNumberOfCardsToday(string date ,int cardsNumber)
        {
            try
            {
                using (FileStream fs = new FileStream(todaystatpath, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, date + '-' + cardsNumber);
                    Console.WriteLine("Today stat is saved");
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveNumberOfCards(int cardsNumber)
        {
            try
            {
                using (FileStream fs = new FileStream(allstatpath, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, cardsNumber);
                    Console.WriteLine("All time stat is saved");
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetNumberOfCardsToday()
        {
            string datePlusCards = "0-0";
            try
            {
                using (FileStream fs = new FileStream(todaystatpath, FileMode.OpenOrCreate))
                {
                    datePlusCards = (string)formatter.Deserialize(fs);
                    Console.WriteLine("Today stat is loaded");
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return datePlusCards;
        }

        public string GetNumberOfCards()
        {
            int numCards = 0;
            try
            {
                using (FileStream fs = new FileStream(allstatpath, FileMode.OpenOrCreate))
                {
                    numCards = (Int32)formatter.Deserialize(fs);
                    Console.WriteLine("All stat is loaded");
                }
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return numCards.ToString();
        }
    }
}
