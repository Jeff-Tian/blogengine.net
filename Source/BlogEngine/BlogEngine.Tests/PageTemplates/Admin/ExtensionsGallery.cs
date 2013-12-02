using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class ExtensionsGallery : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Extensions/Extensions.cshtml"; }
        }
    }
}
