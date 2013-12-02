using NUnit.Framework;
using WatiN.Core;

namespace BlogEngine.Tests.Quixote
{
    [TestFixture]
    public class Runner : BeTest
    {
        [SetUp]
        public void Init()
        {
            Login("admin");
        }

        [Test]
        [Category("primary")]
        public void RunPagerTests()
        {
            ie.GoTo(Constants.AppRoot + "/tests/pager.cshtml");
            Assert.IsTrue(ie.Html.Contains("class=\"pass\""));
            Assert.IsFalse(ie.Html.Contains("class=\"fail\""));
        }

        [Test]
        [Category("primary")]
        public void RunAvatarTests()
        {
            ie.GoTo(Constants.AppRoot + "/tests/avatar.cshtml");
            Assert.IsTrue(ie.Html.Contains("class=\"pass\""));
            Assert.IsFalse(ie.Html.Contains("class=\"fail\""));
        }

        [Test]
        [Category("online")]
        [Category("primary")]
        public void RunPackagingTests()
        {
            ie.GoTo(Constants.AppRoot + "/tests/packaging.cshtml");
            Assert.IsTrue(ie.Html.Contains("class=\"pass\""));
            Assert.IsFalse(ie.Html.Contains("class=\"fail\""));
        }

        [Test]
        public void RunUrlRewriteTests()
        {
            ie.GoTo(Constants.AppRoot + "/tests/urlrewrite.cshtml");
            Assert.IsTrue(ie.Html.Contains("class=\"pass\""));
            Assert.IsFalse(ie.Html.Contains("class=\"fail\""));
        }

        [Test]
        public void RunUrlRewriteNoExtensionsTests()
        {
            ie.GoTo(Constants.AppRoot + "/tests/urlrewrite.cshtml?ext=off");
            Assert.IsTrue(ie.Html.Contains("class=\"pass\""));
            Assert.IsFalse(ie.Html.Contains("class=\"fail\""));
        }
 
    }
}
