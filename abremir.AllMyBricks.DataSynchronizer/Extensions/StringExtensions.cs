using System;
using System.Text.RegularExpressions;

namespace abremir.AllMyBricks.DataSynchronizer.Extensions
{
    public static class StringExtensions
    {
        public static string SanitizeBricksetString(this string source)
        {
            return source.Replace("\n", "").Trim();
        }

        public static string SanitizeBricksetReview(this string source)
        {
            const string htmlBr = "<br>";

            var sanitized = Regex.Replace(source, @"(\n[ ]*)", htmlBr);

            if (sanitized.StartsWith(htmlBr, StringComparison.InvariantCultureIgnoreCase))
            {
                sanitized = sanitized.Substring(htmlBr.Length);
            }

            if (sanitized.EndsWith(htmlBr, StringComparison.InvariantCultureIgnoreCase))
            {
                sanitized = sanitized.Substring(0, sanitized.Length - htmlBr.Length);
            }

            return sanitized;
        }
    }
}