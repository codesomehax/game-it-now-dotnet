namespace GameItNowApi.Data.Model;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public AppUserRole Role { get; set; }
    
    public ICollection<Game> OwnedGames { get; set; }
    public ICollection<Game> Cart { get; set; }
}