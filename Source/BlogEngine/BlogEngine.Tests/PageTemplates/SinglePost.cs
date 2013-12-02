using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates
{
    public class SinglePost : Page
    {
        public void Load(IE ie)
        {
            ie.GoTo(Constants.Root);
            ie.Link(Find.ByClass("taggedlink")).Click();
            ie.Refresh();
        }

        public TextField TxtName
        {
            get { return Document.TextField(Find.ByClass("txt-user")); }
        }

        public TextField TxtEmail
        {
            get { return Document.TextField(Find.ByClass("txt-email")); }
        }

        public TextField TxtContent
        {
            get { return Document.TextField(Find.ByClass("txt-content")); }
        }

        public TextField TxtSimpleCaptcha
        {
            get { return Document.TextField(Find.ById("simpleCaptchaValue")); }
        }

        public Button BtnSave
        {
            get { return Document.Button(Find.ByClass("btn-save")); }
        }
 
    }
}
