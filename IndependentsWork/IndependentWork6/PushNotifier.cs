namespace IndependentWork6;

/// <summary>
/// Push-сповіщувач реалізує лише INotifier (без логування): це демонструє, що поведінку можна комбінувати вибірково.
/// </summary>
public class PushNotifier : INotifier
{
    private readonly string _deviceId;

    public string ChannelName => "Push";

    public PushNotifier(string deviceId)
    {
        _deviceId = deviceId;
    }

    public void Send(string message)
    {
        Console.WriteLine($"Відправлено push-сповіщення на пристрій {_deviceId}: {message}");
    }
}
