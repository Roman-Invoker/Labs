using IndependentWork4;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        
        Console.WriteLine("=== Книга ===");
        Book book = new Book("Кобзар", 1840, "Тарас Шевченко", "BK-001");
        book.DisplayDetails();

        Console.WriteLine();
        Console.WriteLine("=== Журнал ===");
        Journal journal = new Journal("Наука і життя", 2023, 7, "JR-001");
        journal.DisplayDetails();

        
        Console.WriteLine();
        Console.WriteLine("=== Аудіокнига ===");
        AudioBook audioBook = new AudioBook("Лісова пісня", 1911, "Леся Українка", "Олена Коваль", "AB-001");
        audioBook.DisplayDetails();

        
        Console.WriteLine();
        Console.WriteLine("=== Поліморфний виклик ===");
        List<LibraryItem> items = new List<LibraryItem> { book, journal, audioBook };
        foreach (LibraryItem item in items)
        {
            item.DisplayDetails();
            Console.WriteLine();
        }
    }
}
