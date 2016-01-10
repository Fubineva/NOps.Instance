using System.IO;
using System.Reflection;

static internal class TestHelpers
{
    public static string GetAppDir()
    {
        var appPath = Assembly.GetExecutingAssembly().Location;
        return Path.GetDirectoryName(appPath);
    }
}