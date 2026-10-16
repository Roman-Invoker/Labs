Console.OutputEncoding = System.Text.Encoding.UTF8;

var repo = new InMemoryRepository<Course>();

void PrintAll(string header)
{
    Console.WriteLine(header);
    foreach (var c in repo.GetAll())
        Console.WriteLine($"  {c.Id}: {c.Title}, кредитів: {c.Credits}, викладач: {c.TeacherId}");
    Console.WriteLine();
}

// Add
repo.Add(new Course { Id = 1, Title = "Об'єктно-орієнтоване програмування", Credits = 5, TeacherId = 101 });
repo.Add(new Course { Id = 2, Title = "Бази даних", Credits = 4, TeacherId = 102 });
repo.Add(new Course { Id = 3, Title = "Веб технології", Credits = 3, TeacherId = 103 });
PrintAll("Після Add:");

// GetById
var found = repo.GetById(2);
Console.WriteLine($"GetById(2): {found?.Title}");
Console.WriteLine($"GetById(999): {(repo.GetById(999) is null ? "не знайдено" : "знайдено")}");
Console.WriteLine();

// Update
repo.Update(new Course { Id = 2, Title = "Бази даних (поглиблений курс)", Credits = 6, TeacherId = 102 });
PrintAll("Після Update(2):");

try { repo.Update(new Course { Id = 999, Title = "Немає" }); }
catch (KeyNotFoundException ex) { Console.WriteLine($"Update неіснуючого: {ex.Message}\n"); }

// Delete
repo.Delete(1);
PrintAll("Після Delete(1):");

try { repo.Delete(999); }
catch (KeyNotFoundException ex) { Console.WriteLine($"Delete неіснуючого: {ex.Message}"); }
