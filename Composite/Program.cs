using System.Text;

public enum DisplayType { Block, Inline }
public enum ClosingType { Normal, SelfClosing }


public interface IImageLoadingStrategy
{
    void Load(string href);
}

public class FileSystemImageLoadingStrategy : IImageLoadingStrategy
{
    public void Load(string href)
    {
        Console.WriteLine($"Завантаження зображення з локального диска за шляхом: {href}");
    }
}

public class NetworkImageLoadingStrategy : IImageLoadingStrategy
{
    public void Load(string href)
    {
        Console.WriteLine($"Завантаження зображення з інтернету за посиланням: {href}");
    }
}


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


    private readonly Dictionary<string, List<Action>> _eventListeners = new();


    public LightElementNode(string tagName, DisplayType display, ClosingType closing)
    {
        TagName = tagName;
        Display = display;
        Closing = closing;
    }


    public void AddEventListener(string eventType, Action handler)
    {
        if (!_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType] = new List<Action>();
        }
        _eventListeners[eventType].Add(handler);
    }

    public void TriggerEvent(string eventType)
    {
        if (_eventListeners.ContainsKey(eventType))
        {
            Console.WriteLine($"\nПодія '{eventType}' спрацювала на елементі '{TagName}'");
            foreach (var handler in _eventListeners[eventType])
            {
                handler.Invoke();
            }
        }
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


public class LightImageNode : LightElementNode
{
    private IImageLoadingStrategy _loader;
    private string _href;

    public LightImageNode(string href) : base("img", DisplayType.Inline, ClosingType.SelfClosing)
    {
        _href = href;

        if (href.StartsWith("http://") || href.StartsWith("https://"))
        {
            _loader = new NetworkImageLoadingStrategy();
        }
        else
        {
            _loader = new FileSystemImageLoadingStrategy();
        }
    }

    public void LoadImage()
    {
        _loader.Load(_href);
    }

    public override string OuterHTML
    {
        get
        {
            string classes = CssClasses.Any() ? $" class=\"{string.Join(" ", CssClasses)}\"" : "";
            return $"<{TagName} src=\"{_href}\"{classes}>";
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Паттерн 'Спостерігач'");
        
        var button = new LightElementNode("button", DisplayType.Inline, ClosingType.Normal);
        button.CssClasses.Add("btn-primary");
        button.Add(new LightTextNode("Натисни мене"));

        button.AddEventListener("click", () => {
            Console.WriteLine("Користувач натиснув на кнопку");
        });

        button.AddEventListener("click", () => {
            Console.WriteLine("Відправлено повідомлення про клік на сервер");
        });

        button.AddEventListener("mouseover", () => {
            Console.WriteLine("Кнопка підсвітилася синім кольором.");
        });

        Console.WriteLine("Структура елемента");
        Console.WriteLine(button.OuterHTML);

        Console.WriteLine("\nСимуляція взаємодії");
        button.TriggerEvent("mouseover");
        button.TriggerEvent("click");

        button.TriggerEvent("keydown");


        Console.WriteLine("\n\nПаттерн 'Стратегія'");

        var networkImage = new LightImageNode("https://example.com/logo.png");
        Console.WriteLine("Структура елемента");
        Console.WriteLine(networkImage.OuterHTML);
        networkImage.LoadImage();

        Console.WriteLine();

        var localImage = new LightImageNode("C:/Images/photo.jpg");
        Console.WriteLine("Структура елемента");
        Console.WriteLine(localImage.OuterHTML);
        localImage.LoadImage();
    }
}