using System.Text;

public class LightElementInfo
{
    public string TagName { get; }
    public DisplayType Display { get; }
    public ClosingType Closing { get; }

    public LightElementInfo(string tagName, DisplayType display, ClosingType closing)
    {
        TagName = tagName;
        Display = display;
        Closing = closing;
    }
}

public class FlyweightFactory
{
    private readonly Dictionary<string, LightElementInfo> _types = new Dictionary<string, LightElementInfo>();

    public LightElementInfo GetType(string tagName, DisplayType display, ClosingType closing)
    {
        string key = $"{tagName}_{display}_{closing}";
        if (!_types.ContainsKey(key))
        {
            _types[key] = new LightElementInfo(tagName, display, closing);
        }
        return _types[key];
    }
}

public enum DisplayType { Block, Inline }
public enum ClosingType { Normal, SelfClosing }

public abstract class LightNode
{
    public abstract string OuterHTML { get; }
}

public class LightTextNode : LightNode
{
    private readonly string _text;
    public LightTextNode(string text) => _text = text;
    public override string OuterHTML => _text;
}

public class LightElementNode : LightNode
{
    private readonly LightElementInfo _info;
    public List<string> CssClasses { get; } = new List<string>();
    private readonly List<LightNode> _children = new List<LightNode>();

    public LightElementNode(LightElementInfo info)
    {
        _info = info;
    }

    public void Add(LightNode node) => _children.Add(node);

    public override string OuterHTML
    {
        get
        {
            string classes = CssClasses.Any() ? $" class=\"{string.Join(" ", CssClasses)}\"" : "";
            StringBuilder inner = new StringBuilder();
            foreach (var child in _children) inner.Append(child.OuterHTML);

            if (_info.Closing == ClosingType.SelfClosing)
                return $"<{_info.TagName}{classes}>";

            return $"<{_info.TagName}{classes}>{inner}</{_info.TagName}>";
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        string filePath = "book.txt";


        //string filePath = "test.txt";
        //var dummyLines = new List<string> { "Заголовок книги" };
        //for (int i = 0; i < 5000; i++)
        //{
        //    dummyLines.Add("Короткий рядок");
        //    dummyLines.Add("   Це рядок з пробілом на початку для цитати.");
        //    dummyLines.Add("Це звичайний довгий параграф тексту для перевірки пам'яті.");
        //}
        //File.WriteAllLines(filePath, dummyLines);



        string[] lines = File.ReadAllLines(filePath);
        lines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        FlyweightFactory factory = new FlyweightFactory();

        long memoryBefore = GC.GetTotalMemory(true);

        var root = new LightElementNode(factory.GetType("div", DisplayType.Block, ClosingType.Normal));

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            LightElementNode node;

            //Перший рядок h1
            if (i == 0)
                node = new LightElementNode(factory.GetType("h1", DisplayType.Block, ClosingType.Normal));
            //Менше 20 символів h2
            else if (line.Length < 20)
                node = new LightElementNode(factory.GetType("h2", DisplayType.Block, ClosingType.Normal));
            //Починається з пробілу blockquote
            else if (line.StartsWith(" ") || line.StartsWith("\t"))
                node = new LightElementNode(factory.GetType("blockquote", DisplayType.Block, ClosingType.Normal));
            //Інше p
            else
                node = new LightElementNode(factory.GetType("p", DisplayType.Block, ClosingType.Normal));

            node.Add(new LightTextNode(line));
            root.Add(node);
        }

        long memoryAfter = GC.GetTotalMemory(true);
        long memoryUsed = memoryAfter - memoryBefore;

        Console.WriteLine($"Кількість рядків: {lines.Length}");
        Console.WriteLine($"Пам'ять, яку займає дерево: {memoryUsed / 1024.0:F2} KB");

        Console.ReadKey();
        Console.WriteLine("\n" + root.OuterHTML);



        //File.Delete(filePath);
    }
}