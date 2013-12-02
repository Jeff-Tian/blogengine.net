using NUnit.Framework;
using WatiN.Core;
using BlogEngine.Tests.PageTemplates.Admin;

namespace BlogEngine.Tests.QuickNotes
{
    [TestFixture]
    public class Navigation : BeTest
    {

        [Test]
        public void AnonymousUserShouldNotSeeQuickNotesPanel()
        {
            Logout();
            ie.GoTo(Constants.Root);
            Assert.IsFalse(ie.Html.Contains("q-notes"));
        }

        [Test]
        public void AdminShouldSeeQuickNotesPanel()
        {
            Login("admin");
            ie.GoTo(Constants.Root);
            Assert.IsTrue(ie.Html.Contains("q-notes"));
        }

        [Test]
        public void ShouldBeAbleBrowseThroughTabs()
        {
            Login("admin");
            ie.GoTo(Constants.Root);

            ie.Link(Find.ById("open")).Click();

            ie.Link("q-alist").Click();
            Assert.IsTrue(ie.Html.Contains("You do not have any notes yet"));

            ie.Link("q-asettings").Click();
            Assert.IsTrue(ie.Html.Contains("Default post category"));

            ie.Link("q-alist").Click();
            Assert.IsTrue(ie.Html.Contains("You do not have any notes yet"));

            ie.Link("q-anote").Click();
            Assert.IsTrue(ie.Html.Contains("q-area rounded"));

            ie.Link("close").Click();
        }

        [TearDown]
        public void Dispose()
        {
            //var trash = ie.Page<Trash>();
            //ie.GoTo(trash.Url);
            //trash.PurgeAll.Click();
        }

    }
}
