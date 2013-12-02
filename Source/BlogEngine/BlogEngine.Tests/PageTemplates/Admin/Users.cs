using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class Users : Page
    {
        /// <summary>
        /// page url
        /// </summary>
        public string Url { get { return Constants.Root + "/admin/Users/Users.aspx"; } }
        /// <summary>
        /// add new button
        /// </summary>
        public Link AddNew { get { return Document.Link(Find.ById("btnNew")); } }
        /// <summary>
        /// user name
        /// </summary>
        public TextField UserName { get { return Document.TextField(Find.ById("txtUserName")); } }
        /// <summary>
        /// email
        /// </summary>
        public TextField Email { get { return Document.TextField(Find.ById("txtEmail")); } }
        /// <summary>
        /// password
        /// </summary>
        public TextField Password { get { return Document.TextField(Find.ById("txtPassword")); } }
        /// <summary>
        /// confirm password
        /// </summary>
        public TextField Password2 { get { return Document.TextField(Find.ById("txtPassword2")); } }
        /// <summary>
        /// save new user
        /// </summary>
        public Button Save { get { return Document.Button(Find.ById("btnNewUser")); } }
        /// <summary>
        /// delete user link added to the table tools for test user
        /// </summary>
        public Link Delete { get { return Document.Link(Find.ById("del-testuser")); } }
    }
}
