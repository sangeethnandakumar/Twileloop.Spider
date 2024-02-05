using Twileloop.Spider;

using (IBrowser browser = new BrowserBuilder().SetUpChrome(
    openBrowser: true,
    incognito: true,
    killAllChromeDrivers: true,
    driver: "http://twileloop.com:4444"
    ))
{
    browser.UseJQuery();
    browser.UseXPath();

    //Code execution
    CodeExecutions(browser);

    browser.Goto("https://libgen.is");

    browser.NaturalType("//input[@id='searchform']", "rich dad poor dad");

    browser.Click("//input[@value='Search!']");

    Thread.Sleep(1000);
}


static void CodeExecutions(IBrowser browser)
{
    // Example 1: Execute JavaScript and get the result as string
    string resultAsString = browser.ExecuteJavaScript<string>("return 'Sangeeth' + ' ' + 'Nandakumar'");

    // Example 2: Execute JavaScript and get the result as an integer
    int resultAsInt = browser.ExecuteJavaScript<int>("return 21 + 2");
}