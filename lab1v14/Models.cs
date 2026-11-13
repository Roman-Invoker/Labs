namespace Lab16v14;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Budget { get; set; }

    public override string ToString() => $"Department {{ Id = {Id}, Name = {Name}, Budget = {Budget} }}";
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public decimal Salary { get; set; }

    public override string ToString() =>
        $"Employee {{ Id = {Id}, Name = {Name}, DepartmentId = {DepartmentId}, Salary = {Salary} }}";
}
