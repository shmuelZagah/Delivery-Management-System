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

        /// <summary>
        /// Sorts a collection of items and groups them by a specified key.
        /// </summary>
        /// T = The type of items in the collection.
        /// TKey = The type of the key used for grouping.
        public static IEnumerable<T> SortWithGroupBy<T, TKey>( IEnumerable<T> items, 
            Func<T, TKey> keySelector)
        {
            return items
                .GroupBy(keySelector)      // קיבוץ לפי הקריטריון
                .OrderBy(g => g.Key)       // מיון הקבוצות לפי המפתח
                .SelectMany(g => g);       // החזרת כל הפריטים כסדרה ממוינת
        }


        #region Validtions

        //--------------
        //  Validtions
        //--------------
        internal static bool IdValidtion(int id)
        {
            return id.ToString().Length == 9;
        }

        internal static bool IsEmailValidManual(string? email)
        {
            // 1. בדיקה בסיסית: האם המחרוזת ריקה או מכילה רווחים?
            if (string.IsNullOrWhiteSpace(email) || email.Contains(' '))
            {
                return false;
            }

            // 2. בדיקת קיום סימן '@' אחד ויחיד
            int atIndex = email.IndexOf('@');
            int lastAtIndex = email.LastIndexOf('@');

            // חייב להכיל @, והוא חייב להיות הסימן היחיד
            if (atIndex == -1 || atIndex != lastAtIndex)
            {
                return false;
            }

            // 3. בדיקת מיקום הסימן '@'
            // אסור שיהיה תו ראשון או אחרון
            if (atIndex == 0 || atIndex == email.Length - 1)
            {
                return false;
            }

            // 4. בדיקת קיום סימן נקודה ('.') בדומיין (החלק שאחרי ה-@)
            string domainPart = email.Substring(atIndex + 1);
            int dotIndex = domainPart.IndexOf('.');

            // חייב להכיל נקודה אחת לפחות בדומיין
            if (dotIndex == -1)
            {
                return false;
            }

            // 5. בדיקת מיקום הנקודה בדומיין
            // אסור שהנקודה תהיה התו הראשון או האחרון בדומיין
            if (dotIndex == 0 || dotIndex == domainPart.Length - 1)
            {
                return false;
            }

            return true;
        }

    }
    #endregion


}
