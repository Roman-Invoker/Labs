namespace IndependentWork4;

/// <summary>
/// Базовий клас бібліотечного елемента.
/// </summary>
public class LibraryItem
{
    private string _title;
    private int _publicationYear;
    private readonly string _uniqueId; // додано на кроці 4

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public int PublicationYear
    {
        get { return _publicationYear; }
        set { _publicationYear = value; }
    }

    // Лише для читання: після створення елемента ідентифікатор не змінюється (крок 4)
    public string UniqueId
    {
        get { return _uniqueId; }
    }

    // Крок 4: конструктор отримав третій параметр uniqueId
    public LibraryItem(string title, int publicationYear, string uniqueId)
    {
        _title = title;
        _publicationYear = publicationYear;
        _uniqueId = uniqueId;
    }

    public virtual void DisplayDetails()
    {
        Console.WriteLine($"[{_uniqueId}] {_title} ({_publicationYear})");
    }
}
