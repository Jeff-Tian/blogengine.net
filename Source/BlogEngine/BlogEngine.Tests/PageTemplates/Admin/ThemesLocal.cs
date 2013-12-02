using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class ThemesLocal : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Extensions/Themes.cshtml"; }
        }

        public Link ABoldy
        {
            get { return Document.Link(Find.ById("a-Boldy")); }
        }
    }
}
