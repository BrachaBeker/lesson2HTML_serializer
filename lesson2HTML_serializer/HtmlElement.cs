using System;
using System.Collections.Generic;
using System.Text;

namespace lesson2HTML_serializer
{
    internal class HtmlElement
    {
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
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
                HtmlElement current = q.Dequeue();
                yield return current;
                foreach (HtmlElement child in current.Children)
                    q.Enqueue(child);
            }
        }
        public override string ToString()
        {
            return $"<{Name} id=\"{Id}\" class=\"{string.Join(" ", Classes)}\">";
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement? current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }

    internal static class HtmlElementExtensions
    {
        public static IEnumerable<HtmlElement> FindElements(this HtmlElement element, Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindElementsRecursive(element, selector, result);
            return result;
        }

        private static void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            var descendants = element.Descendants().Skip(1);

            var filtered = descendants.Where(d => IsMatch(d, selector));

            if (selector.Child == null)
            {
                foreach (var match in filtered)
                    result.Add(match);
                return;
            }

            foreach (var match in filtered)
                FindElementsRecursive(match, selector.Child, result);
        }

        private static bool IsMatch(HtmlElement element, Selector selector)
        {
            if (selector.TagName != null && selector.TagName != element.Name)
                return false;
            if (selector.Id != null && selector.Id != element.Id)
                return false;
            if (!selector.Classes.All(c => element.Classes.Contains(c)))
                return false;
            return true;
        }
    }
}
