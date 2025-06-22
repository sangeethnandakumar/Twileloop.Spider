namespace Twileloop.Spider
{
    public interface IBrowser : IDisposable
    {
        // Navigation
        void Goto(string url);
        void GoBack();
        void GoForward();
        void Refresh();

        // Element Interaction
        void Click(string xpath);
        void Type(string xpath, string text);
        void NaturalType(string xpath, string text, int minDelay = 50, int maxDelay = 200);
        void Clear(string xpath);
        void ScrollTo(string xpath);

        // Form Controls
        void SelectByText(string xpath, string optionText);
        void SelectByIndex(string xpath, int index);
        void SelectByValue(string xpath, string selection);

        // File Operations
        void UploadFile(string xpath, string filePath);
        void UploadFileContent(string fileSelectorId, string content, string filename, string contentType = "text/plain");

        // Window/Tab Management
        string SwitchToPopup(string triggerXPath);
        void SwitchBackFromPopup(string originalHandle);
        void SwitchToTab(int tabIndex);
        void CloseCurrentTab();

        // JavaScript Execution
        T ExecuteJavaScript<T>(string script);
        void InjectJavaScript(string scriptContent);
        void UseJQuery();
        void UseXPath();

        // Element Information
        string GetText(string xpath);
        string GetAttribute(string xpath, string attributeName);
        bool IsElementPresent(string xpath);
        bool IsElementVisible(string xpath);
        bool IsElementEnabled(string xpath);

        // Page Information
        string GetPageSource();
        string GetCurrentUrl();
        string GetPageTitle();

        // Waiting
        void WaitForElement(string xpath, TimeSpan? timeout = null);
        void WaitForElementToBeClickable(string xpath, TimeSpan? timeout = null);
        void WaitForPageToLoad();

        // Screenshots
        byte[] TakeScreenshot();
        void SaveScreenshot(string filePath);
    }
}
