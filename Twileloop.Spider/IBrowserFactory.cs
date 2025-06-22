namespace Twileloop.Spider
{
    public interface IBrowserFactory
    {
        IBrowser CreateBrowser(BrowserType browserType, BrowserOptions? options = null);
    }
}
