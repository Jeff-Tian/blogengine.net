using NUnit.Framework;
using BlogEngine.Tests.PageTemplates;
using BlogEngine.Tests.PageTemplates.Admin;
using WatiN.Core.Native.InternetExplorer;

namespace BlogEngine.Tests.Comments
{
    [TestFixture]
    public class Comment : BeTest
    {
        [SetUp]
        public void Init()
        {
            Logout();
        }

        [TearDown]
        public void Dispose()
        {
            Login("admin");
            Purge(ie);
        }

        [Test]
        public void CanAddUpdateAndDeleteComment()
        {          
           var post = ie.Page<SinglePost>();
           post.Load(ie);

           // clear up form
           ScrollToTxt(post.TxtContent);
           TypeQuickly(post.TxtName, "");
           TypeQuickly(post.TxtEmail, "");
           TypeQuickly(post.TxtContent, "");
           //TypeQuickly(post.TxtSimpleCaptcha, "");

           

           // test required fields
           post.BtnSave.Click();
           Assert.IsTrue(ie.Html.Contains("Required"));

           TypeQuickly(post.TxtName, "tester1");
           post.BtnSave.Click();
           Assert.IsTrue(ie.Html.Contains("Required"));

           TypeQuickly(post.TxtEmail, "tester1@us.com");
           post.BtnSave.Click();
           Assert.IsTrue(ie.Html.Contains("Required"));

           

          TypeQuickly(post.TxtContent, "This is a test comment by tester1");
          post.BtnSave.Click();
          ie.WaitForComplete();
          //Assert.IsTrue(ie.Html.Contains("Required"));

          if (ie.Elements.Exists("simpleCaptchaValue"))
          {
              TypeQuickly(post.TxtSimpleCaptcha, "20");
              post.BtnSave.Click();
              ie.WaitForComplete();
              Assert.IsTrue(ie.Html.Contains("The captcha value you provided is incorrect"));

              TypeQuickly(post.TxtSimpleCaptcha, "10");
              post.BtnSave.Click();
              ie.WaitUntilContainsText("Thank you for the feedback");
          }

          // check pending comments
          Login("admin");
          ie.GoTo(Constants.Root + "/admin/Comments/Pending.aspx");
          ie.WaitUntilContainsText("This is a test comment by tester1");

          var pending = ie.Page<CommentsPending>();
          pending.CbSelectAll.Checked = true;
          pending.BtnDelete.Click();

            Wait(3);
        }
    }
}
