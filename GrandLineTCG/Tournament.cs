namespace GrandLineTCG;

public class Tournament :BaseEvent
{
    public ListingType GameType { get; set; }
    public TournamentType Type { get; set; }
    public PrizeType PrizeType { get; set; }
    public Ruleset Ruleset { get; set; }
    public MaxParticipants MaxParticipants { get; set; }
    public decimal Prize { get; set; }

    public override int MaxCapacity => TicketTypes.Any()
        ? TicketTypes.Sum(ticket => ticket.Quantity)
        : 0;

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
        MaxParticipants maxParticipants,
        DateTime eventDate)
        : base(host, title, description, location, price, eventDate)
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
        EventDate = eventDate;
        Status = EventStatus.Upcoming;
    }
}