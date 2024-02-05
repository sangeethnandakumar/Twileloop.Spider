using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System.Diagnostics;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;

namespace Twileloop.Spider
{
    public class BrowserBuilder
    {
        public IBrowser SetUpChrome(bool openBrowser = true, bool incognito = true, bool killAllChromeDrivers = false, string driver = null)
        {
            if (killAllChromeDrivers)
            {
                KillAllChromeDrivers("taskkill", "/F /IM chromedriver.exe /T");
            }
            return new Chrome(GetWebDriver(openBrowser, incognito, driver));
        }

        private IWebDriver GetWebDriver(bool openBrowser, bool incognito, string driverUrl)
        {
            var options = new ChromeOptions
            {
                AcceptInsecureCertificates = true,
            };

            if (incognito)
            {
                options.AddArguments("--incognito");
            }

            if (!openBrowser)
            {
                options.AddArguments("headless");
            }

            IWebDriver webDriver;

            if (string.IsNullOrEmpty(driverUrl))
            {
                var service = ChromeDriverService.CreateDefaultService();
                service.SuppressInitialDiagnosticInformation = true;
                service.HideCommandPromptWindow = true;
                options.AddArgument("--no-sandbox");

                new DriverManager().SetUpDriver(new ChromeConfig());
                webDriver = new ChromeDriver(service, options);
            }
            else
            {
                webDriver = new RemoteWebDriver(new Uri(driverUrl), options);
            }

            return webDriver;
        }


        private void KillAllChromeDrivers(string command, string arguments)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = processInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    Console.WriteLine("Output:\n" + output);
                    Console.WriteLine("Error:\n" + error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }


}
