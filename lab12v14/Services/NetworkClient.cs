namespace lab18v14.Services;

/// <summary>Імітація мережевих запитів (варіант 14: надсилання профілю користувача).</summary>
public class NetworkClient
{
    private readonly int _failuresBeforeSuccess;
    private int _calls;

    /// <param name="failuresBeforeSuccess">Скільки перших викликів завершуються HttpRequestException (за варіантом — 2).</param>
    public NetworkClient(int failuresBeforeSuccess = 2)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int Calls => _calls;

    /// <summary>
    /// Надсилає профіль користувача на сервер.
    /// Перші 2 виклики імітують HttpRequestException, потім — успіх (true).
    /// </summary>
    public bool PostUserProfile(string url, string profileJson)
    {
        _calls++;

        if (_calls <= _failuresBeforeSuccess)
        {
            throw new HttpRequestException($"Імітація: сервер {url} тимчасово недоступний (виклик №{_calls}).");
        }

        Console.WriteLine($"    [NetworkClient] POST {url}: профіль надіслано ({profileJson.Length} символів).");
        return true;
    }
}
