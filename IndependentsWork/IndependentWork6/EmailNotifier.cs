namespace IndependentWork6;

public class EmailNotifier : INotifier, ILoggable
{
    private readonly string _address;
    private int _sentCount;

    public string ChannelName => "Email";

    public EmailNotifier(string address)
    {
        _address = address;
    }

    public void Send(string message)
    {
        Console.WriteLine($"Відправлено email на {_address}: {message}");
        _sentCount++;
    }

    public void WriteLog()
    {
        Console.WriteLine($"[Лог] {ChannelName}: надіслано повідомлень - {_sentCount}");
    }
}
