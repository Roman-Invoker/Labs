namespace IndependentWork4;

/// <summary>
/// Книга: бібліотечний елемент з автором.
/// </summary>
public class Book : LibraryItem
{
    private string _author;

    public string Author
    {
        get { return _author; }
        set { _author = value; }
    }

    // Крок 4: додано параметр uniqueId і передачу його в base(...)
    public Book(string title, int publicationYear, string author, string uniqueId)
        : base(title, publicationYear, uniqueId)
    {
        _author = author;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"  Автор: {_author}");
    }
}
