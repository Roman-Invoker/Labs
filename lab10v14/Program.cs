using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab16v14
{
    // 2. Класи сутностей
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public decimal Salary { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №10: LINQ в дії (Варіант 14) ===\n");

        
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "Sales", Budget = 1500000m },
                new Department { Id = 2, Name = "Development", Budget = 3000000m },
                new Department { Id = 3, Name = "HR", Budget = 500000m },
                new Department { Id = 4, Name = "Marketing", Budget = 800000m }
            };

            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "Олександр Петренко", DepartmentId = 1, Salary = 55000m },
                new Employee { Id = 2, Name = "Марія Ковальчук", DepartmentId = 2, Salary = 72000m },
                new Employee { Id = 3, Name = "Іван Бондар", DepartmentId = 1, Salary = 48000m },
                new Employee { Id = 4, Name = "Анна Шевченко", DepartmentId = 3, Salary = 35000m },
                new Employee { Id = 5, Name = "Максим Мельник", DepartmentId = 2, Salary = 85000m },
                new Employee { Id = 6, Name = "Юлія Романюк", DepartmentId = 4, Salary = 60000m },
                new Employee { Id = 7, Name = "Дмитро Ткачук", DepartmentId = 1, Salary = 62000m },
                new Employee { Id = 8, Name = "Олена Савчук", DepartmentId = 2, Salary = 90000m },
                new Employee { Id = 9, Name = "Андрій Павлюк", DepartmentId = 1, Salary = 51000m },
                new Employee { Id = 10, Name = "Тетяна Лисенко", DepartmentId = 4, Salary = 45000m },
                new Employee { Id = 11, Name = "Сергій Коваль", DepartmentId = 2, Salary = 67000m },
                new Employee { Id = 12, Name = "Ольга Мороз", DepartmentId = 3, Salary = 40000m },
                new Employee { Id = 13, Name = "Володимир Гнатюк", DepartmentId = 1, Salary = 58000m },
                new Employee { Id = 14, Name = "Катерина Руденко", DepartmentId = 4, Salary = 53000m },
                new Employee { Id = 15, Name = "Артем Поліщук", DepartmentId = 2, Salary = 78000m }
            };

            var stopwatch = new Stopwatch();

        
            Console.WriteLine("--- Запит 1: Співробітники відділу 'Sales' із зарплатою > 50000 ---");
         
            stopwatch.Restart();
            var q1Query = from e in employees
                          join d in departments on e.DepartmentId equals d.Id
                          where d.Name == "Sales" && e.Salary > 50000m
                          select e;
            var res1Q = q1Query.ToList();
            stopwatch.Stop();
            long timeQuery1 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var q1Method = employees
                .Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new { Employee = e, Department = d })
                .Where(x => x.Department.Name == "Sales" && x.Employee.Salary > 50000m)
                .Select(x => x.Employee);
            var res1M = q1Method.ToList();
            stopwatch.Stop();
            long timeMethod1 = stopwatch.ElapsedTicks;

            Console.WriteLine($"Результатів знайдено: {res1M.Count}");
            foreach (var emp in res1M)
            {
                Console.WriteLine($"  - {emp.Name}: {emp.Salary} грн");
            }
            Console.WriteLine($"Час (Query Syntax): {timeQuery1} ticks | Час (Method Syntax): {timeMethod1} ticks\n");


            Console.WriteLine("--- Запит 2: Об'єднання співробітників та відділів (Join) ---");

            stopwatch.Restart();
            var q2Query = from e in employees
                          join d in departments on e.DepartmentId equals d.Id
                          select new { e.Name, DepartmentName = d.Name };
            var res2Q = q2Query.ToList();
            stopwatch.Stop();
            long timeQuery2 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var q2Method = employees.Join(
                departments,
                e => e.DepartmentId,
                d => d.Id,
                (e, d) => new { e.Name, DepartmentName = d.Name }
            ).ToList();
            stopwatch.Stop();
            long timeMethod2 = stopwatch.ElapsedTicks;

            Console.WriteLine($"Виведено перші 3 записи з {res2MCount = res2Q.Count}:");
            foreach (var item in res2Q.Take(3))
            {
                Console.WriteLine($"  - Співробітник: {item.Name} | Відділ: {item.DepartmentName}");
            }
            Console.WriteLine($"Час (Query Syntax): {timeQuery2} ticks | Час (Method Syntax): {timeMethod2} ticks\n");


          
            Console.WriteLine("--- Запит 3: Загальна зарплата для кожного відділу (Aggregation/GroupBy) ---");

            stopwatch.Restart();
            var q3Query = from e in employees
                          join d in departments on e.DepartmentId equals d.Id
                          group e by d.Name into deptGroup
                          select new { Department = deptGroup.Key, TotalSalary = deptGroup.Sum(x => x.Salary) };
            var res3Q = q3Query.ToList();
            stopwatch.Stop();
            long timeQuery3 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var q3Method = employees
                .Join(departments, e => e.DepartmentId, d => d.Id, (e, d) => new { e.Salary, DepartmentName = d.Name })
                .GroupBy(x => x.DepartmentName)
                .Select(g => new { Department = g.Key, TotalSalary = g.Sum(x => x.Salary) })
                .ToList();
            stopwatch.Stop();
            long timeMethod3 = stopwatch.ElapsedTicks;

            foreach (var item in res3Q)
            {
                Console.WriteLine($"  - Відділ '{item.Department}': загальна зарплата = {item.TotalSalary} грн");
            }
            Console.WriteLine($"Час (Query Syntax): {timeQuery3} ticks | Час (Method Syntax): {timeMethod3} ticks\n");


    
            Console.WriteLine("--- Запит 4: Співробітник з найвищою зарплатою ---");

            stopwatch.Restart();
            var q4Query = (from e in employees
                           orderby e.Salary descending
                           select e).FirstOrDefault();
            stopwatch.Stop();
            long timeQuery4 = stopwatch.ElapsedTicks;

            stopwatch.Restart();
            var q4Method = employees.OrderByDescending(e => e.Salary).FirstOrDefault();
            stopwatch.Stop();
            long timeMethod4 = stopwatch.ElapsedTicks;

            if (q4Method != null)
            {
                Console.WriteLine($"  - Найвища зарплата у: {q4Method.Name} ({q4Method.Salary} грн)");
            }
            Console.WriteLine($"Час (Query Syntax): {timeQuery4} ticks | Час (Method Syntax): {timeMethod4} ticks\n");

            Console.WriteLine("=== Висновки щодо продуктивності ==[cite: 1, 4]");
            Console.WriteLine("1. Синтаксис запитів (Query Syntax) та синтаксис методів (Method Syntax) демонструють практично ідентичну швидкість виконання, оскільки транслятор C# перетворює обидва підходи у схожий код на основі викликів методів розширення (LINQ to Objects).");
            Console.WriteLine("2. Операції об'єднання (Join) та групування (GroupBy) є більш «дорогими» за часом порівняно з простою фільтрацією (Where), оскільки вимагають побудови тимчасових індексів (хеш-таблиць) для зіставлення ключів колекцій.");
            Console.WriteLine("3. Використання негайного виконання (наприклад, через .ToList() або .FirstOrDefault()) змушує запит виконуватися одразу, тоді як стандартні LINQ-вирази використовують відкладене виконання (Deferred Execution), що оптимізує роботу з пам'яттю.");
        }
    }
}