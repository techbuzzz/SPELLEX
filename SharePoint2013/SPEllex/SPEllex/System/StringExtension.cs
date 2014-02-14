using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SPEllex.System
{
    public static class StringExtension
    {
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return (source.IndexOf(value, comparisonType) != -1);
        }

        public static bool ContainsAll(this string source, string[] allValues, StringComparison comparisonType)
        {
            if (allValues == null)
            {
                throw new ArgumentNullException("allValues");
            }
            return allValues.All(str => source.Contains(str, comparisonType));
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static bool IsDateTime(this string source)
        {
            DateTime time;
            return DateTime.TryParse(source, out time);
        }

        public static bool IsInt(this string source)
        {
            int num;
            return int.TryParse(source, out num);
        }

        public static bool IsMatch(this string source, string pattern)
        {
            return Regex.IsMatch(source, pattern);
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool IsWhiteSpace(this string source)
        {
            return ((source != null) && (source.Trim().Length == 0));
        }

        public static string Match(this string source, string pattern)
        {
            return Regex.Match(source, pattern).Value;
        }

        public static bool ToBoolean(this string value)
        {
            return value.ToBoolean(false);
        }

        public static bool ToBoolean(this string value, bool defaultValue)
        {
            bool result = false;
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            if (!bool.TryParse(value, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static DateTime ToDateTime(this string source)
        {
            return DateTime.Parse(source);
        }

        public static int ToInt32(this string source)
        {
            return int.Parse(source);
        }
    }
}