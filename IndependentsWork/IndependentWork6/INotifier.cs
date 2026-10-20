namespace IndependentWork6;

/// <summary>
/// Контракт сповіщувача: будь-який канал вміє надсилати повідомлення.
/// </summary>
public interface INotifier
{
    string ChannelName { get; }

    void Send(string message);
}
