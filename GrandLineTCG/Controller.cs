using GrandLineTCG.interfaces;

namespace GrandLineTCG;

public class Controller
{
    private readonly List<User> _users = new ();
    private readonly List<TradingEvent> _tradeEvents = new();
    private readonly List<Tournament> _tournaments = new();
    
    public User Register(string username, string password)
    {
        if (_users.Any(u => u.Username == username))
            throw new InvalidOperationException($"Username '{username}' is aleready taken. Please enter a new one");
        
        var user = new User(username, password);
        _users.Add(user);
        return user;
    }
    
    public User Login(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);

        if (user == null || !user.CheckPassword(password))
            throw new InvalidOperationException("invalid username or password");
        
        return user;
    }

    public TradingEvent CreateTradingEvent(User host, string title, string description,
        string location, decimal price, DateTime eventDate,
        decimal tableFee, int vendorSlots, List<CardRarity> allowedRarities)
    {
        var tradeEvent = new TradingEvent(host, title, description, location,
            price, eventDate, tableFee, vendorSlots, allowedRarities);
        host.Host.Add(tradeEvent);
        _tradeEvents.Add(tradeEvent);
        return tradeEvent;
    }

    public Tournament CreateTournament(
        User host,
        string title, 
        string description, 
        string location, 
        int price,  
        ListingType gameTypes, 
        TournamentType tournamentType, 
        PrizeType prizeType, 
        Ruleset ruleset, 
        MaxParticipants maxParticipants,
        DateTime eventDate)
    {
        var tournament = new Tournament(
            host,
            title,
            description,
            location,
            price,
            gameTypes,
            tournamentType,
            prizeType,
            ruleset,
            maxParticipants,
            eventDate);
        host.Host.Add(tournament);
        _tournaments.Add(tournament);
        return tournament;
    }
    
    // browse, search and sort listings

    public List<Tournament> GetAllTournaments()
    {
        return _tournaments
            .Where(t => t.Status == EventStatus.Upcoming)
            .ToList();
    }
    
    public List<Tournament> SearchTournaments(string searchTerm)
    {
        var lower = searchTerm.ToLower();
        return _tournaments
            .Where(t => t.Status == EventStatus.Upcoming &&
                        (t.Title.ToLower().Contains(lower) || 
                         t.Description.ToLower().Contains(lower) || 
                         t.Location.ToLower().Contains(lower)))
            .ToList();
    }

    public Booking BookEvent(User user, IEvent @event)
    {
        if (@event.Host == user)
            throw new InvalidOperationException("You cannot book your own tournament.");

        if (@event.IsFull)
            throw new InvalidOperationException("The tournament is full.");

        var baseEvent = (BaseEvent)@event;
        
        if (baseEvent.Participants.Contains(user))
            throw new InvalidOperationException("You are already booked to this tournament.");
        
        @event.AddParticipant(user);
        @event.UpdateStatus();

        var booking = new Booking(user, @event, @event.Price);
        user.Purchases.Add(booking);
        user.Attending.Add(@event);
        return booking;
    }

    public void CancelBooking(User user, Booking booking)
    {
        if (booking.Participant != user)
            throw new InvalidOperationException("You can only cancel your own bookings.");
        
        booking.Cancel();
        booking.Event.RemoveParticipant(user);
        booking.Event.UpdateStatus();
        user.Attending.Remove(booking.Event);
    }

    public void CancelTournament(User user, IEvent @event)
    {
        if (@event.Host != user)
            throw new InvalidOperationException("You can only cancel your tournaments.");
        
        @event.Status = EventStatus.Cancelled;
       
        var baseEvent = (BaseEvent)@event;

        foreach (var participant in @baseEvent.Participants)
            participant.Attending.Remove(@event);
        
        baseEvent.Participants.Clear();
    }
    
}