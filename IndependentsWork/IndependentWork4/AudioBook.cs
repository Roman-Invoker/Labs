namespace IndependentWork4;

/// <summary>
/// Аудіокнига: книга з оповідачем. Успадковується від Book, а не від LibraryItem (крок 5).
/// </summary>
public class AudioBook : Book
{
    private string _narrator;

    public string Narrator
    {
        get { return _narrator; }
        set { _narrator = value; }
    }

    // uniqueId проходить через Book до LibraryItem
    public AudioBook(string title, int publicationYear, string author, string narrator, string uniqueId)
        : base(title, publicationYear, author, uniqueId)
    {
        _narrator = narrator;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails(); // виводить базові дані та автора (Book)
        Console.WriteLine($"  Оповідач: {_narrator}");
    }
}
