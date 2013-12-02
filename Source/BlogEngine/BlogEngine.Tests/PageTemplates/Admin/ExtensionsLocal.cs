using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class ExtensionsLocal : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Extensions/default.cshtml"; }
        }
    }
}
