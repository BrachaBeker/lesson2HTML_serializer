using lesson2HTML_serializer;

HtmlSerializer serializer = new HtmlSerializer();
var html = await serializer.Load("https://getbootstrap.com/docs/3.4/examples/navbar/");
var dom = serializer.Serialize(html);

var result = dom.FindElements(Selector.FromString("ul.nav.navbar-nav"));

result.ToList().ForEach(e => Console.WriteLine(e.ToString()));

Console.ReadLine();
