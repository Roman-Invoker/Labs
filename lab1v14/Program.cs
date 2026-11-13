using System.Diagnostics;
using Lab16v14;

/*
 * ЛАБОРАТОРНА РОБОТА №10 (проєкт lab16v14), варіант 14
 * Тема: LINQ в дії: фільтри, join, агрегації. Продуктивність.
 * Сценарій: Управління співробітниками.
 *
 * ===================== ЗВІТ =====================
 *
 * 1. Класи сутностей (див. Models.cs)
 *    - Department (Id, Name, Budget)
 *    - Employee   (Id, Name, DepartmentId, Salary)
 *
 * 2. Тестові дані
 *    - 5 відділів: Sales, IT, HR, Finance, Support
 *    - 20 співробітників (зв'язок Employee.DepartmentId -> Department.Id)
 *    - Для порівняння продуктивності додатково генерується великий набір
 *      (100 000 співробітників), бо на 20 елементах час вимірюється
 *      у мікросекундах і майже не відрізняється.
 *
 * 3. Запити варіанту 14 (кожен реалізовано в синтаксисі запитів і методів)
 *    Q1. Співробітники із зарплатою > 50000 у відділі "Sales".
 *    Q2. Join співробітників із відділами: ім'я співробітника + назва відділу.
 *    Q3. Загальна зарплата для кожного відділу.
 *    Q4. Співробітник з найвищою зарплатою.
 *
 * 4. Час виконання (Stopwatch)
 *    Вимірюється при запуску програми й виводиться в консоль. Занесіть
 *    власні значення після запуску (вони залежать від комп'ютера):
 *      Малий набір (20):      Q1 = ___ мс, Q2 = ___ мс, Q3 = ___ мс, Q4 = ___ мс
 *      Великий набір (100k):  Q1 = ___ мс, Q2 = ___ мс, Q3 = ___ мс, Q4 = ___ мс
 *
 * 5. Висновок
 *    - Query syntax і method syntax компілюються в однакові виклики
 *      методів розширення, тому за часом практично не відрізняються;
 *      різниця в замірах - це шум та JIT-компіляція першого запуску
 *      (тому перед замірами виконується "прогрів").
 *    - Запит Where/Select відкладений: без ToList() Stopwatch зміряв би лише
 *      створення запиту, а не його виконання. Тому в замірах використано
 *      негайне виконання (ToList / ToArray / First / MaxBy).
 *    - Найдорожчими, як правило, є операції, що потребують побудови
 *      проміжних структур: Join (хеш-таблиця за ключем) та GroupBy
 *      (групування). Where, Max/MaxBy - один прохід O(n) без
 *      додаткової пам'яті, тому найдешевші. OrderByDescending().First()
 *      у запиті Q4 (query syntax) дорожчий за MaxBy, бо сортує всю
 *      колекцію - O(n log n) замість O(n).
 *    - На продуктивність впливають: розмір колекції, кількість проходів,
 *      алгоритм операції, алокації (проєкції в анонімні типи), JIT
 *      та відкладене/негайне виконання.
 * ================================================
 */

// ---------- Тестові дані ----------
var departments = new List<Department>
{
    new() { Id = 1, Name = "Sales",   Budget = 500_000m },
    new() { Id = 2, Name = "IT",      Budget = 900_000m },
    new() { Id = 3, Name = "HR",      Budget = 250_000m },
    new() { Id = 4, Name = "Finance", Budget = 600_000m },
    new() { Id = 5, Name = "Support", Budget = 350_000m },
};

