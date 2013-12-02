using System;
using NUnit.Framework;
using BlogEngine.Tests.FileSystem;

namespace BlogEngine.Tests.Navigation
{
    [TestFixture]
    class CustomPages : BeTest
    {
        private static string TestFile = "";

        [SetUp]
        public void Init()
        {
            TestFile = "<%@ Page Language=\"C#\" %>" + Environment.NewLine;
            TestFile += "<script runat=\"server\"></script>" + Environment.NewLine;
            TestFile += "<html xmlns=\"http://www.w3.org/1999/xhtml\">" + Environment.NewLine;
            TestFile += "<head runat=\"server\"><title></title></head><body><form id=\"form1\" runat=\"server\"><div>This is a test ASPX page.</div></form></body></html>";
        }

        [Test]
        [Category("primary")]
        public void CanNavigateToCustomAspxPage()
        {
            IO.MkFile("test.aspx", TestFile);
            ie.GoTo(Constants.Root + "/test.aspx");
            ie.WaitForComplete();
            Assert.IsTrue(ie.ContainsText("This is a test ASPX page."));
        }

        [Test]
        [Category("primary")]
        public void CanNavigateToDefaultAspxPageInSubDiretory()
        {
            IO.MkDir("User controls/test");
            IO.MkFile("User controls/test/Default.aspx", TestFile);

            ie.GoTo(Constants.Root + "/User controls/test/Default.aspx");
            ie.WaitForComplete();
            Assert.IsTrue(ie.ContainsText("This is a test ASPX page."));

            ie.GoTo(Constants.Root + "/User controls/test/");
            ie.WaitForComplete();
            Assert.IsTrue(ie.ContainsText("This is a test ASPX page."));
        }

        [TearDown]
        public void Dispose()
        {
            IO.DelFile("test.aspx");
            IO.DelFile("User controls/test/Default.aspx");
            IO.DelDir("User controls/test");
        }
    }
}
