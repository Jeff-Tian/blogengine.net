using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class Trash : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Trash.aspx"; }
        }
        public Button PurgeAll
        {
            get { return Document.Button(Find.ById("btnPurgeAll")); }
        }
    }
}
