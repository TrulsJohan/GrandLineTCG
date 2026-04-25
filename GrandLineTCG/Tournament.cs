namespace GrandLineTCG;

public class Tournament
{
    public Guid Id { get; set; }  = Guid.NewGuid();
    public User Host { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string Location { get; set; } = String.Empty;
    public decimal Prize { get; set; }
    public ListingType GameType { get; set; }
    public TournamentType Type { get; set; }
    public EventStatus Status { get; set; }
    public PrizeType PrizeType { get; set; }
    public Ruleset Ruleset { get; set; }
    public MaxParticipants MaxParticipants { get; set; }
    public List<User> Participants { get; set; } = new List<User>();
    public int ParticipantsCount => Participants.Count;
    public bool IsFull => Participants.Count >= (int)MaxParticipants;
    public SlotStatus SlotStatus => IsFull ? SlotStatus.FullyBooked : SlotStatus.Available;

    public Tournament(
        User host,
        string title,
        string description,
        string location,
        int price,
        ListingType gameType,
        TournamentType tournamentType,
        PrizeType prizeType,
        Ruleset ruleset,
        MaxParticipants maxParticipants)
    {
        Host = host;
        Title = title;
        Description = description;
        Location = location;
        Prize = price;
        GameType = gameType;
        Type = tournamentType;
        PrizeType = prizeType;
        Ruleset = ruleset;
        MaxParticipants = maxParticipants;
        Status = EventStatus.Upcoming;
        Ruleset = Ruleset.Regular;
        PrizeType = PrizeType.Products;
    }
}