using IndependentWork6;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // ---------- Сценарій 1: система сповіщень (інтерфейс) ----------
        Console.WriteLine("=== Сценарій 1: Система сповіщень (інтерфейс) ===");

        List<INotifier> notifiers = new List<INotifier>
        {
            new EmailNotifier("user@example.com"),
            new SmsNotifier("+380501234567"),
            new PushNotifier("device-42")
        };

        foreach (INotifier notifier in notifiers)
        {
            Console.WriteLine($"[{notifier.ChannelName}]");
            notifier.Send("Ваше замовлення відправлено!"); // поліморфний виклик

            // Додаткова поведінка є не в усіх каналів
            if (notifier is ILoggable loggable)
            {
                loggable.WriteLog();
            }
        }

        // ---------- Сценарій 2: обробка файлів (абстрактний клас) ----------
        Console.WriteLine();
        Console.WriteLine("=== Сценарій 2: Обробка файлів (абстрактний клас) ===");

        // Тестові файли створюються в коді, тож проєкт самодостатній
        File.WriteAllLines("sample.txt", new[]
        {
            "Перший рядок",
            "Другий рядок тексту",
            "Третій"
        });
        File.WriteAllText("sample.xml",
            "<library>" +
            "<book id=\"1\"><title>Кобзар</title></book>" +
            "<book id=\"2\"><title>Лісова пісня</title></book>" +
            "</library>");
        File.WriteAllLines("sample.csv", new[]
        {
            "name,price",
            "Пенал,25",
            "Зошит,18",
            "Ручка,7"
        });

        List<FileProcessor> processors = new List<FileProcessor>
        {
            new TextFileProcessor("sample.txt"),
            new XmlFileProcessor("sample.xml"),
            new CsvFileProcessor("sample.csv")
        };

        foreach (FileProcessor processor in processors)
        {
            processor.Process(); // поліморфний виклик
        }
    }
}
