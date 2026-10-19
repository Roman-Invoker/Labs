namespace IndependentWork5;

public class Customer
{
    public string Name { get; }
    public string Email { get; }

    public Customer(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public override string ToString()
    {
        return $"Клієнт {Name} ({Email})";
    }
}
