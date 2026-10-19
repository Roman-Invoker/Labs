namespace IndependentWork5;

public class Order
{
    public int Id { get; }
    public Customer Customer { get; }
    public Product Product { get; }
    public int Quantity { get; }

    public decimal Total => Product.Price * Quantity;

    public Order(int id, Customer customer, Product product, int quantity)
    {
        Id = id;
        Customer = customer;
        Product = product;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return $"Замовлення №{Id}: {Customer.Name}, {Product.Name} x {Quantity} = {Total:F2} грн";
    }
}
