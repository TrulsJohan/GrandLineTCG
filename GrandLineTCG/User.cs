namespace GrandLineTCG;

public class User
{
    public string Username { get; set; }
    
    private string Password { get; set; }
    
    public DateTime AccountCreated { get; set; } = DateTime.Now;

    public List<Events> Host { get; set; } = new();
   
    public List<Events> Attending { get; set; } = new();

    public List<Booking> Purchases { get; set; } = new();
    
    public List<Booking> Sales { get; set; } = new();

    public List<Review> ReviewsReceived { get; set; } = new();
    
    public User (string username, string password)
    {
        Username = username;
        Password = password;
    }
    
    public bool CheckPassword(string password)
    {
        return password == Password;
    }
}