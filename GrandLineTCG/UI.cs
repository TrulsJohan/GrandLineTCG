using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace GrandLineTCG;

public class UI
{
    private readonly Controller _controller;
    private User? _currentUser;

    public UI(Controller controller)
    {
        _controller = controller;
    }

    public void Run()
    {
        while (true)
        {
            if (_currentUser == null)
                ShowGuestMenu();
            else
                ShowMainMenu();
        }
    }

    public int ReadIntInRange(string prompt, int min, int max)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Please enter a valid number between {min} and {max}.");
        }
    }

    private string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input))
                return input;
            Console.WriteLine($"This field can not be empty.");
        }
    }

    private T ReadEnum<T>(string prompt) where T : struct, Enum
    {
        var values = Enum.GetValues<T>();
        Console.WriteLine(prompt);
        for (int i = 0; i < values.Length; i++)
            Console.WriteLine($"{i + 1} . {values[i]}");

        int choice = ReadIntInRange("Select: ", 1, values.Length);
        return values[choice - 1];
    }

    private void ShowGuestMenu()
    {
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Exit");

        int choice = ReadIntInRange("Select an option: ", 1, 3);
        switch (choice)
        {
            case 1: HandleRegister(); break;
            case 2: HandleLogin(); break;
            case 3:
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
                break;
        }

    }

    private void ShowMainMenu()
    {
        Console.WriteLine("1. Create Event");
        Console.WriteLine("2. Browse Events");
        Console.WriteLine("3. Search Events");
        Console.WriteLine("4. View Profile");
        Console.WriteLine("5. Exit");
        Console.WriteLine("6. Log Out");
        int choice = ReadIntInRange("Select an option: ", 1, 6);
        switch (choice)
        {
            case 1: HandleCreateEvent(); break;
            case 2: HandleBrowseTournaments(); break;
            case 3: HandleSearchTournaments(); break;
            case 4: HandleProfile(); break;
            case 5:     
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
                break;
            case 6: _currentUser = null; break;
        }
    }

    private void HandleRegister()
    {
        string username = ReadRequiredString("Username: ");
        string password = ReadRequiredString("Password: ");

        try
        {
            var user = _controller.Register(username, password);
            Console.WriteLine($"User {username} successfully registered.");
            _currentUser = user;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"User {username} failed to register. {e.Message}");
        }
    }

    private void HandleLogin()
    {
        {
            string username = ReadRequiredString("Username: ");
            string password = ReadRequiredString("Password: ");

            try
            {
                _currentUser = _controller.Login(username, password);
                Console.WriteLine($"Welcome back {_currentUser.Username}!");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"User {username} failed to Log in. {e.Message}");
            }
        }
    }

    private void HandleCreateEvent()
    {
        string title = ReadRequiredString("Title: ");
        string description = ReadRequiredString("Description: ");
        string location = ReadRequiredString("Location: ");
        int price = ReadIntInRange("Price: ", 0, int.MaxValue);
        ListingType gameTypes = ReadEnum<ListingType>("Game Types: ");
        TournamentType tournamentType = ReadEnum<TournamentType>("Tournament Type: ");
        PrizeType prizeType = ReadEnum<PrizeType>("Prize Type: ");
        Ruleset ruleset = ReadEnum<Ruleset>("Ruleset: ");
        MaxParticipants maxParticipants = ReadEnum<MaxParticipants>("Max Participants: ");

        var tournament = _controller.CreateTournament(
            _currentUser!,
            title,
            description,
            location,
            price,
            gameTypes,
            tournamentType,
            prizeType,
            ruleset,
            maxParticipants);
        Console.WriteLine("Tournament created successfully.");
    }

    private void HandleProfile()
    {
        var profile = new Profile(_controller);
        profile.Display(_currentUser!);
    }

    
    //browsing, searching and sorting tournaments
    
    private void HandleBrowseTournaments()
    {
        Console.WriteLine("1. Browse All Listings");
        Console.WriteLine("2. Filter By Category");

        int choice = ReadIntInRange("Select an option: ", 1, 2);

        List<Tournament> tournaments = _controller.GetAllTournaments();
        if (choice == 2)
        {
            tournaments = HandleFilterTournaments(tournaments);
        }

        ShowTournamentsTable(tournaments);
    }
    
    private List<Tournament> HandleFilterTournaments(List<Tournament> tournaments)
    {
        Console.WriteLine();
        Console.WriteLine("Filter by:");
        Console.WriteLine("1. Game type");
        Console.WriteLine("2. Tournament type");
        Console.WriteLine("3. Status");
        Console.WriteLine("4. Prize type");
        Console.WriteLine("5. Ruleset");
        Console.WriteLine("6. Max participants");

        int choice = ReadIntInRange("Select an option: ", 1, 6);

        return choice switch
        {
            1 => tournaments.Where(t => t.GameType   == ReadEnum<ListingType>("Select game type:")).ToList(),
            2 => tournaments.Where(t => t.Type       == ReadEnum<TournamentType>("Select tournament type:")).ToList(),
            3 => tournaments.Where(t => t.Status     == ReadEnum<EventStatus>("Select status:")).ToList(),
            4 => tournaments.Where(t => t.PrizeType  == ReadEnum<PrizeType>("Select prize type:")).ToList(),
            5 => tournaments.Where(t => t.Ruleset    == ReadEnum<Ruleset>("Select ruleset:")).ToList(),
            6 => tournaments.Where(t =>t.MaxParticipants == ReadEnum<MaxParticipants>("Select Max Participants:")).ToList()
        };
    }
    
    //display tournaments list and single tournament

    private void ShowTournamentsTable(List<Tournament> tournaments)
    {
        if (tournaments.Count == 0)
        {
            Console.WriteLine("There are no tournaments in the list.");
        }

        Console.WriteLine();
        Console.WriteLine(
            $"{"#",4} {"Title",-25} {"Host",-15} {"Location",-15} {"Type",-14} {"Status",-12} {"Prize",8}");
        Console.WriteLine(new string('-', 97));

        for (int i = 0; i < tournaments.Count; i++)
        {
            var t = tournaments[i];
            string prize = t.PrizeType == PrizeType.Cash ? $"{t.Prize:N0} kr" : t.PrizeType.ToString();
            Console.WriteLine(
                $"{i + 1,-4} {t.Title,-25} {t.Host.Username,-15} {t.Location,-15} {t.Type,-14} {t.Status,-12} {prize,8}");
        }

        Console.WriteLine();
        int choice = ReadIntInRange("Select a listing to view (press 0 to go back): ", 0, tournaments.Count);
        if (choice == 0)
        {
            Console.WriteLine("No listings found!");
            return;
        }

        ShowTournamentDetails(tournaments[choice - 1]);
    }
    
    private void ShowTournamentDetails(Tournament tournament)
    {
        Console.WriteLine($"Title:        {tournament.Title}");
        Console.WriteLine($"Host:         {tournament.Host.Username}");
        Console.WriteLine($"Location:     {tournament.Location}");
        Console.WriteLine($"Type:         {tournament.Type}");
        Console.WriteLine($"Game:         {tournament.GameType}");
        Console.WriteLine($"Ruleset:      {tournament.Ruleset}");
        Console.WriteLine($"Status:       {tournament.Status}");
        Console.WriteLine($"Prize:        {tournament.Prize:N0} kr ({tournament.PrizeType})");
        Console.WriteLine($"Max players:  {(int)tournament.MaxParticipants}");
        Console.WriteLine($"Participants: {tournament.ParticipantsCount}/{(int)tournament.MaxParticipants}");
        Console.WriteLine($"Slots:        {tournament.SlotStatus}");
        Console.WriteLine($"Description:  {tournament.Description}");
        Console.WriteLine();

        bool isOwnTournament = tournament.Host.Username == _currentUser!.Username;
        if (isOwnTournament)
        {
            Console.WriteLine("1. Cancel tournament");
            Console.WriteLine("2. Go back");
            int choice = ReadIntInRange("Select an option: ", 1, 2);
            if (choice == 1)
                _controller.CancelTournament(_currentUser!, tournament);
            return;
        }

        Console.WriteLine("1. Join tournament");
        Console.WriteLine("2. Go back");
        int joinChoice = ReadIntInRange("Select an option: ", 1, 2);
        if (joinChoice == 1)
        {
            try
            {
                var booking = _controller.BookTournament(_currentUser!, tournament);
                Console.WriteLine("Booking complete");
                Console.WriteLine($"Reference: {booking.Reference}");
                Console.WriteLine($"Price: {booking.PriceAtBooking:N0} kr");
                Console.WriteLine($"Booked: {booking.BookedAt:dd MMM yyyy}");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Booking failed: {e.Message}");
            }
        }
    }
    
    private void HandleSearchTournaments()
    {
        Console.WriteLine("Search listings");
        string searchTerm = ReadRequiredString("Search Term: ");
        
        var tournaments = _controller.SearchTournaments(searchTerm);
        ShowTournamentsTable(tournaments);
    }
}