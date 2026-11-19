using System.Runtime.Serialization;

namespace lab19v14.Exceptions;

/// <summary>Виняток: спроба завершити завдання, яке вже виконано.</summary>
[Serializable]
public class TaskAlreadyCompletedException : Exception
{
    /// <summary>Ідентифікатор завдання, яке вже виконано.</summary>
    public int TaskId { get; }

    public TaskAlreadyCompletedException()
    {
    }

    public TaskAlreadyCompletedException(string message)
        : base(message)
    {
    }

    public TaskAlreadyCompletedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public TaskAlreadyCompletedException(int taskId)
        : base($"Завдання #{taskId} уже виконано.")
    {
        TaskId = taskId;
    }

    public TaskAlreadyCompletedException(int taskId, string message)
        : base(message)
    {
        TaskId = taskId;
    }

#pragma warning disable SYSLIB0051 // конструктор для серіалізації позначено застарілим у .NET 8
    protected TaskAlreadyCompletedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        TaskId = info.GetInt32(nameof(TaskId));
    }
#pragma warning restore SYSLIB0051

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);
        base.GetObjectData(info, context);
        info.AddValue(nameof(TaskId), TaskId);
    }
}
