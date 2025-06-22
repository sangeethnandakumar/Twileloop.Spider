namespace Twileloop.Spider
{
    public class BrowserOptions
    {
        public bool OpenBrowser { get; set; } = true;
        public bool Incognito { get; set; } = true;
        public bool KillAllDrivers { get; set; } = false;
        public string? RemoteDriverUrl { get; set; }
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
        public bool AcceptInsecureCertificates { get; set; } = true;
        public List<string> AdditionalArguments { get; set; } = new List<string>();
        public Dictionary<string, object> ExperimentalOptions { get; set; } = new Dictionary<string, object>();
    }
}
