using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Data.Requests.AppUser;

namespace GameItNowApi.Profiles;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, AppUserDto>()
            .ForMember(
                dto => dto.Role,
                opt => opt.MapFrom(user => user.Role.ToString()))
            .ForMember(
                dto => dto.Games,
                opt => opt.MapFrom(user => user.OwnedGames.Select(g => g.Name)));
        
        CreateMap<AppUserRegistrationRequest, AppUser>()
            .ForMember(
                appUser => appUser.Id,
                opt => opt.MapFrom(request => 0))
            .ForMember(
                appUser => appUser.Password,
                opt => opt.MapFrom(request => BCrypt.Net.BCrypt.HashPassword(request.Password)))
            .ForMember(
                appUser => appUser.Role,
                opt => opt.MapFrom(request => AppUserRole.User))
            .ForMember(
                appUser => appUser.OwnedGames,
                opt => opt.MapFrom(request => new List<Game>()));
    }
}