using System.Text.RegularExpressions;

public interface ISmartTextReader
{
    char[][] ReadText(string filePath);
}

public class SmartTextReader : ISmartTextReader
{
    public char[][] ReadText(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        char[][] result = new char[lines.Length][];

        for (int i = 0; i < lines.Length; i++)
        {
            result[i] = lines[i].ToCharArray();
        }

        return result;
    }
}

public class SmartTextChecker : ISmartTextReader
{
    private readonly ISmartTextReader _reader;

    public SmartTextChecker(ISmartTextReader reader)
    {
        _reader = reader;
    }

    public char[][] ReadText(string filePath)
    {
        Console.WriteLine($"Спроба відкриття файлу: {filePath}");

        char[][] result = _reader.ReadText(filePath);

        if (result != null)
        {
            Console.WriteLine($"Файл успішно прочитано.");

            int totalChars = result.Sum(row => row.Length);
            Console.WriteLine($"[Stats]: Рядків: {result.Length}, Символів: {totalChars}");

            Console.WriteLine($"[Log]: Файл закрито.");
        }

        return result;
    }
}

public class SmartTextReaderLocker : ISmartTextReader
{
    private readonly ISmartTextReader _reader;
    private readonly Regex _lockPattern;

    public SmartTextReaderLocker(ISmartTextReader reader, string pattern)
    {
        _reader = reader;
        _lockPattern = new Regex(pattern);
    }

    public char[][] ReadText(string filePath)
    {
        if (_lockPattern.IsMatch(filePath))
        {
            Console.WriteLine($"Відмовлено у доступі до файлу: {filePath}");
            return null;
        }

        return _reader.ReadText(filePath);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string publicFile = "public.txt";
        string privateFile1 = "secret_data.txt";
        string privateFile2 = "log.log";

        ISmartTextReader realReader = new SmartTextReader();
        ISmartTextReader checker = new SmartTextChecker(realReader);
        ISmartTextReader locker = new SmartTextReaderLocker(checker, @"(\.log|secret)");

        Console.WriteLine("Читання дозволеного файлу");
        locker.ReadText(publicFile);

        Console.WriteLine("\nЧитання забороненого файлу (secret)");
        locker.ReadText(privateFile1);

        Console.WriteLine("\nЧитання забороненого файлу (.log)");
        locker.ReadText(privateFile2);
    }
}