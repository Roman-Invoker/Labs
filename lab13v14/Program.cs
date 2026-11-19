using System.Text;
using lab19v14.Common;
using lab19v14.Exceptions;
using lab19v14.Services;

// Лабораторна робота №13. Власні винятки та патерн Result.
// Варіант 14: Управління задачами.
//   Виняток:  TaskAlreadyCompletedException (int TaskId)
//   Service:  TaskService
//     void CompleteTask(int taskId)                       — кидає виняток, якщо завдання вже виконано
//     Result<bool> AssignTask(int taskId, string assignee) — Result.Failure, якщо завдання не знайдено

Console.OutputEncoding = Encoding.UTF8;

var service = new TaskService();

Header("Початковий стан");
PrintAll();

// ---------------------------------------------------------------
Header("1. CompleteTask: власний виняток (try-catch)");

TryComplete(2);   // успіх
TryComplete(1);   // уже виконано → TaskAlreadyCompletedException
TryComplete(2);   // повторне завершення → TaskAlreadyCompletedException
TryComplete(99);  // не існує → KeyNotFoundException

// ---------------------------------------------------------------
Header("2. AssignTask: Result<bool> (IsSuccess / Error)");

HandleAssign(service.AssignTask(3, "Роман"));
HandleAssign(service.AssignTask(99, "Роман"));
HandleAssign(service.AssignTask(3, "   "));

// ---------------------------------------------------------------
Header("3. Result<T>.Failure(Exception)");

Result<int> parsed = ParseInt("abc");
Console.WriteLine($"ParseInt(\"abc\") -> IsSuccess = {parsed.IsSuccess}, Error = {parsed.Error}");

Header("Підсумковий стан");
PrintAll();

// ---------------------------------------------------------------
void TryComplete(int taskId)
{
    try
    {
        service.CompleteTask(taskId);
        Console.WriteLine($"CompleteTask({taskId}): завдання завершено.");
    }
    catch (TaskAlreadyCompletedException ex)
    {
        Console.WriteLine($"CompleteTask({taskId}): TaskAlreadyCompletedException (TaskId = {ex.TaskId}) — {ex.Message}");
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"CompleteTask({taskId}): KeyNotFoundException — {ex.Message}");
    }
}

static void HandleAssign(Result<bool> result)
{
    if (result.IsSuccess)
    {
        Console.WriteLine($"AssignTask: успіх (Value = {result.Value}).");
    }
    else
    {
        Console.WriteLine($"AssignTask: невдача — {result.Error}");
    }
}

static Result<int> ParseInt(string input)
{
    try
    {
        return Result<int>.Success(int.Parse(input));
    }
    catch (FormatException ex)
    {
        return Result<int>.Failure(ex);
    }
}

void PrintAll()
{
    foreach (var task in service.GetAll())
    {
        Console.WriteLine($"  {task}");
    }
}

static void Header(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', 70));
}
