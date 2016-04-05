using System.IO;
using System.Reflection;

namespace HostMe
{
    public static class PathNormalizer
    {
        private static readonly string RootPath;

        static PathNormalizer()
        {
            var codeBase = Assembly.GetExecutingAssembly().GetName().CodeBase;
            RootPath = Path.GetDirectoryName(codeBase).Replace(@"file:\", "");
        }

        public static string NormaliePath(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.Combine(RootPath, path);
        }
    }
}
