using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class EditPost : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Posts/Add_entry.aspx"; }
        }

        public TextField PostTitle
        {
            get { return Document.TextField(Find.ById("ctl00_cphAdmin_txtTitle")); }
        }

        // tinyMCE uses frames to simulate text area, need javascript hack as workaround
        public string JsHack
        {
            get { return "document.getElementById('ctl00_cphAdmin_txtContent_TinyMCE1_txtContent_ifr').contentWindow.document.body.innerHTML = 'This is WATIN test post.';"; }
        }

        public void UploadImage(IE ie, string filePath)
        {
            ie.Link(Find.ById("uploadImage")).Click();

            var fu = ie.FileUpload(Find.ByClass("ImageUpload"));
            fu.Set(filePath);

            ie.Button(Find.ById("ctl00_cphAdmin_btnUploadImage")).Click();
            ie.WaitForComplete();
        }

        public void UploadFile(IE ie, string filePath)
        {
            ie.Link(Find.ById("uploadFile")).Click();

            var fu = ie.FileUpload(Find.ById("ctl00_cphAdmin_txtUploadFile"));
            fu.Set(filePath);

            ie.Button(Find.ById("ctl00_cphAdmin_btnUploadFile")).Click();
            ie.WaitForComplete();
        }

        public void UploadVideo(IE ie, string filePath)
        {
            ie.Link(Find.ById("uploadVideo")).Click();

            var fu = ie.FileUpload(Find.ById("ctl00_cphAdmin_txtUploadVideo"));
            fu.Set(filePath);

            ie.Button(Find.ById("ctl00_cphAdmin_btnUploadVideo")).Click();
            ie.WaitForComplete();
        }

        public Button Save
        {
            get { return Document.Button(Find.ById("btnSave")); }
        }
    }
}
