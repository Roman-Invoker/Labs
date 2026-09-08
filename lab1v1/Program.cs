namespace lab1v1;

public class Book
{
    
    private string title;
    private string author;

    
    public int Year { get; set; }

    
    public Book(string title, string author, int year)
    {
        this.title = title;
        this.author = author;
        Year = year;
    }

    
    ~Book()
    {
        Console.WriteLine($"Об'єкт книги '{title}' вилучено з пам'яті.");
    }

    
    public string GetInfo()
    {
        return $"Книга: \"{title}\" | Автор: {author} | Рік видання: {Year}";
    }
}