using IndependentWork1;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Rectangle ===");
        Rectangle rect = new Rectangle(4, 6);
        rect.PrintInfo();
        Console.WriteLine($"Площа: {rect.Area}");
        Console.WriteLine($"Периметр: {rect.CalculatePerimeter()}");
        Console.WriteLine($"Це квадрат? {rect.IsSquare()}");
        rect.Width = 6;
        Console.WriteLine("Після зміни ширини до 6:");
        rect.PrintInfo();

        Console.WriteLine();
        Console.WriteLine("=== Playlist ===");
        Playlist playlist = new Playlist("Ранкова зарядка", 3, 10.5);
        playlist.PrintInfo();
        playlist.AddTrack(4.5);
        Console.WriteLine("Після додавання треку на 4.5 хв:");
        playlist.PrintInfo();
        Console.WriteLine($"Середня тривалість: {playlist.GetAverageTrackLength():F2} хв");

        Console.WriteLine();
        Console.WriteLine("=== Recipe ===");
        Recipe recipe = new Recipe("Омлет з овочами", 2, 250, 15);
        recipe.PrintInfo();
        Console.WriteLine($"Загальна калорійність: {recipe.CalculateTotalCalories()} ккал");
        Console.WriteLine($"Швидкий рецепт? {recipe.IsQuick()}");
        recipe.ScaleServings(4);
        Console.WriteLine("Після збільшення до 4 порцій:");
        recipe.PrintInfo();
    }
}
