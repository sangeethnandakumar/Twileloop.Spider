using Twileloop.Spider;

using (IBrowser browser = new BrowserBuilder().SetUpChrome(true))
{
    browser.UseJQuery();
    browser.UseXPath();

    browser.Goto("https://libgen.is");

    browser.NaturalType("//input[@id='searchform']", "rich dad poor dad");

    browser.Click("//input[@value='Search!']");

    Thread.Sleep(5000);

}

