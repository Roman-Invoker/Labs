namespace IndependentWork6;

public class TextFileProcessor : FileProcessor
{
    public override string FileType => "Текстовий файл";

    public TextFileProcessor(string filePath) : base(filePath)
    {
    }

    protected override void ProcessContent(TextReader reader)
    {
        int lines = 0;
        int words = 0;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            lines++;
            words += line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }

        Console.WriteLine($"  Рядків: {lines}, слів: {words}");
    }
}
