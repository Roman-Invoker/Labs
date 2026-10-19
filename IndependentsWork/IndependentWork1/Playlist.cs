namespace IndependentWork1;

/// <summary>
/// Плейлист: назва, тривалість та кількість треків.
/// </summary>
public class Playlist
{
    
    private string _name;
    private int _trackCount;
    private double _totalMinutes;

    public string Name
    {
        get { return _name; }
        set { _name = string.IsNullOrWhiteSpace(value) ? "Без назви" : value; }
    }

    public int TrackCount
    {
        get { return _trackCount; }
    }

    
    public double TotalMinutes
    {
        get { return _totalMinutes; }
    }

    public Playlist(string name, int trackCount, double totalMinutes)
    {
        _name = string.IsNullOrWhiteSpace(name) ? "Без назви" : name;
        _trackCount = trackCount < 0 ? 0 : trackCount;
        _totalMinutes = totalMinutes < 0 ? 0 : totalMinutes;
    }

    public void AddTrack(double durationMinutes)
    {
        if (durationMinutes <= 0)
        {
            Console.WriteLine("Тривалість треку має бути більшою за 0.");
            return;
        }

        _trackCount++;
        _totalMinutes += durationMinutes;
    }

    public double GetAverageTrackLength()
    {
        if (_trackCount == 0)
        {
            return 0;
        }

        return _totalMinutes / _trackCount;
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Плейлист \"{_name}\": треків = {_trackCount}, " +
                          $"загальна тривалість = {_totalMinutes:F1} хв, " +
                          $"середня тривалість треку = {GetAverageTrackLength():F2} хв");
    }
}
