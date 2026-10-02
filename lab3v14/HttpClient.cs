using System;

namespace Lab3v14;


public class HttpClient : IDisposable
{
    
    private bool _disposed = false;

   
    private readonly string _baseUrl;
    private bool _isConnected;

    
    public string BaseUrl => _baseUrl;
    public bool IsConnected => _isConnected;
    public int RequestCount { get; private set; }

       public HttpClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _isConnected = true;
        RequestCount = 0;
        Console.WriteLine($"[HttpClient] З'єднання з {_baseUrl} відкрито (ресурс виділено)");
    }

        public string Get(string endpoint)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HttpClient), "Клієнт уже звільнено.");

        if (!_isConnected)
        {
            Console.WriteLine("[HttpClient] Помилка: з'єднання закрите, запит неможливий");
            return string.Empty;
        }

        RequestCount++;
        string response = $"200 OK (довжина тіла: {endpoint.Length * 128} байт)";
        Console.WriteLine($"[HttpClient] GET {_baseUrl}{endpoint} -> {response}");
        return response;
    }

    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
               
                Console.WriteLine($"[HttpClient] Звільнення керованих ресурсів " +
                                  $"(оброблено запитів: {RequestCount})");
            }

            
            if (_isConnected)
            {
                Console.WriteLine($"[HttpClient] Закриття з'єднання з {_baseUrl}");
                _isConnected = false;
            }

            _disposed = true;
        }
    }

    
    public void Dispose()
    {
        Dispose(true);
        
        GC.SuppressFinalize(this);
    }

    
    ~HttpClient()
    {
        Console.WriteLine("[HttpClient] Спрацював деструктор ~HttpClient()");
        Dispose(false);
    }
}
