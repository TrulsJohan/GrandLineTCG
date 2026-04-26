namespace GrandLineTCG;

using BCrypt.Net;

public class User
{
    public string Username { get; set; }
    
    private string PasswordHash { get; set; }
    
    public DateTime AccountCreated { get; set; } = DateTime.Now;

    public List<Tournament> Host { get; set; } = new();
   
    public List<Tournament> Attending { get; set; } = new();

    public List<Booking> Purchases { get; set; } = new();
    
    public List<Booking> Sales { get; set; } = new();

    public List<Review> ReviewsReceived { get; set; } = new();
    
    public User (string username, string password)
    {
        Username = username;
        PasswordHash = BCrypt.HashPassword(password);
    }
    
    public bool CheckPassword(string password)
    {
        return BCrypt.Verify(password, PasswordHash);
    }
}