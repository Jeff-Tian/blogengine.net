using System.IO;
using System.Security.AccessControl;

namespace BlogEngine.Tests.FileSystem
{
    public class IO
    {
        public static void MkDir(string dir)
        {
            string path = Path.Combine(RootPath(), dir);

            if (!Directory.Exists((path)))
                Directory.CreateDirectory(path);
        }

        public static void DelDir(string dir)
        {
            string path = Path.Combine(RootPath(), dir);

            if (Directory.Exists((path)))
                Directory.Delete(path);
        }

        public static void MkFile(string file, string text)
        {
            var path = Path.Combine(RootPath(), file);

            if(!File.Exists(path))
            {
                using (var sw = File.CreateText(path))
                {
                    sw.Write(text);
                }
            }
        }

        public static void DelFile(string file)
        {
            var path = Path.Combine(RootPath(), file);

            if (File.Exists((path)))
                File.Delete(path);
        }

        public static string RootPath()
        {
            string s = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            s = s.Substring(0, s.IndexOf("BlogEngine.Tests"));
            s = s.Replace(@"file:///", "");
            s = s.Replace("/", @"\");
            s += @"BlogEngine.NET\";

            return s;
        }
    }
}
