using System;
using System.Text.RegularExpressions;

namespace Nanover.Frame.Import.CIF
{
    public static class CifUtility
    {
        private const string DataItemRegexString = @"^_(\w+).(\w+)(?:\s+(.*[^\s]))?";

        /// <summary>
        /// Get the category from a data item line.
        /// </summary>
        public static string GetCategory(string line)
        {
            var i = line.IndexOf(".", StringComparison.Ordinal);
            return line.Substring(1, i - 1);
        }

        /// <summary>
        /// Get the category, keyword and value from a data item line.
        /// </summary>
        public static (string Category, string Keyword, string value) GetCategoryAndKeyword(
            string line)
        {
            var match = Regex.Match(line, DataItemRegexString).Groups;
            return (match[1].Value, match[2].Value, match[3].Success ? match[3].Value : null);
        }
    }
}