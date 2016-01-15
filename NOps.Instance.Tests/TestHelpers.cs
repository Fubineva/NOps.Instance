using System.IO;
using System.Reflection;

namespace NOps.Instance.Tests
{
    public static class TestHelpers
    {
        public static string GetAppDir()
        {
            var appPath = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(appPath);
        }
    }
}