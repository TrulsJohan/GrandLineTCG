namespace GrandLineTCG;

public class Tournament :BaseEvent
{
    public ListingType GameType { get; set; }
    public TournamentType Type { get; set; }
    public PrizeType PrizeType { get; set; }
    public string PrizeDescription { get; set; } = string.Empty; 
    public Ruleset Ruleset { get; set; }
    public decimal Prize { get; set; }
    public override int MaxCapacity => TicketTypes.Any()
        ? TicketTypes.Sum(ticket => ticket.Quantity)
        : 0;

    public Tournament(
        User host,
        string title,
        string description,
        string location,
        ListingType gameType,
        TournamentType tournamentType,
        PrizeType prizeType,
        string prizeDescription,
        Ruleset ruleset,
        DateTime eventDate)
        : base(host, title, description, location, 0, eventDate)
    {
        Host = host;
        Title = title;
        Description = description;
        Location = location;
        GameType = gameType;
        Type = tournamentType;
        PrizeType = prizeType;
        PrizeDescription = prizeDescription;
        Ruleset = ruleset;
        EventDate = eventDate;
        Status = EventStatus.Upcoming;
    }
}