using System.Linq;

namespace Namotion.Storage
{
    public static class PathUtilities
    {
        public const string Delimiter = "/";

        public const char DelimiterChar = '/';

        public static string[] GetSegments(string path)
        {
            return path?
                .Replace('\\', '/')
                .Trim(DelimiterChar)
                .Split(DelimiterChar)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray() ?? new string[0];
        }
    }
}
