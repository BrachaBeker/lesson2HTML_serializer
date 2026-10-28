using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace lesson2HTML_serializer
{
    internal class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] HtmlTags { get; set; }
        public string[] HtmlVoidTags { get; set; }
        private HtmlHelper()
        {
            HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlTags.json")) ?? Array.Empty<string>();
            HtmlVoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlVoidTags.json")) ?? Array.Empty<string>();
        }
    }
}
