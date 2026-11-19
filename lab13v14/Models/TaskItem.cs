namespace lab19v14.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? Assignee { get; set; }

    public override string ToString() =>
        $"#{Id} \"{Title}\" | виконано: {(IsCompleted ? "так" : "ні")} | виконавець: {Assignee ?? "—"}";
}
