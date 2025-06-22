using System.Diagnostics;
using Twileloop.Spider;
using Twileloop.Spider.Extensions;

class Program
{
    static void Main(string[] args)
    {
        //EXAMPLES

        // Example 1: Just open a browser and do the stuff
        SimpleExample();

        // Example 2: OPen browser & run the stuff, But keep session alive, until user closes it
        SimpleExample(keepSessionAlive: true);

        //[STILL IN WORK]
        // Example 3: Open a browser with a user login, And do stuff
        NonIncognitoVersion();

        // Example 4: Open a browser with options, And do stuff
        AdvancedExample();

        // Example 5: Open a remote browser using remote connection with options, And do stuff
        RemoteDriverExample();
    }

    static void SimpleExample(bool keepSessionAlive = false)
    {
        Console.WriteLine("Running Simple Example...");

        using var browser = SpiderBuilder.Chrome();

        browser.UseJQuery();
        browser.Goto("https://www.google.com");
        browser.Type("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div[1]/div[2]/textarea", "Selenium automation");
        browser.Click("/html/body/div[1]/div[1]/div/div/header/div[1]/div[2]/a");

        if(keepSessionAlive)
        {
            //Keeing session alive until user closes it
            while (browser.IsRunning())
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Browser session ended by user.");
        }
        else
        {
            Thread.Sleep(2000);
        }

        Console.WriteLine($"Page title: {browser.GetPageTitle()}");
    }

    static void NonIncognitoVersion()
    {
        Console.WriteLine("Running Advanced Example...");

        // Kill all existing Chrome processes to avoid profile lock
        foreach (var process in Process.GetProcessesByName("chrome"))
        {
            try { process.Kill(); } catch { /* ignore access denied */ }
        }

        // List all Chrome profiles
        var chromeProfiles = SpiderBuilder.ListChromeProfiles();

        // Select default profile
        var defaultProfile = chromeProfiles.FirstOrDefault() ?? "Default";

        var profilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"Google\Chrome\User Data");

        var options = new BrowserOptions
        {
            OpenBrowser = true,
            Incognito = false,
            KillAllDrivers = true,
            DefaultTimeout = TimeSpan.FromSeconds(15),
            AdditionalArguments = new List<string>
        {
            "--disable-blink-features=AutomationControlled",
            $"--user-data-dir={profilePath}",
            $"--profile-directory={defaultProfile}",
            "--no-first-run",
            "--no-default-browser-check"
        }
        };

        using var browser = SpiderBuilder.Chrome(options);

        browser.UseJQuery();
        browser.Goto("https://www.google.com");
        browser.Type("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div[1]/div[2]/textarea", "Selenium automation");
        browser.Click("/html/body/div[1]/div[1]/div/div/header/div[1]/div[2]/a");
        Thread.Sleep(2000);

        Console.WriteLine($"Page title: {browser.GetPageTitle()}");

        // Wait until browser is closed
        while (browser.IsRunning())
        {
            Thread.Sleep(1000);
        }
    }


    static void AdvancedExample()
    {
        Console.WriteLine("Running Advanced Example...");

        var options = new BrowserOptions
        {
            OpenBrowser = true,
            Incognito = true,
            KillAllDrivers = true,
            DefaultTimeout = TimeSpan.FromSeconds(15),
            AdditionalArguments = new List<string> { "--disable-blink-features=AutomationControlled" }
        };

        using var browser = SpiderBuilder.Chrome(options);

        browser.UseJQuery();
        browser.Goto("https://www.google.com");
        browser.Type("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div[1]/div[2]/textarea", "Selenium automation");
        browser.Click("/html/body/div[1]/div[1]/div/div/header/div[1]/div[2]/a");
        Thread.Sleep(2000);

        Console.WriteLine($"Page title: {browser.GetPageTitle()}");
    }


    static void RemoteDriverExample()
    {
        Console.WriteLine("Running Remote Driver Example...");

        try
        {
            var options = new BrowserOptions
            {
                RemoteDriverUrl = "http://localhost:4444", // Selenium Grid URL
                OpenBrowser = false, // Headless for remote
                DefaultTimeout = TimeSpan.FromSeconds(30)
            };

            using var browser = SpiderBuilder.Chrome(options);

            browser.UseJQuery();
            browser.Goto("https://www.google.com");
            browser.Type("/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div[1]/div[2]/textarea", "Selenium automation");
            browser.Click("/html/body/div[1]/div[1]/div/div/header/div[1]/div[2]/a");
            Thread.Sleep(2000);

            Console.WriteLine($"Page title: {browser.GetPageTitle()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Remote driver not available: {ex.Message}");
        }
    }
}