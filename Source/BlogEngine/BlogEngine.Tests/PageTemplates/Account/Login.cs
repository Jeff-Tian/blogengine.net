using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Account
{
    public class Login : Page
    {
        private const string userName = "UserName";
        private const string password = "Password";
        private const string loginButton = "LoginButton";
        private const string loginLinkId = "ctl00_aLogin";

        public string Url
        {
            get { return Constants.Root + "/Account/login.aspx"; }
        }

        public TextField UserName
        {
            get { return Document.TextField(Find.ById(userName)); }
        }

        public TextField Password
        {
            get { return Document.TextField(Find.ById(password)); }
        }

        public Button LoginButton
        {
            get { return Document.Button(Find.ById(loginButton)); }
        }

        public Link LogoffLink
        {
            get { return Document.Link(Find.ById(loginLinkId)); }
        }
    }
}
