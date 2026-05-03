using GrandLineTCG.interfaces;

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
    
    private DateTime ReadEventDate(string prompt)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string? input = Console.ReadLine();

            if (DateTime.TryParse(input, out DateTime date))
                return date;

            Console.WriteLine("Invalid date format. Example: 2026-12-31 18:00");
        }
    }

    private TicketType? SelectTicketType(IEvent @event)
    {
        if (!@event.TicketTypes.Any())
        {
            Console.WriteLine("No ticket types available.");
            return null;
        }

        Console.WriteLine("Available ticket types:");
        for (int i = 0; i < @event.TicketTypes.Count; i++)
        {
            var t = @event.TicketTypes[i];
            if (t.IsAvailable)
                Console.WriteLine($"{i + 1}. {t.Name} — {t.Price:N0} kr ({t.Remaining} remaining)");
            else
                Console.WriteLine($"{i + 1}. {t.Name} — SOLD OUT");
        }

        int choice = ReadIntInRange("Select a ticket type: ", 1, @event.TicketTypes.Count);
        var selected = @event.TicketTypes[choice - 1];

        if (!selected.IsAvailable)
        {
            Console.WriteLine("That ticket type is sold out. Please select another.");
            return null;
        }

        return selected;
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
            case 2: HandleBrowseEvents(); break;
            case 3: HandleSearchEvents(); break;
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
        Console.WriteLine("What type of event would you like to create?");
        Console.WriteLine("1. Tournament");
        Console.WriteLine("2. Trading Event");

        int choice = ReadIntInRange("Select: ", 1, 2);

        if (choice == 1)
            HandleCreateTournament();
        else
            HandleCreateTradingEvent();   
    }
    
    private void HandleCreateTournament()
    {
        string title = ReadRequiredString("Title: ");
        string description = ReadRequiredString("Description: ");
        string location = ReadRequiredString("Location: ");
        ListingType gameTypes = ReadEnum<ListingType>("Game Types: ");
        TournamentType tournamentType = ReadEnum<TournamentType>("Tournament Type: ");
        PrizeType prizeType = ReadEnum<PrizeType>("Prize Type: ");
        string prizeDescription = prizeType switch
        {
            PrizeType.Cash => $"{ReadIntInRange("Prize amount (kr): ", 0, int.MaxValue)} kr",
            PrizeType.Products => ReadRequiredString("Describe the prize (e.g. Packs, Trophy"),
            PrizeType.Honor => "Honor and glory!",
            PrizeType.Mixed => ReadRequiredString("Describe the prize (e.g. 500kr + 3 Packs"),
            _ => string.Empty
        }; 
        Ruleset ruleset = ReadEnum<Ruleset>("Ruleset: ");
        DateTime eventDate = ReadEventDate("Event Date (yyyy-MM-dd HH:mm): ");

        Console.WriteLine("\nNow set the price and quantity for each ticket type:");
        var ticketPrices = new Dictionary<string, (int price, int quantity)>();
        string[] ticketNames = { "Early Bird", "Standard", "VIP" };
        foreach (var name in ticketNames)
        {
            Console.WriteLine($"\n{name} ticket:");
            int ticketPrice = ReadIntInRange("Price: ", 0, int.MaxValue);
            int quantity = ReadIntInRange("Quantity: ", 1, int.MaxValue);
            ticketPrices[name] = (ticketPrice, quantity);
        }

        var tournament = _controller.CreateTournament(
            _currentUser!,
            title,
            description,
            location,
            gameTypes,
            tournamentType,
            prizeType,
            prizeDescription,
            ruleset,
            eventDate);

        foreach (var (name, info) in ticketPrices)
            _controller.AddTicketType(_currentUser!, tournament, name, info.price, info.quantity);

        Console.WriteLine("Tournament created successfully.");
    }

    private void HandleCreateTradingEvent()
    {
        string title = ReadRequiredString("Title: ");
        string description = ReadRequiredString("Description: ");
        string location = ReadRequiredString("Location: ");
        int price = ReadIntInRange("Entry price: ", 0, int.MaxValue);
        int tableFee = ReadIntInRange("Table fee: ", 0, int.MaxValue);
        int vendorSlots = ReadIntInRange("Vendor slots: ", 1, 500);
        DateTime eventDate = ReadEventDate("Event Date (yyyy-MM-dd HH:mm): ");

        var rarities = new List<CardRarity>();
        Console.WriteLine("Add allowed rarities (select one at a time, enter 0 when done):");
        while (true)
        {
            CardRarity rarity = ReadEnum<CardRarity>("Rarity: ");
            if (!rarities.Contains(rarity))
                rarities.Add(rarity);
            Console.WriteLine("Add another rarity? (1 = Yes, 2 = No)");
            if (ReadIntInRange("Select: ", 1, 2) == 2) break;
        }

        var tradingEvent = _controller.CreateTradingEvent(
            _currentUser!, title, description, location, price,
            eventDate, tableFee, vendorSlots, rarities);

        Console.WriteLine("\nNow set the price and quantity for each ticket type:");
        foreach (var name in new[] { "Vendor", "Visitor" })
        {
            Console.WriteLine($"\n{name} ticket:");
            int ticketPrice = ReadIntInRange("Price: ", 0, int.MaxValue);
            int quantity    = ReadIntInRange("Quantity: ", 1, int.MaxValue);
            _controller.AddTicketType(_currentUser!, tradingEvent, name, ticketPrice, quantity);
        }

        Console.WriteLine("Trading event created successfully.");
    }

    private void HandleProfile()
    {
        var profile = new Profile(_controller);
        profile.Display(_currentUser!);
    }

    
    //browsing, searching and sorting tournaments
    
    private void HandleBrowseEvents()
    {
        Console.WriteLine("1. Browse All Listings");
        Console.WriteLine("2. Filter By Category");

        int choice = ReadIntInRange("Select an option: ", 1, 2);

        List<IEvent> events = _controller.GetAllEvents();
        if (choice == 2)
        {
            events = HandleFilterEvents(events);
        }

        ShowEventsTable(events);
    }
    
    private List<IEvent> HandleFilterEvents(List<IEvent> events)
    {
        Console.WriteLine("\nFilter by:");
        Console.WriteLine("1. Event type (Tournament / Trading)");
        Console.WriteLine("2. Status");
        Console.WriteLine("3. Game type      (tournaments only)");
        Console.WriteLine("4. Prize type     (tournaments only)");
        Console.WriteLine("5. Ruleset        (tournaments only)");
        Console.WriteLine("6. Max participants (tournaments only)");

        int choice = ReadIntInRange("Select an option: ", 1, 6);

        return choice switch
        {
            1 => ReadIntInRange("1. Tournament  2. Trading Event", 1, 2) == 1
                ? events.OfType<Tournament>().Cast<IEvent>().ToList()
                : events.OfType<TradingEvent>().Cast<IEvent>().ToList(),
            2 => events.Where(e => e.Status == ReadEnum<EventStatus>("Select status:")).ToList(),
            3 => events.OfType<Tournament>()
                .Where(t => t.GameType == ReadEnum<ListingType>("Select game type:"))
                .Cast<IEvent>().ToList(),
            4 => events.OfType<Tournament>()
                .Where(t => t.PrizeType == ReadEnum<PrizeType>("Select prize type:"))
                .Cast<IEvent>().ToList(),
            5 => events.OfType<Tournament>()
                .Where(t => t.Ruleset == ReadEnum<Ruleset>("Select ruleset:"))
                .Cast<IEvent>().ToList(),
            6 => events.OfType<Tournament>()
                .Where(t => t.MaxCapacity == (int)ReadEnum<MaxParticipants>("Select max participants:"))
                .Cast<IEvent>().ToList(),
            _ => events
        };
    }
    
    //display tournaments list and single tournament

    private void ShowEventsTable(List<IEvent> events)
    {
        if (events.Count == 0)
        {
            Console.WriteLine("There are no events in the list.");
        }

        Console.WriteLine();
        Console.WriteLine(
            $"{"#",4} {"Title",-25} {"Host",-15} {"Location",-15} {"Kind",-14} {"Status",-12} {"Info",10}");
        Console.WriteLine(new string('-', 101));

        for (int i = 0; i < events.Count; i++)
        {
            var e = events[i];
            string kind = e is Tournament ? "Tournament" : "Trading";
            string info = e is Tournament t
                ? t.PrizeDescription
                : e is TradingEvent te ? $"Fee {te.TableFee:N0} kr" : "-";

            Console.WriteLine(
                $"{i + 1,-4} {e.Title,-25} {e.Host.Username,-15} {e.Location,-15} {kind,-14} {e.Status,-12} {info,10}");
        }

        Console.WriteLine();
        int choice = ReadIntInRange("Select a listing to view (press 0 to go back): ", 0, events.Count);
        if (choice == 0)
        {
            Console.WriteLine("No listings found!");
            return;
        }

        ShowEventDetails((BaseEvent)events[choice - 1]);
    }
    
    private void ShowEventDetails(BaseEvent @event)
    {
        Console.WriteLine($"Title:        {@event.Title}");
        Console.WriteLine($"Host:         {@event.Host.Username}");
        Console.WriteLine($"Location:     {@event.Location}");
        Console.WriteLine($"Status:       {@event.Status}");
        Console.WriteLine($"Participants: {@event.ParticipantsCount}/{@event.MaxCapacity}");
        Console.WriteLine($"Slots:        {@event.SlotStatus}");
        Console.WriteLine($"Date:         {@event.EventDate:dd MMM yyyy HH:mm}");
        Console.WriteLine($"Description:  {@event.Description}");

        if (@event is Tournament t)
        {
            Console.WriteLine($"Game:         {t.GameType}");
            Console.WriteLine($"Type:         {t.Type}");
            Console.WriteLine($"Ruleset:      {t.Ruleset}");
            Console.WriteLine($"Prize:        {t.PrizeDescription} ({t.PrizeType})");
        }
        else if (@event is TradingEvent te)
        {
            Console.WriteLine($"Table fee:    {te.TableFee:N0} kr");
            Console.WriteLine($"Vendor slots: {te.VendorSlots}");
            Console.WriteLine($"Rarities:     {string.Join(", ", te.AllowedRarities)}");
        }

        Console.WriteLine();

        bool isOwn = @event.Host.Username == _currentUser!.Username;
        if (isOwn)
        {
            Console.WriteLine("1. Cancel event");
            Console.WriteLine("2. Go back");
            int choice = ReadIntInRange("Select an option: ", 1, 2);
            if (choice == 1)
                _controller.CancelEvent(_currentUser!, @event);
            return;
        }

        Console.WriteLine("1. Join event");
        Console.WriteLine("2. Go back");
        int joinChoice = ReadIntInRange("Select an option: ", 1, 2);
        if (joinChoice == 1)
        {
            try
            {
                var ticketType = SelectTicketType(@event);
                if (ticketType == null) return;

                var booking = _controller.BookEvent(_currentUser!, @event, ticketType);
                Console.WriteLine("Booking confirmed!");
                Console.WriteLine($"Reference: {booking.Reference}");
                Console.WriteLine($"Ticket:    {booking.TicketType.Name}");
                Console.WriteLine($"Price:     {booking.PriceAtBooking:N0} kr");
                Console.WriteLine($"Booked:    {booking.BookedAt:dd MMM yyyy}");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Booking failed: {e.Message}");
            }
        }
    }
    
    private void HandleSearchEvents()
    {
        Console.WriteLine("Search listings");
        string searchTerm = ReadRequiredString("Search Term: ");
        
        var events = _controller.SearchEvents(searchTerm);
        ShowEventsTable(events);
    }
}