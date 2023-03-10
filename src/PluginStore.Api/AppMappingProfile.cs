using AutoMapper;
using PluginStore.Api.Models;
using PluginStore.Api.Models.Dto;

namespace PluginStore.Api;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<RegisterDto, CreateUserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<PluginVersion, PluginVersionDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name));
        CreateMap<Plugin, PluginDto>();
    }
}