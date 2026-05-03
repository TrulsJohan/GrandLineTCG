using GrandLineTCG.interfaces;

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

    private void DisplayEventCard(IEvent e, int index)
    {
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine($"║ {index,2}. {e.Title,-36}║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine($"║ Status: {e.Status,-36}║");
        Console.WriteLine($"║ Slots:  {e.ParticipantsCount}/{e.MaxCapacity,-30}║");
        Console.WriteLine($"║ Place:  {e.Location,-36}║");

        if (e is Tournament t)
        {
            Console.WriteLine($"║ Game:   {t.GameType,-36}║");
            Console.WriteLine($"║ Type:   {t.Type,-36}║");
            Console.WriteLine($"║ Prize:  {t.Prize} ({t.PrizeType})".PadRight(47) + "║");
            Console.WriteLine($"║ Rating: {t.AverageRating:F1}/5 ({t.Reviews.Count} reviews)║");
        }
        else if (e is TradingEvent te)
        {
            Console.WriteLine($"║ Table fee:    {te.TableFee:N0} kr".PadRight(47) + "║");
            Console.WriteLine($"║ Vendor slots: {te.VendorSlots,-26}║");
            var rarities = string.Join(", ", te.AllowedRarities);
            Console.WriteLine($"║ Rarities: {rarities,-32}║");
            Console.WriteLine($"║ Rating: {te.AverageRating:F1}/5 ({te.Reviews.Count} reviews)║");
        }

        Console.WriteLine("╠══════════════════════════════════════════════╣");

        var lines = SplitText(e.Description, 38);
        foreach (var line in lines)
            Console.WriteLine($"║ {line,-38} ║");

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
        Console.WriteLine("HOSTED EVENTS:");

        if (!user.Host.Any())
        {
            Console.WriteLine("No events hosted yet.\n");
            Console.ReadLine();
            return;
        }

        for (int i = 0; i < user.Host.Count; i++)
        {
            var e = (BaseEvent)user.Host[i];
            e.UpdateStatus();
            DisplayEventCard(e, i + 1);
        }

        Console.WriteLine($"{user.Host.Count + 1}. Go Back");

        int choice = ReadIntInRange("Select a event: ", 1, user.Host.Count + 1);

        if (choice == user.Host.Count + 1)
            return;

        ShowHostOptions(user, (BaseEvent)user.Host[choice - 1]);
    }

    private void ShowHostOptions(User user, BaseEvent @event)
    {
        Console.Clear();

        Console.WriteLine($"{@event.Title} [{@event.Status}]");
        Console.WriteLine($"{@event.Location}");
        Console.WriteLine(@event.Description);
        Console.WriteLine();

        Console.WriteLine("1. Update Event");
        Console.WriteLine("2. Cancel Event");
        Console.WriteLine("3. Go Back");

        int choice = ReadIntInRange("Select an option: ", 1, 3);

        switch (choice)
        {
            case 1:
                UpdateEvent(@event);
                break;

            case 2:
                try
                {
                    _controller.CancelEvent(user, @event);
                    Console.WriteLine("Event cancelled successfully.");
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

    private void UpdateEvent(BaseEvent @event)
    {
        Console.WriteLine("Enter new title (leave blank to keep current):");
        string? title = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(title))
            @event.Title = title;

        Console.WriteLine("Enter new description (leave blank to keep current):");
        string? description = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(description))
            @event.Description = description;

        Console.WriteLine("Event updated.");
        Console.ReadLine();
    }

    private void LeaveEvent(User user, Booking booking)
    {
        if (booking.Status == BookingStatus.Cancelled)
        {
            Console.WriteLine("This booking is already cancelled.");
            Console.ReadLine();
            return;
        }

        try
        {
            booking.Cancel();

            booking.Event.RemoveParticipant(user);
            user.Attending.Remove(booking.Event);

            Console.WriteLine("Your booking has been cancelled and your ticket has been refunded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.ReadLine();
    }

    private void WriteReview(User user, BaseEvent @event)
    {
        if (@event.Host == user)
        {
            Console.WriteLine("You cannot review your own event.");
            Console.ReadLine();
            return;
        }

        if (!user.Attending.Contains(@event))
        {
            Console.WriteLine("You can only review events you attended.");
            Console.ReadLine();
            return;
        }

        if (DateTime.Now < @event.EventDate)
        {
            Console.WriteLine("You can only review after the event has ended.");
            Console.ReadLine();
            return;
        }

        bool alreadyReviewed = @event.Reviews.Any(r => r.Author == user);

        if (alreadyReviewed)
        {
            Console.WriteLine("You have already reviewed this event.");
            Console.ReadLine();
            return;
        }

        int rating = ReadIntInRange("Enter rating (1-5): ", 1, 5);

        Console.WriteLine("Write an optional comment (press Enter to skip):");
        string? comment = Console.ReadLine();

        var review = new Review
        {
            Author = user,
            Event = @event,
            Rating = rating,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment
        };

        @event.Reviews.Add(review);

        Console.WriteLine("Review submitted!");
        Console.ReadLine();
    }

    private void ShowBookingOptions(User user, Booking booking)
    {
        Console.Clear();

        var e = booking.Event;

        Console.WriteLine($"{e.Title}");
        Console.WriteLine($"{e.Location}");
        Console.WriteLine($"Ticket: {booking.TicketType.Name}");
        Console.WriteLine($"Status: {booking.Status}");
        Console.WriteLine();

        Console.WriteLine("1. Leave Event");
        Console.WriteLine("2. Write Review");
        Console.WriteLine("3. Go Back");

        int choice = ReadIntInRange("Select an option: ", 1, 3);

        switch (choice)
        {
            case 1: LeaveEvent(user, booking); break;
            case 2: WriteReview(user, e as BaseEvent); break;
            default: return;
        }
    }

    private void ShowMyBookings(User user)
    {
        Console.Clear();
        Console.WriteLine("MY BOOKINGS:\n");

        var bookings = user.Purchases
            .Where(b => b.Status == BookingStatus.Confirmed)
            .ToList();

        if (!bookings.Any())
        {
            Console.WriteLine("No bookings found.\n");
            Console.ReadLine();
            return;
        }

        for (int i = 0; i < bookings.Count; i++)
        {
            var b = bookings[i];
            var e = b.Event;

            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine($"║ {i + 1,2}. {e.Title,-36}║");
            Console.WriteLine("╠══════════════════════════════════════════════╣");
            Console.WriteLine($"║ Ticket: {b.TicketType.Name,-34}║");
            Console.WriteLine($"║ Price:  {b.PriceAtBooking:N0} kr║");
            Console.WriteLine($"║ Booked: {b.BookedAt:dd MMM yyyy}║");
            Console.WriteLine($"║ Booking Status: {b.Status,-34}║");
            Console.WriteLine($"║ Ref:    {b.Reference,-34}║");
            Console.WriteLine($"║ Place:  {e.Location,-36}║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        Console.WriteLine($"{bookings.Count + 1}. Go Back");

        int choice = ReadIntInRange("Select a booking: ", 1, bookings.Count + 1);

        if (choice == bookings.Count + 1)
            return;

        ShowBookingOptions(user, bookings[choice - 1]);
    }

    private void ShowMyReviewComments(User user)
    {
        Console.Clear();
        Console.WriteLine("REVIEWS ON YOUR EVENTS:\n");

        bool hasReviews = false;

        foreach (var e in user.Host)
        {
            var baseEvent = (BaseEvent)e;
            baseEvent.UpdateStatus();

            foreach (var review in baseEvent.Reviews)
            {
                hasReviews = true;
                Console.WriteLine("════════════════════════════════════");
                Console.WriteLine($"Event:   {baseEvent.Title}");
                Console.WriteLine($"Rating:  {review.Rating}/5");
                Console.WriteLine($"By:      {review.Author.Username}");
                Console.WriteLine(string.IsNullOrWhiteSpace(review.Comment)
                    ? "Comment: (no comment)"
                    : $"Comment: {review.Comment}");
                Console.WriteLine($"Date:    {review.CreatedAt:dd MMM yyyy}");
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
