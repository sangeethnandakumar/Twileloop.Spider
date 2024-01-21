using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace Twileloop.Spider
{
    public class Chrome : IBrowser, IDisposable
    {
        private readonly ChromeDriver _browser;

        internal Chrome(ChromeDriver browser)
        {
            _browser = browser;
        }

        public void Dispose()
        {
            _browser.Quit();
        }

        public void Goto(string url)
        {
            _browser.Navigate().GoToUrl(url);
        }

        public void SwitchBackFromPopup(string currentHandle)
        {
            _browser.Close();
            _browser.SwitchTo().Window(currentHandle);
        }

        public string SwitchOverToPopup(string xPath)
        {
            string currentHandle = _browser.CurrentWindowHandle;
            ReadOnlyCollection<string> originalHandles = _browser.WindowHandles;
            _browser.FindElement(By.XPath(xPath)).Click();

            WebDriverWait wait = new WebDriverWait(_browser, TimeSpan.FromSeconds(5));
            string popupWindowHandle = wait.Until<string>((d) =>
            {
                string foundHandle = null;
                List<string> newHandles = _browser.WindowHandles.Except(originalHandles).ToList();
                if (newHandles.Count > 0)
                {
                    foundHandle = newHandles[0];
                }
                return foundHandle;
            });
            _browser.SwitchTo().Window(popupWindowHandle);
            return currentHandle;
        }

        private IWebElement Element(string xpath)
        {
            var element = _browser.FindElement(By.XPath(xpath));
            return element;
        }

        public void Click(string xpath)
        {
            Element(xpath).Click();
        }

        public void SelectByText(string xpath, string optionText)
        {
            var select = new SelectElement(Element(xpath));
            select.SelectByText(optionText);
        }

        public void SelectByIndex(string xpath, int index)
        {
            var select = new SelectElement(Element(xpath));
            select.SelectByIndex(index);
        }

        public void SelectByValue(string xpath, string selection)
        {
            var select = new SelectElement(Element(xpath));
            select.SelectByValue(selection);
        }

        public void UploadFile(string fileSelectorId, string content, string filename, string type = "text/plain")
        {
            var script = @"const fileInput = document.getElementById('" + fileSelectorId + @"')
            const dataTransfer = new DataTransfer()
            const file = new File(['" + content + @"'], '" + filename + @"', {type: '" + type + @"'})
            dataTransfer.items.add(file)
            fileInput.files = dataTransfer.files";
            ((IJavaScriptExecutor)_browser).ExecuteScript(script);
        }

        public void ScrollTo(string xPath)
        {
            IJavaScriptExecutor je = (IJavaScriptExecutor)_browser;
            je.ExecuteScript("arguments[0].scrollIntoView(false);", Element(xPath));
        }

        public string ExecuteJavaScript(string script)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)_browser;
            var test = js.ExecuteScript(script);
            return (string)js.ExecuteScript(script);
        }

        public void InjectJavaScript(string scriptContent)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)_browser;
            var test = js.ExecuteScript(scriptContent);
            js.ExecuteScript(scriptContent);
        }

        public void Type(string xpath, string text)
        {
            var element = Element(xpath);
            element.Clear();
            element.SendKeys(text);
        }

        public void NaturalType(string xpath, string text)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var element = Element(xpath);
            foreach (var letter in text)
            {
                element.SendKeys(letter.ToString());
                Thread.Sleep(rnd.Next(0, 300));
            }
        }

        public string GetSource()
        {
            return _browser.PageSource;
        }

        public string GetOuterHTMLFromJQuerySelector(string jQuerySelector)
        {
            return ExecuteJavaScript($"return $('{jQuerySelector}').prop('outerHTML');");
        }

        public void UseJQuery()
        {
            InjectJavaScript(File.ReadAllText("BridgeFiles\\jquery.js"));
        }

        public void UseXPath()
        {
            InjectJavaScript(File.ReadAllText("BridgeFiles\\xPath.js"));
        }

        public string GetHTMLFromJQueryElement()
        {
            throw new NotImplementedException();
        }
    }
}
