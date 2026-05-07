public class Logger
{
    public virtual void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[INFO]: {message}");
        Console.ResetColor();
    }

    public virtual void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR]: {message}");
        Console.ResetColor();
    }

    public virtual void Warn(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"[WARNING]: {message}");
        Console.ResetColor();
    }
}

public class FileWriter
{
    private string _filePath;

    public FileWriter(string filePath)
    {
        _filePath = filePath;
    }

    public void Write(string text)
    {
        File.AppendAllText(_filePath, text);
    }

    public void WriteLine(string text)
    {
        File.AppendAllText(_filePath, text + Environment.NewLine);
    }
}

public class FileLoggerAdapter : Logger
{
    private readonly FileWriter _fileWriter;

    public FileLoggerAdapter(FileWriter fileWriter)
    {
        _fileWriter = fileWriter;
    }

    public override void Log(string message)
    {
        _fileWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [INFO]: {message}");
    }

    public override void Error(string message)
    {
        _fileWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR]: {message}");
    }

    public override void Warn(string message)
    {
        _fileWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [WARNING]: {message}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Logger consoleLogger = new Logger();
        Console.WriteLine("Робота з Console Logger");
        consoleLogger.Log("Система запущена.");
        consoleLogger.Warn("Мало вільного місця на диску.");
        consoleLogger.Error("Критична помилка бази даних!");
        Console.WriteLine();

        string fileName = "log.txt";
        FileWriter fileWriter = new FileWriter(fileName);
        Logger fileLogger = new FileLoggerAdapter(fileWriter);

        Console.WriteLine("Робота з File Logger (Адаптер)");
        Console.WriteLine($"Шлях до файлу: {Path.GetFullPath(fileName)}");

        fileLogger.Log("Користувач увійшов у систему.");
        fileLogger.Warn("Спроба доступу до закритого ресурсу.");
        fileLogger.Error("Не вдалося зберегти зміни.");

    }
}