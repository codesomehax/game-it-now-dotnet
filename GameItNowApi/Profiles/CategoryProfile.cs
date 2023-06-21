using AutoMapper;
using GameItNowApi.Data.Dto;
using GameItNowApi.Data.Model;
using GameItNowApi.Requests.Category;

namespace GameItNowApi.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryAdditionRequest, Category>()
            .ForMember(
                c => c.Games,
                opt => opt.MapFrom(car => new List<Game>())
            );
    }
}