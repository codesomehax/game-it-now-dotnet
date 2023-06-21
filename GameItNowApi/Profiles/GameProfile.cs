using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Requests.Game;

namespace GameItNowApi.Profiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<Game, GameDto>()
            .ForMember(
                dto => dto.Categories,
                opt => opt.MapFrom(g => g.Categories.Select(c => c.Name))    
            );

        CreateMap<GameAdditionRequest, Game>()
            .ForMember(
                g => g.Id,
                opt => opt.MapFrom(ar => 0))
            .ForMember(
                g => g.Categories,
                opt => opt.MapFrom(ar => new List<Category>()));
    }
}