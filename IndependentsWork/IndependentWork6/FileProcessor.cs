namespace IndependentWork6;

/// <summary>
/// Базовий клас обробника файлів. Містить спільні кроки (відкриття, закриття)
/// та фіксує порядок дій у методі Process() (шаблонний метод).
/// Унікальна логіка обробки вмісту реалізується в нащадках.
/// </summary>
public abstract class FileProcessor
{
    private readonly string _filePath;
    private StreamReader? _reader;

    public string FilePath
    {
        get { return _filePath; }
    }

    // Кожен нащадок повідомляє, який тип файлів він обробляє
    public abstract string FileType { get; }

    protected FileProcessor(string filePath)
    {
        _filePath = filePath;
    }

    // Не virtual: порядок "відкрити -> обробити -> закрити" не можна порушити в нащадках
    public void Process()
    {
        Console.WriteLine($"[{FileType}] Обробка файлу: {_filePath}");

        Open();
        try
        {
            ProcessContent(_reader!);
        }
        finally
        {
            Close(); // файл закривається навіть у разі помилки
        }
    }

    // Спільний крок 1: відкриття (за потреби нащадок може перевизначити)
    protected virtual void Open()
    {
        Console.WriteLine("  Відкриття файлу...");
        _reader = new StreamReader(_filePath);
    }

    // Спільний крок 2: закриття
    protected virtual void Close()
    {
        _reader?.Dispose();
        _reader = null;
        Console.WriteLine("  Закриття файлу.");
    }

    // Унікальна логіка: обов'язково реалізується в кожному нащадку
    protected abstract void ProcessContent(TextReader reader);
}
