namespace GrandLineTCG;

public class Tournament
{
    public Guid Id { get; set; }  = Guid.NewGuid();
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public ListingType GameType { get; set; }
    public List<Participant> Participants { get; set; } = new List<Participant>();
    public int ParticipantsCount => Participants.Count;
    public string Location { get; set; } = String.Empty;
    public decimal Prize { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TournamentType Type { get; set; }
    public EventStatus Status { get; set; }
    public PrizeType PrizeType { get; set; }
    public Ruleset Ruleset { get; set; }
    
    // Add later:
    // public string Host { get; set; } = _currentUser;
    // public string HostId { get; set; }
    // public string Address { get; set; }
    // public int MaxParticipants { get; set; }

    public Tournament()
    {
        Status = EventStatus.Upcoming;
        Type = TournamentType.Casual;
        Ruleset = Ruleset.Regular;
        PrizeType = PrizeType.Products;
    }
}