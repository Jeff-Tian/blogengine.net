using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class ThemesGallery : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Extensions/Themes.cshtml?t=2"; }
        }

        public TextField TxtSearch
        {
            get { return Document.TextField(Find.ById("searchGallery")); }
        }

        public Button BtnSearch
        {
            get { return Document.Button(Find.ById("btnGalSearch")); }
        }

        public Link ABoldy
        {
            get { return Document.Link(Find.ById("a-Boldy")); }
        }
    }
}
