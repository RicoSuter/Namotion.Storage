using System.Linq;

namespace Namotion.Storage
{
    public static class PathUtilities
    {
        public const string Delimiter = "/";

        public const char DelimiterChar = '/';

        public static string[] GetSegments(string path)
        {
            return path
                .Trim('/')
                .Split('/')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
        }
    }
}
