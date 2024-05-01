using ComicAPI.Classes;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IDataService
{


    Task<List<ComicDTO>> GetAllComic();
}