using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System.Diagnostics;
using Twileloop.Spider.Implementations;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Twileloop.Spider.Factories
{
    public class BrowserFactory : IBrowserFactory
    {
        public IBrowser CreateBrowser(BrowserType browserType, BrowserOptions? options = null)
        {
            options ??= new BrowserOptions();

            if (options.KillAllDrivers)
            {
                KillAllDriverProcesses(browserType);
            }

            var webDriver = CreateWebDriver(browserType, options);
            return new Browser(webDriver, options.DefaultTimeout);
        }

        private IWebDriver CreateWebDriver(BrowserType browserType, BrowserOptions options)
        {
            if (!string.IsNullOrEmpty(options.RemoteDriverUrl))
            {
                return CreateRemoteWebDriver(browserType, options);
            }

            return browserType switch
            {
                BrowserType.Chrome => CreateChromeDriver(options),
                BrowserType.Edge => CreateEdgeDriver(options),
                BrowserType.Firefox => CreateFirefoxDriver(options),
                BrowserType.Safari => CreateSafariDriver(options),
                _ => throw new NotSupportedException($"Browser type {browserType} is not supported")
            };
        }

        private IWebDriver CreateChromeDriver(BrowserOptions options)
        {
            var chromeOptions = new ChromeOptions();
            ConfigureCommonOptions(chromeOptions, options);

            if (options.Incognito)
                chromeOptions.AddArgument("--incognito");

            if (!options.OpenBrowser)
                chromeOptions.AddArgument("--headless");

            var service = ChromeDriverService.CreateDefaultService();
            ConfigureDriverService(service);

            new DriverManager().SetUpDriver(new ChromeConfig());
            return new ChromeDriver(service, chromeOptions);
        }

        private IWebDriver CreateEdgeDriver(BrowserOptions options)
        {
            var edgeOptions = new EdgeOptions();
            ConfigureCommonOptions(edgeOptions, options);

            if (options.Incognito)
                edgeOptions.AddArgument("--inprivate");

            if (!options.OpenBrowser)
                edgeOptions.AddArgument("--headless");

            var service = EdgeDriverService.CreateDefaultService();
            ConfigureDriverService(service);

            new DriverManager().SetUpDriver(new EdgeConfig());
            return new EdgeDriver(service, edgeOptions);
        }

        private IWebDriver CreateFirefoxDriver(BrowserOptions options)
        {
            var firefoxOptions = new FirefoxOptions();

            if (options.AcceptInsecureCertificates)
                firefoxOptions.AcceptInsecureCertificates = true;

            if (options.Incognito)
                firefoxOptions.AddArgument("-private");

            if (!options.OpenBrowser)
                firefoxOptions.AddArgument("--headless");

            foreach (var arg in options.AdditionalArguments)
                firefoxOptions.AddArgument(arg);

            var service = FirefoxDriverService.CreateDefaultService();
            ConfigureDriverService(service);

            new DriverManager().SetUpDriver(new FirefoxConfig());
            return new FirefoxDriver(service, firefoxOptions);
        }

        private IWebDriver CreateSafariDriver(BrowserOptions options)
        {
            var safariOptions = new SafariOptions();

            if (options.AcceptInsecureCertificates)
                safariOptions.AcceptInsecureCertificates = true;

            return new SafariDriver(safariOptions);
        }

        private IWebDriver CreateRemoteWebDriver(BrowserType browserType, BrowserOptions options)
        {
            DriverOptions driverOptions;

            switch (browserType)
            {
                case BrowserType.Chrome:
                    driverOptions = CreateChromeOptionsForRemote(options);
                    break;
                case BrowserType.Edge:
                    driverOptions = CreateEdgeOptionsForRemote(options);
                    break;
                case BrowserType.Firefox:
                    driverOptions = CreateFirefoxOptionsForRemote(options);
                    break;
                default:
                    throw new NotSupportedException($"Remote browser type '{browserType}' is not supported.");
            }

            return new RemoteWebDriver(new Uri(options.RemoteDriverUrl!), driverOptions);
        }

        private ChromeOptions CreateChromeOptionsForRemote(BrowserOptions options)
        {
            var chromeOptions = new ChromeOptions();
            ConfigureCommonOptions(chromeOptions, options);

            if (options.Incognito)
                chromeOptions.AddArgument("--incognito");

            if (!options.OpenBrowser)
                chromeOptions.AddArgument("--headless");

            return chromeOptions;
        }

        private EdgeOptions CreateEdgeOptionsForRemote(BrowserOptions options)
        {
            var edgeOptions = new EdgeOptions();
            ConfigureCommonOptions(edgeOptions, options);

            if (options.Incognito)
                edgeOptions.AddArgument("--inprivate");

            if (!options.OpenBrowser)
                edgeOptions.AddArgument("--headless");

            return edgeOptions;
        }

        private FirefoxOptions CreateFirefoxOptionsForRemote(BrowserOptions options)
        {
            var firefoxOptions = new FirefoxOptions();

            if (options.AcceptInsecureCertificates)
                firefoxOptions.AcceptInsecureCertificates = true;

            if (options.Incognito)
                firefoxOptions.AddArgument("-private");

            if (!options.OpenBrowser)
                firefoxOptions.AddArgument("--headless");

            foreach (var arg in options.AdditionalArguments)
                firefoxOptions.AddArgument(arg);

            return firefoxOptions;
        }

        private void ConfigureCommonOptions(DriverOptions options, BrowserOptions browserOptions)
        {
            if (browserOptions.AcceptInsecureCertificates)
                options.AcceptInsecureCertificates = true;

            foreach (var arg in browserOptions.AdditionalArguments)
            {
                if (options is ChromeOptions chromeOptions)
                    chromeOptions.AddArgument(arg);
                else if (options is EdgeOptions edgeOptions)
                    edgeOptions.AddArgument(arg);
            }

            // Add experimental options for Chrome and Edge
            if (options is ChromeOptions chrome)
            {
                foreach (var option in browserOptions.ExperimentalOptions)
                    chrome.AddAdditionalOption(option.Key, option.Value);
            }
            else if (options is EdgeOptions edge)
            {
                foreach (var option in browserOptions.ExperimentalOptions)
                    edge.AddAdditionalOption(option.Key, option.Value);
            }
        }

        private void ConfigureDriverService(DriverService service)
        {
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
        }

        private void KillAllDriverProcesses(BrowserType browserType)
        {
            var processName = browserType switch
            {
                BrowserType.Chrome => "chromedriver",
                BrowserType.Edge => "msedgedriver",
                BrowserType.Firefox => "geckodriver",
                _ => null
            };

            if (processName != null)
            {
                KillProcesses(processName);
            }
        }

        private void KillProcesses(string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit(5000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to kill process {processName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error killing {processName} processes: {ex.Message}");
            }
        }
    }
}