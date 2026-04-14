namespace GrandLineTCG;

public class Controller
{
    private readonly List<User> _users = new ();
    
    public User Register(string username, string password)
    {
        if (_users.Any(u => u.Username == username))
            throw new InvalidOperationException($"Username '{username}' is aleready taken. Please enter a new one");
        
        var user = new User(username, password);
        _users.Add(user);
        return user;
    }
}