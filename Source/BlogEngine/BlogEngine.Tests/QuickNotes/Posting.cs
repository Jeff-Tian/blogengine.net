using NUnit.Framework;
using WatiN.Core;
using BlogEngine.Tests.PageTemplates.Admin;

namespace BlogEngine.Tests.QuickNotes
{
    [TestFixture]
    public class Posting : BeTest
    {
        [Test]
        public void PublishQuickNoteAsPost()
        {
            Login("admin");
            ie.GoTo(Constants.Root);
            ie.Link(Find.ById("open")).Click();

            // create a note
            TypeQuickly(ie.TextField("q-txtarea"), "Note to publish. This is a note that will be published.");
            ie.Button("q-save").Click();
            ie.Link("q-alist").Click();
            Assert.IsTrue(ie.Html.Contains("Note to publish"));
            ie.SelectList("q-listbox").Options[0].Select();
            Assert.IsTrue(ie.Html.Contains("q-txtarea"), "Text area not found on the page after selecting new note");

            // publish note
            ie.Link("q-publish").Click();
            ie.GoTo(Constants.Root + "/Account/login.aspx");
            ie.GoTo(Constants.Root);
            ie.WaitUntilContainsText("Note to publish");

            // clean up
            var postList = ie.Page<PostList>();
            Assert.IsTrue(postList.DeletePostByTitle("Note to publish", ie));
            Wait(5);
        }

        [TearDown]
        public void Dispose()
        {
            var trash = ie.Page<Trash>();
            ie.GoTo(trash.Url);
            trash.PurgeAll.Click();
        }
    }
}
