namespace Twileloop.Spider
{
    public interface IPopUp
    {
        string SwitchOverToPopup(string xPath);
        void SwitchBackFromPopup(string currentHandle);
    }

    public interface ISelect
    {
        void SelectByText(string xpath, string optionText);
        void SelectByIndex(string xpath, int index);
        void SelectByValue(string xpath, string selection);
    }

    public interface IScript
    {
        string ExecuteJavaScript(string script);
        void InjectJavaScript(string script);
        void UseJQuery();
        void UseXPath();
    }

    public interface IBrowser : IPopUp, ISelect, IScript, IDisposable
    {
        void Goto(string url);
        void Click(string xpath);
        void Type(string xpath, string text);
        void NaturalType(string xpath, string text);
        void ScrollTo(string xPath);
        void UploadFile(string fileSelectorId, string content, string filename, string type = "text/plain");
        string GetSource();
        string GetHTMLFromJQueryElement();
    }
}
