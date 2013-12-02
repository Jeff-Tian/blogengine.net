using NUnit.Framework;
using BlogEngine.Tests.PageTemplates.Admin;

namespace BlogEngine.Tests.Packaging
{
    [TestFixture]
    public class Installer : BeTest
    {
        [SetUp]
        public void Init()
        {
            Login("admin");
        }

        [Test]
        [Category("primary")]
        [Category("online")]
        public void CanInstallAndUninstallTheme()
        {
            var gal = ie.Page<ThemesGallery>();

            ie.GoTo(gal.Url);

            TypeQuickly(gal.TxtSearch, "Boldy");

            gal.BtnSearch.Click();

            Wait(10);

            gal.ABoldy.Click();

            Wait(15);

            var loc = ie.Page<ThemesLocal>();

            ie.GoTo(loc.Url);

            ie.WaitUntilContainsText("Boldy");

            Assert.IsTrue(ie.ContainsText("Boldy"));

            ie.GoTo(loc.Url);

            loc.ABoldy.Click();

            Wait(10);

            Assert.IsFalse(ie.ContainsText("Boldy"));
        }

    }
}