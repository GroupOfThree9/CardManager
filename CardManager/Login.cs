using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Windows;
using CsQuery;
using OpenQA.Selenium.Chrome;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Diagnostics;

namespace CardManager
{
    [Serializable]
    class Login
	{        
        Dictionary<string, string> cookies = new Dictionary<string, string>(5);

        public ChromeDriverService Service { get; set; }
		public ChromeDriver Driver { get; set; }

        MainWindow mw;
        CookieContainer gaCookies;
        HttpWebRequest request;
        HttpWebResponse response;
        CQ cq;

        public Login()
        {
            gaCookies = new CookieContainer();            
        }

        public Login(MainWindow mw)
        {
            gaCookies = new CookieContainer();
            this.mw = mw;
        } 

        public void SetChromeDriver()
        {
            Service = ChromeDriverService.CreateDefaultService();

            // hide console 
            Service.HideCommandPromptWindow = true;

            // not hide DiagnosticInformation
            Service.SuppressInitialDiagnosticInformation = false;

            var options = new ChromeOptions();

            // disable infobar
            options.AddArguments("disable-infobars");

            Driver = new ChromeDriver(Service, options);

            Driver.Navigate().GoToUrl("https://steamcommunity.com/login/home/?goto=my/profile");
        }

        public bool CheckLogin()
        {
            // variable for checking correct login
            bool checkUrl = true;

            try
            {
                while (checkUrl)

                {

                    if (Driver.Url.Contains("https://steamcommunity.com/id/"))
                    {
                        foreach (var c in Driver.Manage().Cookies.AllCookies)
                        {
                            if (c.Name == "timezoneOffset" || c.Name == "_gid" || c.Name == "_ga")
                            {
                                continue;
                            }
                            if (cookies.ContainsKey(c.Name))
                            {
                                cookies[c.Name] = c.Value;
                            }
                            else
                            {
                                cookies.Add(c.Name, c.Value);
                            }
                            
                        }
                        checkUrl = false;
                        Driver.Close();
                        SaveCookiesToFile();
                        foreach (var process in Process.GetProcessesByName("chromedriver"))
                        {
                            process.Kill();
                        }
                    }
                    Thread.Sleep(500);
                }
            }

            catch (Exception)
            {
                Console.WriteLine(checkUrl);
                return checkUrl;                
            }

            return checkUrl;
        } 

		public void GetPage()
		{
            //string urlAddress = "https://steamcommunity.com/login/home/?goto=my/badges";
            string urlAddress = "https://steamcommunity.com/my/badges";
            // urlAddress = "http://steamcommunity.com/profiles/76561198318961215/badges";

            ReadCookiesFromFile();

            request = (HttpWebRequest)WebRequest.Create(urlAddress);

            request.CookieContainer = gaCookies;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.170 Safari/537.36 OPR/53.0.2907.99";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Host = "steamcommunity.com";

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                MessageBox.Show("Steam maybe is down, please, check https://steamstat.us/ for more info and reload the program later", "Network Error");
                mw.Dispatcher.Invoke(() =>
                {
                    Environment.Exit(0);
                });
            }            

            Console.WriteLine(request.Headers);            

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();               

                response.Close();
                readStream.Close();                

                cq = CQ.Create(data);                             
            }
            
        }

        public Dictionary<string, List<string>> GetBadges()
        {
            var cards = new Dictionary<string, List<string>>();
            string resultString;

            var rows = cq[".badge_row"].Contents()[".badge_row_inner"].Contents()[".badge_title_row"].Contents()
                         [".badge_title_stats"].Contents()[".badge_title_stats_content"].Contents()
                         [".badge_title_stats_drops"].Contents();                         
            Console.WriteLine(rows);

            foreach (var row in rows)
            {
                if (row.HasClass("progress_info_bold"))
                {
                    var result = WebUtility.HtmlDecode(row.InnerHTML);
                    Console.WriteLine(resultString = Regex.Match(result, @"\d+").Value);                    
                    if (!String.IsNullOrEmpty(resultString))
                    {
                        var games = new List<string>();
                        games.Add(row.ParentNode.ParentNode.ParentNode.ParentNode[3].InnerText.Split('&')[0].Trim()); // game name
                        games.Add(result); // cards remaining with text                           
                        Console.WriteLine(row.InnerHTML);
                        games.Add(resultString); // cards remaining
                        cards.Add(row.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.FirstElementChild.GetAttribute("href").Split('/')[6], games); // appid
                        Console.WriteLine(cards);
                    }                    
                }
            }
            return cards;
        }

        public string GetUserName()
        {
            foreach (var obj in cq.Find("span"))
            {
                if (obj.GetAttribute("class").Contains("pulldown global_action_link"))
                {
                    Console.WriteLine(obj.InnerHTML);
                    return obj.InnerHTML;
                }
            }
            return null;            
        }

        public bool GetUserIcon()
        {
            bool session = true;
            string[] tokens = new string[1];
            foreach (var obj in cq.Find("a"))
            {
                if (obj.GetAttribute("class").Contains("user_avatar playerAvatar"))
                {
                    Console.WriteLine(obj.InnerHTML);
                    tokens = obj.InnerHTML.Split('"');                   
                    Console.WriteLine(tokens[1]);                    
                }
            }

            try
            {
                using (var myWebClient = new WebClient())
                {
                    myWebClient.DownloadFile(tokens[1], @"Pictures\User1.png");
                }
            }
            catch
            {
                MessageBox.Show("Steam session is expired. Login into steam again, please");
                return !session;
            }
            return session;          
        }

        public void GetGamesIcons(string appId)
        {
            using (var myWebClient = new WebClient())
            {
                Console.WriteLine(appId);
                myWebClient.DownloadFile("https://steamcdn-a.akamaihd.net/steam/apps/" + appId + "/header.jpg", @"Pictures\" + appId + ".jpg");
            }
        }

        
        private void SaveCookiesToFile()
		{
			var path = @"cookies.dat";
            var formatter = new BinaryFormatter();
            try
			{
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, cookies);

                    Console.WriteLine("Объект сериализован");
                }
   
			}

			catch (IOException ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

        private void ReadCookiesFromFile()
        {
            var path = @"cookies.dat";           
            string[] tokens = new string[2];
            var formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    cookies = (Dictionary<string, string>)formatter.Deserialize(fs);

                    Console.WriteLine("Объект десериализован");
                    foreach (var item in cookies)
                    {
                        Console.WriteLine(item);
                        tokens = item.ToString().Split('[', ',', ']');
                        gaCookies.Add(new System.Net.Cookie(tokens[1], tokens[2], "/", "steamcommunity.com"));
                    }                    
                    Console.WriteLine("Объект десериализован");
                }           
            }

            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
	}
}
