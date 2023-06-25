using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;

namespace GameItNowApi.Profiles;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, AppUserDto>()
            .ForMember(
                dto => dto.Games,
                opt => opt.MapFrom(user => user.Games.Select(g => g.Name)));
    }
}