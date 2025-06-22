// File: Implementations/Browser.cs
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Twileloop.Spider.Implementations
{
    public class Browser : IBrowser
    {
        private readonly IWebDriver _webDriver;
        private readonly TimeSpan _defaultTimeout;
        private readonly WebDriverWait _wait;

        public Browser(IWebDriver webDriver, TimeSpan defaultTimeout)
        {
            _webDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
            _defaultTimeout = defaultTimeout;
            _wait = new WebDriverWait(_webDriver, _defaultTimeout);
        }

        public void Dispose()
        {
            try
            {
                _webDriver?.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error disposing browser: {ex.Message}");
            }
        }

        // Navigation Methods
        public void Goto(string url)
        {
            _webDriver.Navigate().GoToUrl(url);
        }

        public void GoBack()
        {
            _webDriver.Navigate().Back();
        }

        public void GoForward()
        {
            _webDriver.Navigate().Forward();
        }

        public void Refresh()
        {
            _webDriver.Navigate().Refresh();
        }

        // Element Interaction Methods
        public void Click(string xpath)
        {
            FindElement(xpath).Click();
        }

        public void Type(string xpath, string text)
        {
            var element = FindElement(xpath);
            element.Clear();
            element.SendKeys(text);
        }

        public void NaturalType(string xpath, string text, int minDelay = 50, int maxDelay = 200)
        {
            var element = FindElement(xpath);
            element.Clear();

            var random = new Random();
            foreach (var character in text)
            {
                element.SendKeys(character.ToString());
                Thread.Sleep(random.Next(minDelay, maxDelay));
            }
        }

        public void Clear(string xpath)
        {
            FindElement(xpath).Clear();
        }

        public void ScrollTo(string xpath)
        {
            var element = FindElement(xpath);
            ((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(false);", element);
        }

        // Form Control Methods
        public void SelectByText(string xpath, string optionText)
        {
            var select = new SelectElement(FindElement(xpath));
            select.SelectByText(optionText);
        }

        public void SelectByIndex(string xpath, int index)
        {
            var select = new SelectElement(FindElement(xpath));
            select.SelectByIndex(index);
        }

        public void SelectByValue(string xpath, string selection)
        {
            var select = new SelectElement(FindElement(xpath));
            select.SelectByValue(selection);
        }

        // File Operation Methods
        public void UploadFile(string xpath, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            FindElement(xpath).SendKeys(filePath);
        }

        public void UploadFileContent(string fileSelectorId, string content, string filename, string contentType = "text/plain")
        {
            var script = $@"
                const fileInput = document.getElementById('{fileSelectorId}');
                const dataTransfer = new DataTransfer();
                const file = new File(['{content}'], '{filename}', {{type: '{contentType}'}});
                dataTransfer.items.add(file);
                fileInput.files = dataTransfer.files;
            ";
            ((IJavaScriptExecutor)_webDriver).ExecuteScript(script);
        }

        // Window/Tab Management Methods
        public string SwitchToPopup(string triggerXPath)
        {
            string currentHandle = _webDriver.CurrentWindowHandle;
            ReadOnlyCollection<string> originalHandles = _webDriver.WindowHandles;

            Click(triggerXPath);

            string popupWindowHandle = _wait.Until(driver =>
            {
                var newHandles = driver.WindowHandles.Except(originalHandles).ToList();
                return newHandles.Count > 0 ? newHandles[0] : null;
            });

            _webDriver.SwitchTo().Window(popupWindowHandle);
            return currentHandle;
        }

        public void SwitchBackFromPopup(string originalHandle)
        {
            _webDriver.Close();
            _webDriver.SwitchTo().Window(originalHandle);
        }

        public void SwitchToTab(int tabIndex)
        {
            var handles = _webDriver.WindowHandles;
            if (tabIndex < 0 || tabIndex >= handles.Count)
                throw new ArgumentOutOfRangeException(nameof(tabIndex), "Tab index is out of range");

            _webDriver.SwitchTo().Window(handles[tabIndex]);
        }

        public void CloseCurrentTab()
        {
            _webDriver.Close();
        }

        // JavaScript Execution Methods
        public T ExecuteJavaScript<T>(string script)
        {
            var jsExecutor = (IJavaScriptExecutor)_webDriver;
            var result = jsExecutor.ExecuteScript(script);

            if (result == null)
                return default(T);

            if (result is T typedResult)
                return typedResult;

            if (typeof(T) == typeof(string))
                return (T)(object)result.ToString();

            if (result is string resultString)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(resultString);
                }
                catch
                {
                    return (T)Convert.ChangeType(resultString, typeof(T));
                }
            }

            try
            {
                return JsonSerializer.Deserialize<T>(result.ToString());
            }
            catch
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        public void InjectJavaScript(string scriptContent)
        {
            ((IJavaScriptExecutor)_webDriver).ExecuteScript(scriptContent);
        }

        public void UseJQuery()
        {
            var jqueryPath = Path.Combine("BridgeFiles", "jquery.js");
            if (File.Exists(jqueryPath))
            {
                var jqueryScript = File.ReadAllText(jqueryPath);
                InjectJavaScript(jqueryScript);
            }
            else
            {
                // Fallback to CDN
                InjectJavaScript(@"
                    if (typeof jQuery === 'undefined') {
                        var script = document.createElement('script');
                        script.src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js';
                        document.head.appendChild(script);
                    }
                ");
            }
        }

        public void UseXPath()
        {
            var xpathPath = Path.Combine("BridgeFiles", "xPath.js");
            if (File.Exists(xpathPath))
            {
                var xpathScript = File.ReadAllText(xpathPath);
                InjectJavaScript(xpathScript);
            }
        }

        // Element Information Methods
        public string GetText(string xpath)
        {
            return FindElement(xpath).Text;
        }

        public string GetAttribute(string xpath, string attributeName)
        {
            return FindElement(xpath).GetAttribute(attributeName);
        }

        public bool IsElementPresent(string xpath)
        {
            try
            {
                _webDriver.FindElement(By.XPath(xpath));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsElementVisible(string xpath)
        {
            try
            {
                return FindElement(xpath).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsElementEnabled(string xpath)
        {
            try
            {
                return FindElement(xpath).Enabled;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Page Information Methods
        public string GetPageSource()
        {
            return _webDriver.PageSource;
        }

        public string GetCurrentUrl()
        {
            return _webDriver.Url;
        }

        public string GetPageTitle()
        {
            return _webDriver.Title;
        }

        // Waiting Methods
        public void WaitForElement(string xpath, TimeSpan? timeout = null)
        {
            var wait = timeout.HasValue ? new WebDriverWait(_webDriver, timeout.Value) : _wait;
            wait.Until(driver => driver.FindElement(By.XPath(xpath)));
        }

        public void WaitForElementToBeClickable(string xpath, TimeSpan? timeout = null)
        {
            var wait = timeout.HasValue ? new WebDriverWait(_webDriver, timeout.Value) : _wait;
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xpath)));
        }

        public void WaitForPageToLoad()
        {
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        // Screenshot Methods
        public byte[] TakeScreenshot()
        {
            var screenshot = ((ITakesScreenshot)_webDriver).GetScreenshot();
            return screenshot.AsByteArray;
        }

        public void SaveScreenshot(string filePath)
        {
            var screenshot = TakeScreenshot();
            File.WriteAllBytes(filePath, screenshot);
        }

        // Private Helper Methods
        private IWebElement FindElement(string xpath)
        {
            return _webDriver.FindElement(By.XPath(xpath));
        }
    }
}