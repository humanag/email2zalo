using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace Email2Zalo
{
    public class ZaloBusiness : IDisposable
    {
        public static void ZaloDispose()
        {
            if (_instance != null)
                _instance.Dispose();
        }

        private static ZaloBusiness _instance = null;
        public static ZaloBusiness Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ZaloBusiness();

                return _instance;
            }
        }

        public string Contact { get; set; }
        public string IdContactSearch { get; set; } = "contact-search-input";
        public string IdInput { get; set; } = "richInput";

        private string _Url = null;
        public string ZaloUrl {
            get {
                if (_Url == null)
                    _Url = "https://id.zalo.me/account/login?continue=https%3A%2F%2Fchat.zalo.me%2F";

                return _Url;
            }
            set { _Url = value; }
        }

        public void OpenUrl()
        {
            Driver.Navigate().GoToUrl(ZaloUrl);
            Thread.Sleep(10000);
        }

        public bool SendMessage(string pMessage)
        {
            var element = Driver.FindElementById(IdContactSearch);
            element.SendKeys(Contact);
            Thread.Sleep(2000);
            element.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(2000);

            element = Driver.FindElementById(IdInput);            
            element.SendKeys(pMessage);
            Thread.Sleep(2000);
            element.SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(2000);

            return true;
        }

        private ChromeDriver _driver = null;
        private ChromeDriver Driver
        {
            get
            {
                if (_driver == null)
                {
                    ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                    chromeDriverService.HideCommandPromptWindow = true;
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--start-maximized");
                    chromeOptions.AddArgument("--disable-infobars");
                    chromeOptions.AddArgument("--disable-notifications");
                    _driver = new ChromeDriver(chromeDriverService, chromeOptions);                    
                }                    

                return _driver;
            }
        }

        public void Dispose()
        {
            if (_driver != null)
            {                
                _driver.Close();
                _driver.Quit();
                _driver.Dispose();
            }
                
        }
    }
}
