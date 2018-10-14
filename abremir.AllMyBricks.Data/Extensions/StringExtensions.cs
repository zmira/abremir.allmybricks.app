namespace abremir.AllMyBricks.Data.Extensions
{
    public static class StringExtensions
    {
        public static string Sanitize(this string source)
        {
            return source.Replace("'", "\\'").Trim();
        }
    }
}