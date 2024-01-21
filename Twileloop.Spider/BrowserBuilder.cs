using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using System.Diagnostics;

namespace Twileloop.Spider
{
    public class BrowserBuilder
    {
        public IBrowser SetUpChrome(bool openBrowser = true, bool incognito = true, bool killAllChromeDrivers = false)
        {
            if (killAllChromeDrivers)
            {
                Console.WriteLine("KILLING ALL CHROME DRIVERS");
                Console.WriteLine("___________________________________________________________");
                KillAllChromeDrivers("taskkill", "/F /IM chromedriver.exe /T");
                Console.WriteLine("___________________________________________________________");
            }
            return new Chrome(GetChromeDriver(openBrowser, incognito));
        }

        private ChromeDriver GetChromeDriver(bool openBrowser, bool incognito)
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

            var service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            options.AddArgument("--no-sandbox");

            new DriverManager().SetUpDriver(new ChromeConfig());
            var newChrome = new ChromeDriver(service, options);
            return newChrome;
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
