using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace lesson2HTML_serializer
{
    internal class HtmlSerializer
    {
        public static readonly string[] HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlTags.json")) ?? Array.Empty<string>();
        public static readonly string[] HtmlVoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlVoidTags.json")) ?? Array.Empty<string>();

        public async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }

        public HtmlElement Serialize(string html)
        {
            var htmlClean = new Regex("\\s").Replace(html, " ");

            var htmlLines = new Regex("<(.*?)>").Split(htmlClean).Where(s => !string.IsNullOrWhiteSpace(s));

            HtmlElement root = new HtmlElement { Name = "html" };

            HtmlElement thisHelement = root;

            foreach (var line in htmlLines)
            {
                var firstWord = line.Split(' ')[0];
                if (firstWord == "/html")
                    break;

                if (firstWord.StartsWith("/"))
                {
                    if (thisHelement.Parent != null)
                        thisHelement = thisHelement.Parent;
                    continue;
                }

                var tagName = firstWord.TrimEnd('/');

                if (HtmlSerializer.HtmlTags.Contains(tagName))
                {
                    var newElement = new HtmlElement { Parent = thisHelement, Name = tagName };
                    thisHelement.Children.Add(newElement);

                    var rest = line.Substring(firstWord.Length);
                    var attributes = new Regex("([^\\s].*?)=\"(.*?)\"").Matches(rest);
                    foreach (Match attribute in attributes)
                    {
                        var name = attribute.Groups[1].Value;
                        var value = attribute.Groups[2].Value;
                        newElement.Attributes.Add(attribute.Value);
                        if (name == "class")
                            newElement.Classes.AddRange(value.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                        if (name == "id")
                            newElement.Id = value;
                    }
                    bool isSelfClosing = line.EndsWith("/") || HtmlSerializer.HtmlVoidTags.Contains(tagName);

                    if (!isSelfClosing)
                        thisHelement = newElement;
                }
                else
                {
                    thisHelement.InnerHtml = line;
                }
            }

            return root;
        }
    }
}
