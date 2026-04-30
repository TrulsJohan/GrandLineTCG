namespace GrandLineTCG;

public abstract class BaseEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public User Host { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime EventDate { get; set; }
    public EventStatus Status { get; set; }
    public List<User> Participants { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();

    public abstract int MaxCapacity { get; }
    public int ParticipantsCount => Participants.Count;
    public bool IsFull => Participants.Count >= MaxCapacity;
    public SlotStatus SlotStatus => IsFull ? SlotStatus.FullyBooked : SlotStatus.Available;
    public double AverageRating => Reviews.Count == 0 ? 0 : Reviews.Average(r => r.Rating);

    protected BaseEvent(User host, string title, string description,
        string location, decimal price, DateTime eventDate)
    {
        Host = host;
        Title = title;
        Description = description;
        Location = location;
        Price = price;
        EventDate = eventDate;
        Status = EventStatus.Upcoming;
    }

    public bool AddParticipant(User user)
    {
        if (IsFull) return false;
        Participants.Add(user);
        return true;
    }

    public void RemoveParticipant(User user) => Participants.Remove(user);

    public void UpdateStatus()
    {
        if (Status == EventStatus.Cancelled || Status == EventStatus.Postponed)
            return;
        if (DateTime.Now >= EventDate)
        {
            Status = EventStatus.Completed;
            return;
        }
        Status = IsFull ? EventStatus.InProgress : EventStatus.Upcoming;
    }   
}