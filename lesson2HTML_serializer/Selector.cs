using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace lesson2HTML_serializer
{
    internal class Selector
    {
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector()
        {
            Classes = new List<string>();
        }
        public static Selector FromString(string query)
        {
            string[] levelQuery = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Selector root=new Selector();
            Selector thisHelement = root;
            for (int i = 0; i < levelQuery.Length; i++)
            {
                string[] parts = Regex.Split(levelQuery[i], "(?=[#.])");

                foreach (string part in parts)
                {
                    if (part.Length == 0)
                        continue;
                    if (part.StartsWith("#"))
                        thisHelement.Id = part.Substring(1);
                    else if (part.StartsWith("."))
                        thisHelement.Classes.Add(part.Substring(1));
                    else if (HtmlSerializer.HtmlTags.Contains(part))
                        thisHelement.TagName = part;
                }
                if (i < levelQuery.Length - 1)
                {
                    Selector newSelector = new Selector();
                    newSelector.Parent = thisHelement;
                    thisHelement.Child = newSelector;
                    thisHelement = newSelector;
                }
            }
            return root;
        }
    }
}
