using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class EditPage : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Pages/EditPage.aspx"; }
        }
    }
}
