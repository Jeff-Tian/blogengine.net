using System;
using System.IO;
using NUnit.Framework;
using BlogEngine.Tests.PageTemplates.Admin;

namespace BlogEngine.Tests.Posts
{
    [TestFixture]
    public class Post : BeTest
    {
        private const string TheTestPost = "The test post";
        private const string ImgFile = @"setup\TestFiles\sample.png";
        private const string TxtFile = @"setup\TestFiles\sample.txt";
        private const string FlvFile = @"setup\TestFiles\sample.flv";

        [SetUp]
        public void Init()
        {
            Login("admin");
        }

        [TearDown]
        public void Dispose()
        {
            var trash = ie.Page<Trash>();
            ie.GoTo(trash.Url);
            trash.PurgeAll.Click();

            var f = string.Format(@"App_Data\files\{0}\{1}\", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            Console.WriteLine(f);
            FileSystem.IO.DelFile(f + "sample.png");
            FileSystem.IO.DelFile(f + "sample.txt");
            FileSystem.IO.DelFile(@"media\sample.flv");
            FileSystem.IO.DelDir("media");
        }

        [Test]
        public void CanCreateAndDeletePost()
        {
            var editPost = ie.Page<EditPost>();

            ie.GoTo(editPost.Url);

            TypeQuickly(editPost.PostTitle, TheTestPost);

            // this will type test post into TinyMCE editor
            ie.Eval(editPost.JsHack);

            // upload image
            var filePath = Path.Combine(FileSystem.IO.RootPath(), ImgFile);
            editPost.UploadImage(ie, filePath);

            // upload file
            filePath = Path.Combine(FileSystem.IO.RootPath(), TxtFile);
            editPost.UploadFile(ie, filePath);

            // upload video
            filePath = Path.Combine(FileSystem.IO.RootPath(), FlvFile);
            editPost.UploadVideo(ie, filePath);
                        
            editPost.Save.Click();
            ie.GoTo(Constants.Root);

            ie.WaitUntilContainsText(TheTestPost);

            Assert.IsTrue(ie.Html.Contains("sample.png"));
            Assert.IsTrue(ie.Html.Contains("sample.txt"));
            Assert.IsTrue(ie.Html.Contains("sample.flv"));

            var canDelete = ie.Page<PostList>().DeletePostByTitle(TheTestPost, ie);
            Assert.IsTrue(canDelete, "Could not delete created post by title");

            Wait(3); // wait for ajax to complete. WaitForComplete not always working (((

            ie.GoTo(Constants.Root);

            ie.WaitForComplete();

            Assert.IsFalse(ie.ContainsText(TheTestPost));
        }
    }
}