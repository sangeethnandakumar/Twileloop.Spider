using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;

namespace Twileloop.Spider
{
    public class BrowserBuilder
    {
        public IBrowser SetUpChrome(bool openBrowser = true, bool incognito = true, string userProfilePath = null)
        {
            return new Chrome(GetChromeDriver(openBrowser, incognito, userProfilePath));
        }

        private ChromeDriver GetChromeDriver(bool openBrowser, bool incognito, string userProfilePath)
        {
            var options = new ChromeOptions
            {
                AcceptInsecureCertificates = true,
            };

            if (!string.IsNullOrEmpty(userProfilePath))
            {
                options.AddArguments($"--user-data-dir={userProfilePath}");
            }
            else if (incognito)
            {
                options.AddArguments("--incognito");
            }

            if (!openBrowser)
            {
                options.AddArguments("headless");
            }

            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            options.AddArgument("--no-sandbox");

            new DriverManager().SetUpDriver(new ChromeConfig());
            var newChrome = new ChromeDriver(service, options);
            return newChrome;
        }
    }


}
