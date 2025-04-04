using System;
using System.Text.RegularExpressions;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static partial class StringExtensions
    {
        public static string SanitizeBricksetString(this string source)
        {
            return source.Replace("\n", "").Trim();
        }

        public static string SanitizeBricksetReview(this string source)
        {
            const string htmlBr = "<br>";

            var sanitized = NewLineRegex().Replace(source, htmlBr);

            if (sanitized.StartsWith(htmlBr, StringComparison.InvariantCultureIgnoreCase))
            {
                sanitized = sanitized[htmlBr.Length..];
            }

            if (sanitized.EndsWith(htmlBr, StringComparison.InvariantCultureIgnoreCase))
            {
                sanitized = sanitized[..^htmlBr.Length];
            }

            return sanitized;
        }

        [GeneratedRegex(@"(\n[ ]*)")]
        private static partial Regex NewLineRegex();
    }
}
