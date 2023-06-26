using GameItNowApi.Data.Dto;

namespace GameItNowApi.Data.Responses;

public class AuthenticationResponse
{
    public AuthenticationResponse(string token, AppUserDto appUser)
    {
        Token = token;
        AppUser = appUser;
    }

    public string Token { get; set; }
    public AppUserDto AppUser { get; set; }
}