using System;

namespace GenX.Cli.Infrastructure
{
    public static class StringExtensions
    {
        private static char[] _compressedStringSeparators = new char[] { ' ', '-', '_' };

        public static string ToCamelCase(this string source) =>
            source.Length > 0 
                ? source.Substring(0, 1).ToLower() + source.Substring(1, source.Length - 1) 
                : source;

        public static string ToCompressedString(this string source) =>
            source.Replace(_compressedStringSeparators, string.Empty);
 
        public static string Replace(this string source, char[] separators, string newVal)
        {
            string[] temp = source.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(newVal, temp);
        }
    }
}
