public class Course : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Credits { get; set; }
    public int TeacherId { get; set; }
}
