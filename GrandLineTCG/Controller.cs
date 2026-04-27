namespace GrandLineTCG;

public class Controller
{
    private readonly List<User> _users = new ();
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

    public Booking BookTournament(User user, Tournament tournament)
    {
        if (tournament.Host == user)
            throw new InvalidOperationException("You cannot book your own tournament.");

        if (tournament.IsFull)
            throw new InvalidOperationException("The tournament is full.");

        if (tournament.Participants.Contains(user))
            throw new InvalidOperationException("You are already booked to this tournament.");
        
        tournament.AddParticipant(user);
        tournament.UpdateStatus();

        var booking = new Booking(user, tournament, tournament.Prize);
        user.Purchases.Add(booking);
        user.Attending.Add(tournament);
        return booking;
    }

    public void CancelBooking(User user, Booking booking)
    {
        if (booking.Participant != user)
            throw new InvalidOperationException("You can only cancel your own bookings.");
        
        booking.Cancel();
        booking.Tournament.RemoveParticipant(user);
        booking.Tournament.UpdateStatus();
        user.Attending.Remove(booking.Tournament);
    }

    public void CancelTournament(User user, Tournament tournament)
    {
        if (tournament.Host != user)
            throw new InvalidOperationException("You can only cancel your tournaments.");
        
        tournament.Status = EventStatus.Cancelled;

        foreach (var participant in tournament.Participants)
            participant.Attending.Remove(tournament);
        
        tournament.Participants.Clear();
    }
    
}