namespace Twileloop.Spider.Extensions
{
    public static class BrowserExtensions
    {
        /// <summary>
        /// Checks if the browser is still running/alive
        /// </summary>
        /// <param name="browser">The browser instance</param>
        /// <returns>True if browser is running, false if closed or crashed</returns>
        public static bool IsRunning(this IBrowser browser)
        {
            try
            {
                // Try to get current URL - if browser is closed, this will throw an exception
                browser.GetCurrentUrl();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}