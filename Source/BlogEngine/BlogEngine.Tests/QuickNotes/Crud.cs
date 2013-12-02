using NUnit.Framework;
using WatiN.Core;
using BlogEngine.Tests.PageTemplates.Admin;

namespace BlogEngine.Tests.QuickNotes
{
    [TestFixture]
    public class Crud : BeTest
    {
        [SetUp]
        public void Init()
        {
            Login("admin");
        }

        [Test]
        public void ShouldBeAbleToCreateUpdateAndDeleteNote()
        {         
            ie.GoTo(Constants.Root);
            ie.Link(Find.ById("open")).Click();

            // create
            TypeQuickly(ie.TextField("q-txtarea"), "Note one. This is a note one.");
            ie.Button("q-save").Click();
            ie.WaitForComplete();
            ie.Link("q-alist").Click();
            ie.WaitForComplete();
            Assert.IsTrue(ie.Html.Contains("Note one"));
            ie.SelectList("q-listbox").Options[0].Select();
            ie.WaitForComplete();
            Assert.IsTrue(ie.Html.Contains("q-txtarea"), "Text area not found on the page after selecting new note");

            // updated
            TypeQuickly(ie.TextField("q-txtarea"), "Note one. This is a note one. Updated.");
            ie.Button("q-save").Click();
            ie.WaitForComplete();
            ie.Link("q-alist").Click();
            Assert.IsTrue(ie.Html.Contains("This is a note one. Updated."));

            // delete
            ie.SelectList("q-listbox").Options[0].Select();
            ie.WaitForComplete();
            ie.Link("q-delete").Click();
            ie.WaitForComplete();
            ie.Link("q-alist").Click();
            Assert.IsTrue(ie.Html.Contains("You do not have any notes yet"));
        }
    }
}
