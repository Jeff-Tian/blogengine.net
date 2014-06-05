using System;
using NUnit.Framework;

namespace BlogEngine.Tests.Navigation
{
    [TestFixture]
    class SubBlogAggregation : BeTest
    {
        private static string TestFile = "";

        [SetUp]
        public void Init()
        {

        }

        [Test]
        [Category("primary")]
        public void MyTest()
        {
            Assert.IsTrue(1 == 1);
        }

        [TearDown]
        public void Dispose()
        {

        }
    }
}