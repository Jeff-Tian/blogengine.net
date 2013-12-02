using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;
using BlogEngine.Tests.PageTemplates.Account;
using BlogEngine.Tests.PageTemplates.Admin;

namespace BlogEngine.Tests
{
    public abstract class BeTest
    {
        protected IE ie = null;

        [TestFixtureSetUp]
        public void SetUp()
        {
            ie = new IE();

            ie.Refresh();
            ie.ClearCache();

            Settings.WaitForCompleteTimeOut = 240;

            // to hide IE window
            // ie.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.Hide);
        }

        [STAThread]
        static void Main(string[] args)
        {

        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            ie.Close();
        }

        public void Login(string user, string pwd = "")
        {
            if (string.IsNullOrEmpty(pwd)) 
                pwd = user;

            var login = ie.Page<Login>();

            ie.GoTo(login.Url);

            TypeQuickly(login.UserName, user);
            TypeQuickly(login.Password, pwd);

            login.LoginButton.Click();
        }

        public void Logout()
        {
            ie.GoTo(Constants.Root);

            var login = ie.Page<Login>();
            var logOffLink = login.LogoffLink;

            if (logOffLink != null && logOffLink.Text == Constants.LogOff)
            {
                logOffLink.Click();
            }
        }

        public static void TypeQuickly(TextField textField, string text)
        {
            textField.SetAttributeValue("value", text);
        }

        public static object GetByRegex(string id, IE ie)
        {
            var reg = string.Format(@"[a-zA-Z0-9\$]*{0}", id);
            return Find.ById(new Regex(reg));
        }

        public static bool IsGUID(string expression)
        {
            if (expression != null)
            {
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

                return guidRegEx.IsMatch(expression);
            }
            return false;
        }

        public void Wait(int seconds)
        {
            int i;
            int.TryParse(string.Format("{0}000", seconds), out i);
            System.Threading.Thread.Sleep(i);
        }

        public void Purge(IE ie)
        {
            var trash = ie.Page<Trash>();
            ie.GoTo(trash.Url);
            ie.WaitForComplete();
            trash.PurgeAll.Click();
        }

        public void ScrollToTxt(TextField txt)
        {
            var nativeTxt = txt.NativeElement as IEElement;
            nativeTxt.AsHtmlElement.scrollIntoView();
        }
    }
}
