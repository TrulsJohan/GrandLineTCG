namespace GrandLineTCG;

public class TradingEvent : BaseEvent
{
    public List<CardRarity> AllowedRarities { get; set; } = new();
    public decimal TableFee { get; set; }
    public int VendorSlots { get; set; }

    public override int MaxCapacity => VendorSlots;
    
    public TradingEvent(User host, string title, string description, string location,
        decimal price, DateTime eventDate, decimal tableFee, int vendorSlots,
        List<CardRarity> allowedRarities)
        : base(host, title, description, location, price, eventDate)
    {
        TableFee = tableFee;
        VendorSlots = vendorSlots;
        AllowedRarities = allowedRarities;
    }
}