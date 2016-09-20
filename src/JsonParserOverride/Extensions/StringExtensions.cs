using System;
using System.Text.RegularExpressions;

namespace JsonParserOverride.Extensions
{
    public static class StringExtensions
    {
        public static int WordCount(string str)
        {
            return str.Split(new [] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        //"  I'm    wearing the   cheese.  It isn't wearing me!   ".TrimAndReduce()
        public static string TrimAndReduce(string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static string ToFirstUpperCase(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return char.ToUpperInvariant(value[0]) + value.Substring(1);
        }

        public static string ToFirstLowerCase(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
    }
}