
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComicAPI.Services;
using Microsoft.Extensions.ObjectPool;

namespace ComicApp.Services;
public class UserService : IUserService
{
    readonly IDataService _dataService;
    readonly IComicReposibility _comicReposibility;
    ComicDbContext _dbContext;
    readonly ITokenMgr _tokenMgr;
    private int _UserID = -1;

    public int UserID
    {
        get
        {
            return _UserID;
        }
        set
        {
            _UserID = value;
        }
    }

    //Contructor
    public UserService(ComicDbContext db, ITokenMgr tokenMgr, IComicReposibility comicReposibility, IDataService dataService)
    {
        _comicReposibility = comicReposibility;
        _dataService = dataService;
        _dbContext = db;
        _tokenMgr = tokenMgr;

    }

    public async Task<ServiceResponse<int>> FollowComic(int userid, int comicid)
    {
        if (!_dbContext.Comics.Any(x => x.ID == comicid)) return new ServiceResponse<int> { Status = 0, Message = "Comic not found", Data = 0 };

        var user = await _dbContext.UserFollowComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
        if (user != null) return new ServiceResponse<int> { Status = 0, Message = "Already followed", Data = 0 };
        //check comicid exist


        _dbContext.UserFollowComics.Add(new UserFollowComic { UserID = userid, ComicID = comicid });
        await _dbContext.SaveChangesAsync();

        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 1 };
    }
    public async Task<ServiceResponse<int>> FollowComic(int comicid)
    {
        return await FollowComic(UserID, comicid);
    }

    public async Task<ServiceResponse<int>> UnFollowComic(int userid, int comicid)
    {
        if (!_dbContext.Comics.Any(x => x.ID == comicid)) return new ServiceResponse<int> { Status = 0, Message = "Comic not found", Data = 0 };

        var user = _dbContext.UserFollowComics.FirstOrDefault(x => x.UserID == userid && x.ComicID == comicid);
        if (user == null) return new ServiceResponse<int> { Status = 0, Message = "Not followed", Data = 0 };
        _dbContext.UserFollowComics.Remove(user);

        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 0 };
    }
    public async Task<ServiceResponse<int>> UnFollowComic(int comicid)
    {
        return await UnFollowComic(UserID, comicid);
    }

    public async Task<ServiceResponse<ListComicDTO>> GetFollowComics(int userid, int page = 1, int step = 40)
    {
        ListComicDTO? data = await _comicReposibility.GetUserFollowComics(userid, page, step);
        return ServiceUtilily.GetDataRes(data);
    }
    public async Task<ServiceResponse<ListComicDTO>> GetFollowComics(int page = 1, int step = 40)
    {
        return await GetFollowComics(UserID, page, step);
    }

    public async Task<bool> IsFollowComic(int userid, int comicid)
    {
        //check comicid exist

        var user = await _dbContext.UserFollowComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
        return user != null;

    }
    public async Task<bool> IsFollowComic(int comicid)
    {
        return await IsFollowComic(UserID, comicid);
    }

    public async Task<ServiceResponse<CommentDTO>> AddComment(int userid, string content, int chapterid, int parentcommentid = 0)
    {
        var chapter = _dbContext.Chapters.FirstOrDefault(x => x.ID == chapterid);
        if (chapter == null) return new ServiceResponse<CommentDTO> { Status = 0, Message = "Chapter not found", Data = null };

        Comment comment_data = new Comment
        {
            UserID = userid,
            Content = content,
            ChapterID = chapterid,
            ComicID = chapter.ComicID,
            ParentCommentID = parentcommentid == 0 ? null : parentcommentid
        };
        var commentData = _dbContext.Comments.Add(comment_data);
        await _dbContext.SaveChangesAsync();

        var cmtData = await _dbContext.Comments
        .Where(x => x.ID == commentData.Entity.ID)
        .Include(x => x.User)
        .Select(x => new CommentDTO
        {
            ID = x.ID,
            Content = x.Content,
            UserID = x.UserID,
            ChapterID = x.ChapterID,
            ComicID = x.ComicID,
            ParentCommentID = x.ParentCommentID,
            CommentedAt = x.CommentedAt,
            UserName = x.User!.FirstName + " " + x.User.LastName,
        })
        .FirstOrDefaultAsync();



        return new ServiceResponse<CommentDTO> { Status = 1, Message = "Success", Data = cmtData };

    }

    public async Task<ServiceResponse<CommentDTO>> AddComment(string content, int chapterid, int parentcommentid = 0)
    {
        return await AddComment(UserID, content, chapterid, parentcommentid);
    }

    public async Task<ServiceResponse<List<CommentDTO>>> GetCommentsOfComic(int comicid, int page = 1, int step = 40)
    {
        var data = await _dbContext.Comments
            .Where(x => x.ComicID == comicid && x.ParentCommentID == null)
            .OrderByDescending(x => x.CommentedAt)
            .Include(x => x.Replies)
            .Include(x => x.User)
            .Select(x => new CommentDTO
            {
                ID = x.ID,
                Content = x.Content,
                UserID = x.UserID,
                ChapterID = x.ChapterID,
                ComicID = x.ComicID,
                ParentCommentID = x.ParentCommentID,
                CommentedAt = x.CommentedAt,
                UserName = x.User!.FirstName + " " + x.User.LastName,
                Replies = x.Replies!.Select(y => new CommentDTO
                {
                    ID = y.ID,
                    Content = y.Content,
                    UserID = y.UserID,
                    ChapterID = y.ChapterID,
                    ComicID = y.ComicID,
                    ParentCommentID = y.ParentCommentID,
                    CommentedAt = y.CommentedAt,
                    UserName = y.User!.FirstName + " " + y.User.LastName
                }).ToList()
            })
            .Skip((page - 1) * step)
            .Take(step)
            .ToListAsync();
        return ServiceUtilily.GetDataRes<List<CommentDTO>>(data);
    }

    public Task<ServiceResponse<List<CommentDTO>>> GetCommentsOfChapter(int chapterid, int page = 1, int step = 40)
    {
        throw new NotImplementedException();
    }
}