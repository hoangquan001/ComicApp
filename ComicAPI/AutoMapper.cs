using ComicApp.Models;
using AutoMapper;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Comic, ComicDTO>();
        CreateMap<Chapter, ChapterDTO>();
        CreateMap<ComicDTO, Comic>();
        CreateMap<ChapterDTO, Chapter>();
        //gENRE LITE
        CreateMap<Genre, GenreLiteDTO>();
        CreateMap<GenreLiteDTO, Genre>();
        CreateMap<Genre, GenreDTO>();
        CreateMap<GenreDTO, Genre>();

    }
}