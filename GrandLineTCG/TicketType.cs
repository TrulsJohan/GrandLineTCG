namespace GrandLineTCG;

public class TicketType
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int Remaining { get; set; }

    public TicketCategory Category { get; set; }

    public bool IsAvailable => Remaining > 0;

    public TicketType(string name, decimal price, int quantity, TicketCategory category)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        Remaining = quantity;
        Category = category;
    }

    public bool Book()
    {
        if (!IsAvailable)
            return false;

        Remaining--;
        return true;
    }

    public void Release()
    {
        if (Remaining < Quantity)
            Remaining++;
    }
}