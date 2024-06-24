using ComicApp.Models;
using AutoMapper;
using ComicAPI.Models;
using ComicAPI.DTOs;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Comic, ComicDTO>();
        CreateMap<Chapter, ChapterDTO>();
        CreateMap<ComicDTO, Comic>();
        CreateMap<ChapterDTO, Chapter>();
        // CreateMap<UserNotification, UserNotificationDTO>()
        //           .ForMember(dest => dest.comic, opt => opt.MapFrom(src => src.comic))
        //           .ForMember(dest => dest.user, opt => opt.MapFrom(src => src.user));

        CreateMap<User, UserDTO>();

        //gENRE LITE
        CreateMap<Genre, GenreLiteDTO>();
        CreateMap<GenreLiteDTO, Genre>();
        CreateMap<Genre, GenreDTO>();
        CreateMap<GenreDTO, Genre>();


    }
}