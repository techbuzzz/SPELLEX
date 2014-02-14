using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SPEllex.Generic
{
    public static class DictionaryExtention
    {
        public static string ToJson(this IDictionary<string, string> dictionary)
        {
            var builder = new StringBuilder();
            builder.Append("{");
            foreach (var pair in dictionary)
            {
                builder.AppendFormat("{0}:'{1}',", pair.Key, pair.Value.ToUnicode());
            }
            builder.Remove(builder.Length - 1, 1);
            builder.Append("}");
            return builder.ToString();
        }

        public static string ToUnicode(this string srcText)
        {
            string str = string.Empty;
            char[] chArray = srcText.ToCharArray();
            return
                chArray.Select(t => Encoding.Unicode.GetBytes(t.ToString(CultureInfo.InvariantCulture)))
                    .Select(bytes => @"\u" + bytes[1].ToString("X2") + bytes[0].ToString("X2"))
                    .Aggregate(str, (current, str2) => current + str2);
        }
    }
}