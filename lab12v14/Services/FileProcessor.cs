namespace lab18v14.Services;

/// <summary>Імітація роботи з файлами (варіант 14: оновлення профілю користувача).</summary>
public class FileProcessor
{
    private readonly int _failuresBeforeSuccess;
    private int _calls;

    /// <param name="failuresBeforeSuccess">Скільки перших викликів завершуються FileNotFoundException (за варіантом — 3).</param>
    public FileProcessor(int failuresBeforeSuccess = 3)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int Calls => _calls;

    /// <summary>
    /// Оновлює профіль користувача у файлі.
    /// Перші 3 виклики імітують FileNotFoundException, потім — успіх.
    /// </summary>
    public void UpdateUserProfile(string path, string profileJson)
    {
        _calls++;

        if (_calls <= _failuresBeforeSuccess)
        {
            throw new FileNotFoundException(
                $"Імітація: файл профілю тимчасово недоступний (виклик №{_calls}).", path);
        }

        Console.WriteLine($"    [FileProcessor] Профіль записано у '{path}' ({profileJson.Length} символів).");
    }
}
