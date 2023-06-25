using GameItNowApi.Data.Model;

namespace GameItNowApi.Data.Dto;

public class AppUserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public List<string> Games { get; set; }
}