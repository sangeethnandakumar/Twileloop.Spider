using OpenQA.Selenium.Chrome;

namespace Twileloop.Spider
{
    public class BrowserBuilder
    {
        public IBrowser SetUpChrome(bool openBrowser = true, string binary = @"C:\Program Files\Google\Chrome\Application\chrome.exe")
        {
            return new Chrome(GetChromeDriver(openBrowser, binary));
        }

        private ChromeDriver GetChromeDriver(bool openBrowser, string bin)
        {
            var options = new ChromeOptions
            {
                BinaryLocation = bin,
                AcceptInsecureCertificates = true,
            };
            if (!openBrowser)
            {
                options.AddArguments("headless");
            }
            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            options.AddArgument("--no-sandbox");
            return new ChromeDriver(service, options);
        }
    }
}
