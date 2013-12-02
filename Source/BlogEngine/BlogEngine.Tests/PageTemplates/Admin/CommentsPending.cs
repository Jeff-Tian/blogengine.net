using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class CommentsPending : Page
    {
        public CheckBox CbSelectAll
        {
            get { return Document.CheckBox(Find.ById("selectall")); }
        }

        public Button BtnDelete
        {
            get { return Document.Button(Find.ById("btn-delete")); }
        }
    }
}
