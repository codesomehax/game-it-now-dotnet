using System.ComponentModel.DataAnnotations;

namespace GameItNowApi.Data.Requests.AppUser;

public class AppUserRegistrationRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Username { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Password2 { get; set; }
    [Required(AllowEmptyStrings = false)]
    [EmailAddress]
    public string Email { get; set; }
}