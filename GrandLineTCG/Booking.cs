using GrandLineTCG.interfaces;

namespace GrandLineTCG;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference => $"BK-{Id.ToString()[..5].ToUpper()}";
    public User Participant { get; set; }
    public IEvent Event { get; set; }
    public TicketType TicketType { get; set; }
    public decimal PriceAtBooking { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.Now;
    public BookingStatus Status { get; set; }

    public Booking(User participant, IEvent @event, TicketType ticketType)
    {
        Participant = participant;
        Event = @event;
        TicketType = ticketType;
        PriceAtBooking = ticketType.Price;
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("This booking is already cancelled.");
        Status = BookingStatus.Cancelled;
        TicketType.Release();
    }
}