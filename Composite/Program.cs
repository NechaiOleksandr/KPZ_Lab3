using System.Text;

public enum DisplayType { Block, Inline }
public enum ClosingType { Normal, SelfClosing }

public abstract class LightNode
{
    public abstract string OuterHTML { get; }
    public abstract string InnerHTML { get; }
}

public class LightTextNode : LightNode
{
    private readonly string _text;
    public LightTextNode(string text) => _text = text;

    public override string InnerHTML => _text;
    public override string OuterHTML => _text;
}

public class LightElementNode : LightNode
{
    public string TagName { get; }
    public DisplayType Display { get; }
    public ClosingType Closing { get; }
    public List<string> CssClasses { get; } = new List<string>();
    private readonly List<LightNode> _children = new List<LightNode>();

    public LightElementNode(string tagName, DisplayType display, ClosingType closing)
    {
        TagName = tagName;
        Display = display;
        Closing = closing;
    }

    public void Add(LightNode node) => _children.Add(node);

    public int ChildrenCount => _children.Count;

    public override string InnerHTML
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            foreach (var child in _children)
            {
                sb.Append(child.OuterHTML);
            }
            return sb.ToString();
        }
    }

    public override string OuterHTML
    {
        get
        {
            string classes = CssClasses.Any() ? $" class=\"{string.Join(" ", CssClasses)}\"" : "";

            if (Closing == ClosingType.SelfClosing)
            {
                return $"<{TagName}{classes}>";
            }

            return $"<{TagName}{classes}>{InnerHTML}</{TagName}>";
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var container = new LightElementNode("div", DisplayType.Block, ClosingType.Normal);
        container.CssClasses.Add("container");
        container.CssClasses.Add("p-4");

        var title = new LightElementNode("h1", DisplayType.Block, ClosingType.Normal);
        title.Add(new LightTextNode("Мій список справ:"));

        var list = new LightElementNode("ul", DisplayType.Block, ClosingType.Normal);
        list.CssClasses.Add("todo-list");

        var item1 = new LightElementNode("li", DisplayType.Block, ClosingType.Normal);
        item1.Add(new LightTextNode("Вивчити патерн Компонувальник"));

        var item2 = new LightElementNode("li", DisplayType.Block, ClosingType.Normal);
        item2.Add(new LightTextNode("Здати лабораторну "));
        var icon = new LightElementNode("img", DisplayType.Inline, ClosingType.SelfClosing);
        icon.CssClasses.Add("icon-check");
        item2.Add(icon);

        list.Add(item1);
        list.Add(item2);
        container.Add(title);
        container.Add(list);

        Console.WriteLine("Візуалізація LightHTML");
        Console.WriteLine(container.OuterHTML);

        Console.WriteLine("\nСтатистика елемента <ul>");
        Console.WriteLine($"Кількість дочірніх елементів: {list.ChildrenCount}");

        Console.WriteLine("\nЛише InnerHTML списку");
        Console.WriteLine(list.InnerHTML);
    }
}