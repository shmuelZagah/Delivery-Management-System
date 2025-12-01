using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    internal static class Tools
    {
        public static string ToStringProperty<T>(this T t)
        {
            if (t == null)
                return "";

            PropertyInfo[] properties = typeof(T).GetProperties();
            List<string> lines = new();

            foreach (var prop in properties)
            {
                object? value = prop.GetValue(t);

                // הדפסה יפה לרשימות
                if (value is System.Collections.IEnumerable list &&
                    value is not string)
                {
                    List<string> items = new();
                    foreach (var item in list)
                        items.Add(item?.ToString() ?? "null");

                    lines.Add($"{prop.Name}: [ {string.Join(", ", items)} ]");
                }
                else
                {
                    lines.Add($"{prop.Name}: {value}");
                }
            }

            return string.Join("\n", lines);
        }

        public static IEnumerable<T> SortWithGroupBy<T, TKey>( IEnumerable<T> items, 
            Func<T, TKey> keySelector)
        {
            return items
                .GroupBy(keySelector)      // קיבוץ לפי הקריטריון
                .OrderBy(g => g.Key)       // מיון הקבוצות לפי המפתח
                .SelectMany(g => g);       // החזרת כל הפריטים כסדרה ממוינת
        }

    }


}
