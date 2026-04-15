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
        Console.WriteLine("1. Create Tournament");
        Console.WriteLine("2. Exit");
        int choice = ReadIntInRange("Select an option: ", 1, 3);
        switch (choice)
        {
            case 1: HandleCreateEvent(); break;
            case 2:
                Console.WriteLine("Goodbye!");
                Environment.Exit(0);
                break;
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
        
    }
}