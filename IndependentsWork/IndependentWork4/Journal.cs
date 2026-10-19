namespace IndependentWork4;

/// <summary>
/// Журнал: бібліотечний елемент з номером випуску.
/// </summary>
public class Journal : LibraryItem
{
    private int _issueNumber;

    public int IssueNumber
    {
        get { return _issueNumber; }
        set { _issueNumber = value; }
    }

    // Крок 4: додано параметр uniqueId і передачу його в base(...)
    public Journal(string title, int publicationYear, int issueNumber, string uniqueId)
        : base(title, publicationYear, uniqueId)
    {
        _issueNumber = issueNumber;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"  Номер випуску: {_issueNumber}");
    }
}
