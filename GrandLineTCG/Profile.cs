namespace GrandLineTCG;

public class Profile
{
    public void Display(User user)
    {
        Console.Clear();
        
        DisplayHeader(user);
        DisplayHostedTournaments(user);
        
        Console.WriteLine("\nPress any key to go back...");
        Console.ReadKey();
    }

    private void DisplayHeader(User user)
    {
        Console.WriteLine($"║  {user.Username,-27}║");
        Console.WriteLine($"║  Member since: {user.AccountCreated:dd MMM yyyy}║");
    }

    private void DisplayHostedTournaments(User user)
    {
        Console.WriteLine("HOSTED TOURNAMENTS:");

        if (!user.Host.Any())
        {
            Console.WriteLine("No tournaments hosted yet.\n");
            return;
        }

        foreach (var tournament in user.Host)
        {
            Console.WriteLine($"{tournament.Title} [{tournament.Status}]");
            Console.WriteLine($"{tournament.Location} | {tournament.GameType}");
            Console.WriteLine($"{tournament.Description}");
            Console.WriteLine();
        }
    }
}