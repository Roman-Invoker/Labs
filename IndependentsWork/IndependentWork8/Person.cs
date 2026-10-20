namespace IndependentWork8;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

/// <summary>
/// Custom comparer: дві людини вважаються однаковими, якщо збігаються імена.
/// Equals і GetHashCode мають бути узгодженими: однакові об'єкти мають давати однаковий хеш.
/// </summary>
public class PersonNameComparer : IEqualityComparer<Person>
{
    public bool Equals(Person? x, Person? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Name == y.Name;
    }

    public int GetHashCode(Person obj)
    {
        return StringComparer.Ordinal.GetHashCode(obj.Name);
    }
}
