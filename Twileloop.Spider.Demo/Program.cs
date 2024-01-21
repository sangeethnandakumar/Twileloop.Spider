using Twileloop.Spider;

using (IBrowser browser = new BrowserBuilder().SetUpChrome(
    openBrowser: true,
    incognito: true
    ))
{
    browser.UseJQuery();
    browser.UseXPath();

    var test = browser.ExecuteJavaScript("return (1+1)");

    browser.Goto("https://libgen.is");

    browser.NaturalType("//input[@id='searchform']", "rich dad poor dad");

    browser.Click("//input[@value='Search!']");

    Thread.Sleep(1000);

}

