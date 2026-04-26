namespace GrandLineTCG;

public class Profile
{
    private readonly Controller _controller;

    public Profile(Controller controller)
    {
        _controller = controller;
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
    
    private List<string> SplitText(string text, int maxLength)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in words)
        {
            if ((currentLine + word).Length > maxLength)
            {
                lines.Add(currentLine.Trim());
                currentLine = "";
            }

            currentLine += word + " ";
        }

        if (!string.IsNullOrWhiteSpace(currentLine))
            lines.Add(currentLine.Trim());

        return lines;
    }
    
    private void DisplayTournamentCard(Tournament t, int index)
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine($"║ {index,2}. {t.Title,-36}║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine($"║ Status: {t.Status,-36}║");
        Console.WriteLine($"║ Game:   {t.GameType,-36}║");
        Console.WriteLine($"║ Type:   {t.Type,-36}║");
        Console.WriteLine($"║ Slots:  {t.ParticipantsCount}/{(int)t.MaxParticipants,-30}║");
        Console.WriteLine($"║ Prize:  {t.Prize} ({t.PrizeType})".PadRight(47) + "║");
        Console.WriteLine($"║ Place:  {t.Location,-36}║");
        Console.WriteLine($"║ Rating: {t.AverageRating:F1}/5 ({t.Reviews.Count} reviews)║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        
        var lines = SplitText(t.Description, 38);
        foreach (var line in lines)
        {
            Console.WriteLine($"║ {line,-38} ║");
        }

        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();
    }

    public void Display(User user)
    {
        Console.Clear();

        DisplayHeader(user);

        Console.WriteLine();
        Console.WriteLine("1. My Events");
        Console.WriteLine("2. My Bookings");
        Console.WriteLine("3. Review Comments");
        Console.WriteLine("4. Go Back");

        int choice = ReadIntInRange("Select an option: ", 1, 4);

        switch (choice)
        {
            case 1: ShowMyEvents(user); break;
            case 2: ShowMyBookings(user); break;
            case 3: ShowMyReviewComments(user); break;
            default: return;
        }
    }

    private void DisplayHeader(User user)
    {
        Console.WriteLine($"║  {user.Username,-27}║");
        Console.WriteLine($"║  Member since: {user.AccountCreated:dd MMM yyyy}║");
    }

    private void ShowMyEvents(User user)
    {
        Console.Clear();
        Console.WriteLine("HOSTED TOURNAMENTS:");

        if (!user.Host.Any())
        {
            Console.WriteLine("No tournaments hosted yet.\n");
            Console.ReadLine();
            return;
        }

        for (int i = 0; i < user.Host.Count; i++)
        {
            var t = user.Host[i];
            t.UpdateStatus();
            DisplayTournamentCard(t, i + 1);
        }

        Console.WriteLine($"{user.Host.Count + 1}. Go Back");

        int choice = ReadIntInRange("Select a tournament: ", 1, user.Host.Count + 1);

        if (choice == user.Host.Count + 1)
            return;

        var selectedTournament = user.Host[choice - 1];
        ShowHostOptions(user, selectedTournament);
    }

    private void ShowHostOptions(User user, Tournament tournament)
    {
        Console.Clear();

        Console.WriteLine($"{tournament.Title} [{tournament.Status}]");
        Console.WriteLine($"{tournament.Location} | {tournament.GameType}");
        Console.WriteLine(tournament.Description);
        Console.WriteLine();

        Console.WriteLine("1. Update Event");
        Console.WriteLine("2. Cancel Event");
        Console.WriteLine("3. Go Back");

        int choice = ReadIntInRange("Select an option: ", 1, 3);

        switch (choice)
        {
            case 1:
                UpdateTournament(tournament);
                break;

            case 2:
                try
                {
                    _controller.CancelTournament(user, tournament);
                    Console.WriteLine("Tournament cancelled successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
                break;

            default:
                return;
        }
    }

    private void UpdateTournament(Tournament tournament)
    {
        Console.WriteLine("Enter new title (leave blank to keep current):");
        string? title = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(title))
            tournament.Title = title;

        Console.WriteLine("Enter new description (leave blank to keep current):");
        string? description = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(description))
            tournament.Description = description;

        Console.WriteLine("Tournament updated.");
        Console.ReadLine();
    }
    
    private void LeaveTournament(User user, Tournament tournament)
    {
        if (!user.Attending.Contains(tournament))
        {
            Console.WriteLine("You are not registered for this tournament.");
            Console.ReadLine();
            return;
        }
        
        user.Attending.Remove(tournament);
        tournament.RemoveParticipant(user);

        Console.WriteLine("You have left the tournament.");
        Console.ReadLine();
    }
    
    private void WriteReview(User user, Tournament tournament)
    {
        if (tournament.Host == user)
        {
            Console.WriteLine("You cannot review your own event.");
            Console.ReadLine();
            return;
        }

        if (!user.Attending.Contains(tournament))
        {
            Console.WriteLine("You can only review tournaments you attended.");
            Console.ReadLine();
            return;
        }

        if (DateTime.Now < tournament.EventDate)
        {
            Console.WriteLine("You can only review after the event has ended.");
            Console.ReadLine();
            return;
        }
        
        bool alreadyReviewed = tournament.Reviews.Any(r => r.Author == user);

        if (alreadyReviewed)
        {
            Console.WriteLine("You have already reviewed this tournament.");
            Console.ReadLine();
            return;
        }

        int rating = ReadIntInRange("Enter rating (1-5): ", 1, 5);

        Console.WriteLine("Write an optional comment (press Enter to skip):");
        string? comment = Console.ReadLine();

        var review = new Review
        {
            Author = user,
            Tournament = tournament,
            Rating = rating,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment
        };

        tournament.Reviews.Add(review);

        Console.WriteLine("Review submitted!");
        Console.ReadLine();
    }
    
    private void ShowBookingOptions(User user, Tournament tournament)
    {
        Console.Clear();

        Console.WriteLine($"{tournament.Title}");
        Console.WriteLine($"{tournament.Location} | {tournament.GameType}");
        Console.WriteLine();

        Console.WriteLine("1. Leave Tournament");
        Console.WriteLine("2. Write Review");
        Console.WriteLine("3. Go Back");

        int choice = ReadIntInRange("Select an option: ", 1, 3);

        switch (choice)
        {
            case 1: LeaveTournament(user, tournament); break;
            case 2: WriteReview(user, tournament); break;
            default: return;
        }
    }

    private void ShowMyBookings(User user)
    {
        Console.Clear();
        Console.WriteLine("ATTENDING TOURNAMENTS:");

        if (!user.Attending.Any())
        {
            Console.WriteLine("No tournaments attending.\n");
            Console.ReadLine();
            return;
        }

        for (int i = 0; i < user.Attending.Count; i++)
        {
            var t = user.Attending[i];
            t.UpdateStatus();
            DisplayTournamentCard(t, i + 1);
        }

        Console.WriteLine($"{user.Attending.Count + 1}. Go Back");

        int choice = ReadIntInRange("Select a tournament: ", 1, user.Attending.Count + 1);

        if (choice == user.Attending.Count + 1)
            return;

        var selectedTournament = user.Attending[choice - 1];
        ShowBookingOptions(user, selectedTournament);
    }

    private void ShowMyReviewComments(User user)
    {
        Console.Clear();
        Console.WriteLine("REVIEWS ON YOUR EVENTS:\n");

        bool hasReviews = false;

        foreach (var tournament in user.Host)
        {
            tournament.UpdateStatus();

            foreach (var review in tournament.Reviews)
            {
                hasReviews = true;

                Console.WriteLine("════════════════════════════════════");
                Console.WriteLine($"Event: {tournament.Title}");
                Console.WriteLine($"Rating: {review.Rating}/5");
                Console.WriteLine($"By: {review.Author.Username}");

                if (!string.IsNullOrWhiteSpace(review.Comment))
                    Console.WriteLine($"Comment: {review.Comment}");
                else
                    Console.WriteLine("Comment: (no comment)");

                Console.WriteLine($"Date: {review.CreatedAt:dd MMM yyyy}");
            }
        }

        if (!hasReviews)
        {
            Console.WriteLine("No reviews yet on your events.");
        }

        Console.WriteLine("\nPress Enter to go back...");
        Console.ReadLine();
    }
}