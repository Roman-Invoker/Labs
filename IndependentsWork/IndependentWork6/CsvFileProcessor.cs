using System.Globalization;

namespace IndependentWork6;

public class CsvFileProcessor : FileProcessor
{
    public override string FileType => "CSV-файл";

    public CsvFileProcessor(string filePath) : base(filePath)
    {
    }

    protected override void ProcessContent(TextReader reader)
    {
        string? header = reader.ReadLine(); // перший рядок: назви стовпців
        if (header == null)
        {
            Console.WriteLine("  Файл порожній.");
            return;
        }

        int rows = 0;
        int sum = 0;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] parts = line.Split(',');
            sum += int.Parse(parts[1], CultureInfo.InvariantCulture);
            rows++;
        }

        string[] columns = header.Split(',');
        Console.WriteLine($"  Рядків даних: {rows}, сума стовпця {columns[1]}: {sum}");
    }
}
