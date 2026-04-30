namespace GrandLineTCG;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public User Author { get; set; }
    public BaseEvent Event { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}