
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
using ComicAPI.DTOs;
using Azure;
using AutoMapper;
// using static System.Net.Mime.MediaTypeNames;


using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ComicApp.Services;
public class UserService : IUserService
{
    readonly IDataService _dataService;
    readonly IComicReposibility _comicReposibility;
    ComicDbContext _dbContext;
    readonly ITokenMgr _tokenMgr;
    private int _UserID = -1;
    private readonly IUserReposibility _userReposibility;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;
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
    public UserService(ComicDbContext db, ITokenMgr tokenMgr, IComicReposibility comicReposibility,
     IDataService dataService, IUserReposibility userReposibility, IMapper mapper, IWebHostEnvironment environment)
    {
        _mapper = mapper;
        _environment = environment;
        _userReposibility = userReposibility;
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

    public async Task<ServiceResponse<UserDTO>> UpdateInfo(UpdateUserInfo request)
    {
        var response = new ServiceResponse<UserDTO>();
        var user = await _userReposibility.GetUser(UserID);
        if (user == null)
        {
            response.Status = 404;
            response.Message = "User not found";
            response.Data = null;
            return response;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Dob = request.Dob;
        user.UpdateAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        response.Status = 200;
        response.Message = "Success";
        response.Data = _mapper.Map<UserDTO>(user);


        return response;
    }

    public async Task<ServiceResponse<string>> UpdatePassword(UpdateUserPassword request)
    {
        var response = new ServiceResponse<string>();
        var user = await _userReposibility.GetUser(UserID);
        if (user == null)
        {
            response.Status = 404;
            response.Message = "User not found";
            return response;
        }
        else if (user.HashPassword != request.OldPassword)
        {
            response.Status = 404;
            response.Message = "Password is incorrect";
            return response;
        }
        else if (request.NewPassword != request.RePassword)
        {
            response.Status = 404;
            response.Message = "Confirm password is incorrect";
            return response;
        }

        user.HashPassword = request.NewPassword;
        user.UpdateAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        response.Status = 200;
        response.Message = "Success";

        return response;
    }

    public async Task<ServiceResponse<string>> UpdateAvatar(IFormFile avatar)
    {
        try
        {
            var response = new ServiceResponse<string>();
            var user = await _userReposibility.GetUser(UserID);
            if (user == null)
            {
                response.Status = 404;
                response.Message = "User not found";
                return response;
            }
            if (avatar == null || avatar.Length == 0)
            {
                response.Status = 400;
                response.Message = "Invalid avatar file";
                return response;
            }

            // Check file type (ensure it's an image)
            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
            if (!validImageTypes.Contains(avatar.ContentType))
            {
                response.Status = 400;
                response.Message = "Invalid file type. Only JPEG, PNG, GIF, and BMP are allowed";
                return response;
            }

            // Check file size (3MB = 3 * 1024 * 1024 bytes)
            if (avatar.Length > 3 * 1024 * 1024)
            {
                response.Status = 400;
                response.Message = "File size exceeds the 3MB limit";
                return response;
            }

            // Define the folder to save the uploaded avatar

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "StaticFiles\\Avatarimg");
            // if (!Directory.Exists(uploadsFolder))
            // {
            //     Directory.CreateDirectory(uploadsFolder);
            // }

            // Define the file path for the avatar
            var fileExtension = Path.GetExtension(avatar.FileName);
            var fileName = $"{user.Email}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (user.Avatar != null)
            {
                var oldeFilePath = Path.Combine(uploadsFolder, user.Avatar);
                if (File.Exists(oldeFilePath))
                {
                    // Delete the existing file
                    File.Delete(oldeFilePath);
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                await avatar.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (var image = await Image.LoadAsync(memoryStream))
                {
                    // Crop the image to 200x200 pixels from the center

                    // Ensure the crop size is not larger than the image dimensions
                    int cropWidth = Math.Min(200, image.Width);
                    int cropHeight = Math.Min(200, image.Height);
                    int cropX = (image.Width - cropWidth) / 2;
                    int cropY = (image.Height - cropHeight) / 2;

                    var cropRectangle = new Rectangle(cropX, cropY, cropWidth, cropHeight);

                    // Crop the image
                    image.Mutate(x => x.Crop(cropRectangle));

                    // Save the cropped image
                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        switch (avatar.ContentType)
                        {
                            case "image/jpeg":
                                await image.SaveAsJpegAsync(fileStream);
                                break;
                            case "image/png":
                                await image.SaveAsPngAsync(fileStream);
                                break;
                            case "image/gif":
                                await image.SaveAsGifAsync(fileStream);
                                break;
                            case "image/bmp":
                                await image.SaveAsBmpAsync(fileStream);
                                break;
                        }
                    }
                }
            }

            // Update the user's avatar path
            user.Avatar = fileName;
            user.UpdateAt = DateTime.UtcNow;

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();

            response.Status = 200;
            response.Message = "Avatar updated successfully";
            response.Data = user.Avatar; // Return the new avatar path

            return response;
        }
        catch (Exception ex)
        {
            return new ServiceResponse<string>
            {
                Status = 500,
                Message = ex.Message
            };
        }


    }

    public async Task<ServiceResponse<UserDTO>> GetMyUserInfo()
    {
        var response = new ServiceResponse<UserDTO>();
        var user = await _userReposibility.GetUser(UserID);
        if (user == null)
        {
            response.Status = 404;
            response.Message = "User not found";
            return response;
        }

        response.Status = 200;
        response.Data = _mapper.Map<UserDTO>(user); // Return the user
        return response;
    }
}