// File: SpiderBuilder.cs (Main Entry Point)
using Twileloop.Spider.Factories;

namespace Twileloop.Spider
{
    public static class SpiderBuilder
    {
        private static readonly IBrowserFactory _browserFactory = new BrowserFactory();

        public static IBrowser Chrome(BrowserOptions? options = null)
        {
            return _browserFactory.CreateBrowser(BrowserType.Chrome, options);
        }

        public static IBrowser Edge(BrowserOptions? options = null)
        {
            return _browserFactory.CreateBrowser(BrowserType.Edge, options);
        }

        public static IBrowser Firefox(BrowserOptions? options = null)
        {
            return _browserFactory.CreateBrowser(BrowserType.Firefox, options);
        }

        public static IBrowser Safari(BrowserOptions? options = null)
        {
            return _browserFactory.CreateBrowser(BrowserType.Safari, options);
        }

        public static IBrowser Create(BrowserType browserType, BrowserOptions? options = null)
        {
            return _browserFactory.CreateBrowser(browserType, options);
        }

        public static List<string> ListChromeProfiles()
        {
            string userDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Google", "Chrome", "User Data"
            );

            if (Directory.Exists(userDataDir))
            {
                var profiles = Directory.GetDirectories(userDataDir)
                    .Where(dir => Path.GetFileName(dir).StartsWith("Profile") || Path.GetFileName(dir) == "Default")
                    .Select(dir => Path.GetFileName(dir))
                    .ToList();

                return profiles;
            }
            return new List<string>
            {
                "Default" // Default profile if no other profiles found
            };
        }
    }
}