var employees = new List<Employee>
{
    new() { Id = 1,  Name = "Oleksandr Kovalenko", DepartmentId = 1, Salary = 62_000m },
    new() { Id = 2,  Name = "Maria Shevchenko",    DepartmentId = 1, Salary = 48_000m },
    new() { Id = 3,  Name = "Andrii Bondarenko",   DepartmentId = 1, Salary = 55_500m },
    new() { Id = 4,  Name = "Iryna Melnyk",        DepartmentId = 1, Salary = 51_000m },
    new() { Id = 5,  Name = "Taras Kravchenko",    DepartmentId = 2, Salary = 95_000m },
    new() { Id = 6,  Name = "Olena Tkachenko",     DepartmentId = 2, Salary = 88_000m },
    new() { Id = 7,  Name = "Dmytro Oliinyk",      DepartmentId = 2, Salary = 72_000m },
    new() { Id = 8,  Name = "Natalia Savchenko",   DepartmentId = 2, Salary = 64_000m },
    new() { Id = 9,  Name = "Vasyl Lysenko",       DepartmentId = 3, Salary = 41_000m },
    new() { Id = 10, Name = "Kateryna Moroz",      DepartmentId = 3, Salary = 39_500m },
    new() { Id = 11, Name = "Serhii Polishchuk",   DepartmentId = 3, Salary = 45_000m },
    new() { Id = 12, Name = "Yulia Boyko",         DepartmentId = 4, Salary = 70_000m },
    new() { Id = 13, Name = "Petro Rudenko",       DepartmentId = 4, Salary = 66_000m },
    new() { Id = 14, Name = "Anna Zhuk",           DepartmentId = 4, Salary = 58_000m },
    new() { Id = 15, Name = "Roman Hryhorenko",    DepartmentId = 4, Salary = 49_000m },
    new() { Id = 16, Name = "Svitlana Panchenko",  DepartmentId = 5, Salary = 36_000m },
    new() { Id = 17, Name = "Bohdan Marchenko",    DepartmentId = 5, Salary = 38_500m },
    new() { Id = 18, Name = "Larysa Kovalchuk",    DepartmentId = 5, Salary = 42_000m },
    new() { Id = 19, Name = "Ihor Stepanenko",     DepartmentId = 5, Salary = 40_000m },
    new() { Id = 20, Name = "Oksana Pavlenko",     DepartmentId = 1, Salary = 50_000m },
};

// Великий набір для порівняння продуктивності
var rnd = new Random(42);
var bigEmployees = Enumerable.Range(1, 100_000)
    .Select(i => new Employee
    {
        Id = i,
        Name = $"Employee {i}",
        DepartmentId = rnd.Next(1, departments.Count + 1),
        Salary = rnd.Next(20_000, 120_000),
    })
    .ToList();

// ---------- Допоміжний метод вимірювання ----------
static (T Result, double Ms) Measure<T>(Func<T> query)
{
    var sw = Stopwatch.StartNew();
    T result = query();           // query має виконуватись негайно (ToList, First, ...)
    sw.Stop();
    return (result, sw.Elapsed.TotalMilliseconds);
}

static void PrintTime(string label, double ms) =>
    Console.WriteLine($"   [{label}] час: {ms:F4} мс");

