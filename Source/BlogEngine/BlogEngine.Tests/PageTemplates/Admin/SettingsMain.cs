using WatiN.Core;
using System.Text.RegularExpressions;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class SettingsMain : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Settings/Main.aspx"; }
        }

        //public bool EnableQuickNotes(IE ie)
        //{
        //    ie.GoTo(Url);
        //    var cbEnable = Document.CheckBox(Find.ById("ctl00_cphAdmin_cbEnableQuickNotes"));

        //    if (cbEnable != null)
        //    {
        //        if (!cbEnable.Checked)
        //        {
        //            cbEnable.Click();
        //            Save();
        //        }
        //        return true;
        //    }
        //    return false;
        //}

        public void Save()
        {
            Document.Button(Find.ById("btnSave")).Click();
        }
    }
}
