using System.ComponentModel.DataAnnotations;

namespace GameItNowApi.Data.Requests.Authentication;

public class AuthenticationRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Username { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; }
}