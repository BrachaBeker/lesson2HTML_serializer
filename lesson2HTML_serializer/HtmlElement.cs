using System;
using System.Collections.Generic;
using System.Text;

namespace lesson2HTML_serializer
{
    internal class HtmlElement
    {
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        // תוקן: Id הוא המזהה מתוך ה-attributes (id="main") ולכן string, ויכול לא להיות קיים
        public string? Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }

        public HtmlElement()
        {
            this.Children = new List<HtmlElement>();
            this.Attributes = new List<string>();
            this.Classes = new List<string>();
        }

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            q.Enqueue(this);
            while (q.Count > 0)
            {
                // תוקן: מוציאים אלמנט מהתור, מחזירים אותו, ומכניסים לתור את כל הילדים שלו
                HtmlElement current = q.Dequeue();
                yield return current;
                foreach (HtmlElement child in current.Children)
                    q.Enqueue(child);
            }
        }
        // דריסת ToString - כדי ש-Console.WriteLine(e) ידפיס את התגית עם הפרטים ולא את שם המחלקה
        public override string ToString()
        {
            return $"<{Name} id=\"{Id}\" class=\"{string.Join(" ", Classes)}\">";
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            // תוקן: אי אפשר לשנות את this - משתמשים במשתנה עזר שמתחיל מהאבא
            HtmlElement? current = this.Parent;
            // תוקן: התנאי צריך להיות bool - בודקים שהאלמנט לא null
            while (current != null)
            {
                // תוקן: מחזירים את האב הנוכחי (ולא את this) ועולים דור אחד למעלה
                yield return current;
                current = current.Parent;
            }
        }
    }

    // פונקציית הרחבה חייבת להיות במחלקה static - לכן היא כאן, מתחת ל-HtmlElement
    internal static class HtmlElementExtensions
    {
        // פונקציית ההרחבה - נקראת עם נקודה: root.FindElements(selector)
        public static IEnumerable<HtmlElement> FindElements(this HtmlElement element, Selector selector)
        {
            // HashSet - כדי שאותו אלמנט לא ייכנס לתוצאה פעמיים (אפשר להגיע אליו מכמה מסלולים)
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindElementsRecursive(element, selector, result);
            return result;
        }

        // הפונקציה הריקורסיבית - מקבלת אלמנט, סלקטור ואוסף שמחזיק את התוצאה
        private static void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            // כל הצאצאים של האלמנט הנוכחי, בלי האלמנט עצמו (Descendants מחזירה אותו ראשון)
            var descendants = element.Descendants().Skip(1);

            // סינון הצאצאים שעונים לקריטריונים של הסלקטור הנוכחי
            var filtered = descendants.Where(d => IsMatch(d, selector));

            // תנאי עצירה - הגענו לסלקטור האחרון, מוסיפים את הרשימה המסוננת לתוצאה
            if (selector.Child == null)
            {
                foreach (var match in filtered)
                    result.Add(match);
                return;
            }

            // הפעלת הריקורסיה על הרשימה המסוננת עם הסלקטור הבא
            foreach (var match in filtered)
                FindElementsRecursive(match, selector.Child, result);
        }

        // בדיקה אם אלמנט עונה לסלקטור - רק מאפיינים שמוגדרים בסלקטור נבדקים
        private static bool IsMatch(HtmlElement element, Selector selector)
        {
            if (selector.TagName != null && selector.TagName != element.Name)
                return false;
            if (selector.Id != null && selector.Id != element.Id)
                return false;
            // כל ה-classes של הסלקטור חייבים להופיע באלמנט
            if (!selector.Classes.All(c => element.Classes.Contains(c)))
                return false;
            return true;
        }
    }
}
