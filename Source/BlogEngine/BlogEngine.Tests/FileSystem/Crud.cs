using System.IO;
using System.Security.AccessControl;
using NUnit.Framework;

namespace BlogEngine.Tests.FileSystem
{
    public class Crud : BeTest
    {
        readonly string AppDataDir = Path.Combine(IO.RootPath(), "App_Data");
        readonly string AppCodeDir = Path.Combine(IO.RootPath(), "App_Code");

        [Test]
        [Category("primary")]
        public void CanWriteAndReadAppDataDirectory()
        {
            Assert.IsTrue(WriteReadDirectory(AppDataDir));
            Assert.IsTrue(DeleteDirectory(AppDataDir));
        }

        [Test]
        [Category("slow")]
        [Category("primary")]
        public void CanWriteAndReadAppCodeDirectory()
        {
            Assert.IsTrue(WriteReadDirectory(AppCodeDir));
            Assert.IsTrue(DeleteDirectory(AppCodeDir));

            // writing into code directory will
            // recycle application pool -
            // wait untill application back online
            ie.GoTo(Constants.Root);
            //ie.WaitForComplete();
            Wait(35);
        }

        #region Methods

        static bool WriteReadDirectory(string dir)
        {
            var testDir = Path.Combine(dir, "foo");
            var file = Path.Combine(testDir, "bar.txt");

            Directory.CreateDirectory(testDir);

            using (var sw = File.CreateText(file))
            {
                sw.WriteLine("The test");
            }

            if (!File.Exists(file))
                return false;

            using (var sr = File.OpenText(file))
            {
                if (sr.ReadToEnd() != "The test\r\n")
                    return false;
            }

            using (var sr = File.OpenText(file))
            {
                return (sr.ReadToEnd() == "The test\r\n");
            }
        }

        static bool DeleteDirectory(string dir)
        {
            File.Delete(Path.Combine(dir, "foo/bar.txt"));
            Directory.Delete(Path.Combine(dir, "foo"));

            return !Directory.Exists(Path.Combine(dir, "foo"));
        }

        #endregion
    }
}