// ---------- Набір запитів ----------
static void RunAll(string title, List<Employee> emps, List<Department> deps, bool print)
{
    Console.WriteLine($"\n================ {title} (співробітників: {emps.Count}) ================");

    // ---- Q1: Sales, зарплата > 50000 ----
    Console.WriteLine("\nQ1. Співробітники з зарплатою > 50000 у відділі \"Sales\"");

    var (q1Query, t1q) = Measure(() =>
        (from e in emps
         join d in deps on e.DepartmentId equals d.Id
         where d.Name == "Sales" && e.Salary > 50_000m
         select e).ToList());
    PrintTime("синтаксис запитів", t1q);

    var (q1Method, t1m) = Measure(() =>
        emps.Join(deps, e => e.DepartmentId, d => d.Id, (e, d) => new { e, d })
            .Where(x => x.d.Name == "Sales" && x.e.Salary > 50_000m)
            .Select(x => x.e)
            .ToList());
    PrintTime("синтаксис методів", t1m);

    if (print) foreach (var e in q1Method) Console.WriteLine($"   {e.Name}: {e.Salary}");
    else Console.WriteLine($"   знайдено: {q1Method.Count}");

    // ---- Q2: Join ----
    Console.WriteLine("\nQ2. Співробітники + назва відділу (Join)");

    var (q2Query, t2q) = Measure(() =>
        (from e in emps
         join d in deps on e.DepartmentId equals d.Id
         select new { Employee = e.Name, Department = d.Name }).ToList());
    PrintTime("синтаксис запитів", t2q);

    var (q2Method, t2m) = Measure(() =>
        emps.Join(deps,
                  e => e.DepartmentId,
                  d => d.Id,
                  (e, d) => new { Employee = e.Name, Department = d.Name })
            .ToList());
    PrintTime("синтаксис методів", t2m);

    if (print) foreach (var r in q2Method) Console.WriteLine($"   {r.Employee} -> {r.Department}");
    else Console.WriteLine($"   рядків: {q2Method.Count}");

    // ---- Q3: сума зарплат по відділах ----
    Console.WriteLine("\nQ3. Загальна зарплата для кожного відділу");

    var (q3Query, t3q) = Measure(() =>
        (from e in emps
         join d in deps on e.DepartmentId equals d.Id
         group e by d.Name into g
         select new { Department = g.Key, Total = g.Sum(x => x.Salary) }).ToList());
    PrintTime("синтаксис запитів", t3q);

    var (q3Method, t3m) = Measure(() =>
        emps.GroupBy(e => e.DepartmentId)
            .Join(deps, g => g.Key, d => d.Id,
                  (g, d) => new { Department = d.Name, Total = g.Sum(x => x.Salary) })
            .ToList());
    PrintTime("синтаксис методів", t3m);

    foreach (var r in q3Method.OrderBy(x => x.Department))
        Console.WriteLine($"   {r.Department}: {r.Total}");

    // ---- Q4: найвища зарплата ----
    Console.WriteLine("\nQ4. Співробітник з найвищою зарплатою");

    var (q4Query, t4q) = Measure(() =>
        (from e in emps
         orderby e.Salary descending
         select e).First());
    PrintTime("синтаксис запитів (orderby + First)", t4q);

    var (q4Method, t4m) = Measure(() => emps.MaxBy(e => e.Salary)!);
    PrintTime("синтаксис методів (MaxBy)", t4m);

    Console.WriteLine($"   {q4Method.Name}: {q4Method.Salary}");

    // Перевірка, що обидва синтаксиси дають однаковий результат
    bool ok = q1Query.Count == q1Method.Count
              && q2Query.Count == q2Method.Count
              && q3Query.Count == q3Method.Count
              && q4Query.Salary == q4Method.Salary;
    Console.WriteLine($"\nПеревірка збігу результатів обох синтаксисів: {(ok ? "OK" : "РОЗБІЖНІСТЬ")}");
}

// ---------- Запуск ----------
// "Прогрів" JIT, щоб перший заміряний запит не включав час компіляції
RunAllSilently(employees, departments);

RunAll("МАЛИЙ НАБІР", employees, departments, print: true);
RunAll("ВЕЛИКИЙ НАБІР", bigEmployees, departments, print: false);

// ---------- Демонстрація відкладеного виконання ----------
Console.WriteLine("\n================ ВІДКЛАДЕНЕ vs НЕГАЙНЕ ВИКОНАННЯ ================");
var swDeferred = Stopwatch.StartNew();
var deferred = bigEmployees.Where(e => e.Salary > 100_000m);   // запит ще не виконано
swDeferred.Stop();
Console.WriteLine($"Створення запиту (Where, без ітерації): {swDeferred.Elapsed.TotalMilliseconds:F4} мс");

var swImmediate = Stopwatch.StartNew();
var immediate = deferred.ToList();                              // виконання тут
swImmediate.Stop();
Console.WriteLine($"Виконання (ToList): {swImmediate.Elapsed.TotalMilliseconds:F4} мс, елементів: {immediate.Count}");

static void RunAllSilently(List<Employee> emps, List<Department> deps)
{
    var original = Console.Out;
    Console.SetOut(TextWriter.Null);
    try { RunAll("WARM-UP", emps, deps, print: false); }
    finally { Console.SetOut(original); }
}
