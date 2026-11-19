using lab19v14.Common;
using lab19v14.Exceptions;
using lab19v14.Models;

namespace lab19v14.Services;

/// <summary>Сервіс управління задачами (варіант 14).</summary>
public class TaskService
{
    private readonly Dictionary<int, TaskItem> _tasks = new();

    public TaskService()
    {
        // Початкові тестові дані
        Add(new TaskItem { Id = 1, Title = "Налаштувати репозиторій", IsCompleted = true, Assignee = "Олена" });
        Add(new TaskItem { Id = 2, Title = "Написати Result<T>" });
        Add(new TaskItem { Id = 3, Title = "Підготувати звіт" });
    }

    public IReadOnlyCollection<TaskItem> GetAll() => _tasks.Values.OrderBy(t => t.Id).ToList();

    private void Add(TaskItem task) => _tasks[task.Id] = task;

    /// <summary>
    /// Завершує завдання. Підхід: ВИНЯТОК — повторне завершення вже виконаного завдання
    /// є порушенням бізнес-інваріанта (помилкою логіки викликаючого коду).
    /// </summary>
    /// <exception cref="TaskAlreadyCompletedException">Завдання вже виконано.</exception>
    /// <exception cref="KeyNotFoundException">Завдання з таким Id не існує.</exception>
    public void CompleteTask(int taskId)
    {
        if (!_tasks.TryGetValue(taskId, out TaskItem? task))
        {
            throw new KeyNotFoundException($"Завдання #{taskId} не знайдено.");
        }

        if (task.IsCompleted)
        {
            throw new TaskAlreadyCompletedException(taskId);
        }

        task.IsCompleted = true;
    }

    /// <summary>
    /// Призначає виконавця. Підхід: RESULT — відсутність завдання чи порожнє ім'я
    /// є очікуваними сценаріями невдачі (наприклад, помилка введення користувача).
    /// </summary>
    public Result<bool> AssignTask(int taskId, string assignee)
    {
        if (!_tasks.TryGetValue(taskId, out TaskItem? task))
        {
            return Result<bool>.Failure($"Завдання #{taskId} не знайдено.");
        }

        if (string.IsNullOrWhiteSpace(assignee))
        {
            return Result<bool>.Failure("Ім'я виконавця не може бути порожнім.");
        }

        task.Assignee = assignee.Trim();
        return Result<bool>.Success(true);
    }
}
