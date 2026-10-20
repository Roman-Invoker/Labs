namespace IndependentWork6;

public class SmsNotifier : INotifier, ILoggable
{
    private readonly string _phoneNumber;
    private int _sentCount;

    public string ChannelName => "SMS";

    public SmsNotifier(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
    }

    public void Send(string message)
    {
        Console.WriteLine($"Відправлено SMS на {_phoneNumber}: {message}");
        _sentCount++;
    }

    public void WriteLog()
    {
        Console.WriteLine($"[Лог] {ChannelName}: надіслано повідомлень - {_sentCount}");
    }
}
