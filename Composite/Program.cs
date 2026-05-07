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

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

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
    }
}