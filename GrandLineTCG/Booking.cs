namespace GrandLineTCG;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Reference => $"BK-{Id.ToString()[..5].ToUpper()}";
    public User Participant { get; set; }
    public Tournament Tournament { get; set; }
    public decimal PriceAtBooking { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.Now;
    public BookingStatus Status { get; set; }

    public Booking(User participant, Tournament tournament, decimal priceAtBooking)
    {
        Participant = participant;
        Tournament = tournament;
        PriceAtBooking = priceAtBooking;
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("This booking is already cancelled.");
        Status = BookingStatus.Cancelled;
    }
}