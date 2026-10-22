using System.Globalization;
using System.Text;
using IndependentWork12;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // dotnet run -c Release            -> усе (1M, 5M, 10M елементів; кілька хвилин)
        // dotnet run -c Release -- quick   -> усе на менших колекціях (100k, 500k, 1M)
        // dotnet run -c Release -- perf    -> лише порівняння продуктивності
        // dotnet run -c Release -- side    -> лише побічні ефекти
        string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "all";
        bool quick = mode == "quick";
        bool doPerf = mode is "all" or "quick" or "perf";
        bool doSide = mode is "all" or "quick" or "side";

        int[] sizes = quick
            ? new[] { 100_000, 500_000, 1_000_000 }
            : new[] { 1_000_000, 5_000_000, 10_000_000 };

#if DEBUG
        Console.WriteLine("УВАГА: це Debug-збірка, результати будуть спотворені. Запустіть: dotnet run -c Release");
        Console.WriteLine();
#endif

        Console.WriteLine($"Логічних процесорів: {Environment.ProcessorCount}");
        Console.WriteLine();

        var report = new StringBuilder();
        report.Append(PerformanceExperiments.FormatEnvironment());

        if (doPerf)
        {
            Console.WriteLine("=== Експеримент 1: LINQ проти PLINQ ===");
            var rows = PerformanceExperiments.RunExperiments(sizes);
            string table = PerformanceExperiments.FormatExperiments(rows);

            Console.WriteLine();
            Console.WriteLine(table);

            Console.WriteLine("=== Експеримент 2: залежність від кількості потоків ===");
            Operation heavy = Operations.All[1]; // "Складні математичні перетворення"
            int scalingSize = sizes[1];
            var (linqMs, scalingRows) = PerformanceExperiments.RunParallelismScaling(heavy, scalingSize);
            string scalingTable = PerformanceExperiments.FormatScaling(linqMs, scalingRows);

            Console.WriteLine();
            Console.WriteLine($"Операція: {heavy.Name}, розмір: {scalingSize:N0}");
            Console.WriteLine(scalingTable);

            report.AppendLine("## Експеримент 1: LINQ проти PLINQ");
            report.AppendLine();
            report.Append(table);
            report.AppendLine($"## Експеримент 2: кількість потоків (операція «{heavy.Name}», {scalingSize:N0} елементів)");
            report.AppendLine();
            report.Append(scalingTable);
        }

        if (doSide)
        {
            Console.WriteLine("=== Побічні ефекти в PLINQ ===");
            string transcript = SideEffectsDemo.Run(sizes[1]);

            report.AppendLine("## Побічні ефекти в PLINQ");
            report.AppendLine();
            report.AppendLine("```");
            report.Append(transcript);
            report.AppendLine("```");
        }

        File.WriteAllText("benchmark-results.md", report.ToString());
        Console.WriteLine();
        Console.WriteLine("Результати збережено у benchmark-results.md");
    }
}
