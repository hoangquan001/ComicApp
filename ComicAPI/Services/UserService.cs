
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

using ComicAPI.DTOs;

using AutoMapper;
// using static System.Net.Mime.MediaTypeNames;


using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using ComicAPI.Models;
using System.Formats.Tar;

namespace ComicApp.Services;
public class UserService : IUserService
{
    private readonly IComicReposibility _comicReposibility;
    private readonly IUserReposibility _userReposibility;
    private readonly IWebHostEnvironment _environment;
    private readonly UrlService _urlService;
    private readonly ComicDbContext _dbContext;
    private readonly IMapper _mapper;
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
    public UserService(ComicDbContext db, IComicReposibility comicReposibility,
      IUserReposibility userReposibility, IMapper mapper, IWebHostEnvironment environment, UrlService urlService)
    {
        _urlService = urlService;
        _mapper = mapper;
        _environment = environment;
        _userReposibility = userReposibility;
        _comicReposibility = comicReposibility;
        _dbContext = db;
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
            User = new UserDTO
            {
                ID = x.UserID,
                Username = x.User!.FirstName + " " + x.User.LastName,
                Email = x.User.Email,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Avatar = _urlService.GetUserImagePath(x.User.Avatar),
                Dob = x.User.Dob,
                Gender = x.User.Gender,
                CreateAt = x.User.CreateAt,
                TypeLevel = x.User.TypeLevel,
                Experience = x.User.Experience,
                Maxim = x.User.Maxim
            },
        })
        .FirstOrDefaultAsync();



        return new ServiceResponse<CommentDTO> { Status = 1, Message = "Success", Data = cmtData };

    }

    public async Task<ServiceResponse<CommentDTO>> AddComment(string content, int chapterid, int parentcommentid = 0)
    {
        return await AddComment(UserID, content, chapterid, parentcommentid);
    }

    public async Task<ServiceResponse<CommentPageDTO>> GetCommentsOfComic(int comicid, int page = 1, int step = 10)
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
                User = new UserDTO
                {
                    ID = x.UserID,
                    Username = x.User!.FirstName + " " + x.User.LastName,
                    Email = x.User.Email,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Avatar = _urlService.GetUserImagePath(x.User.Avatar),
                    Dob = x.User.Dob,
                    Gender = x.User.Gender,
                    CreateAt = x.User.CreateAt,
                    TypeLevel = x.User.TypeLevel,
                    Experience = x.User.Experience,
                    Maxim = x.User.Maxim

                },
                Replies = x.Replies!.Select(y => new CommentDTO
                {
                    ID = y.ID,
                    Content = y.Content,
                    UserID = y.UserID,
                    ChapterID = y.ChapterID,
                    ComicID = y.ComicID,
                    ParentCommentID = y.ParentCommentID,
                    CommentedAt = y.CommentedAt,
                    UserName = y.User!.FirstName + " " + y.User.LastName,
                    User = new UserDTO
                    {
                        ID = y.UserID,
                        Username = y.User!.FirstName + " " + y.User.LastName,
                        Email = y.User.Email,
                        FirstName = y.User.FirstName,
                        LastName = y.User.LastName,
                        Avatar = _urlService.GetUserImagePath(x.User.Avatar),
                        Dob = y.User.Dob,
                        Gender = y.User.Gender,
                        CreateAt = y.User.CreateAt,
                        TypeLevel = y.User.TypeLevel,
                        Experience = y.User.Experience,
                        Maxim = y.User.Maxim
                    },
                }).ToList()
            })
            .Skip((page - 1) * step)
            .Take(step)
            .ToListAsync();
        if (data != null)
        {
            int totalcomment = _dbContext.Comments.Where(x => x.ComicID == comicid && x.ParentCommentID == null).Count();
            CommentPageDTO list = new CommentPageDTO
            {
                Totalpage = (int)MathF.Ceiling((float)totalcomment / step),
                cerrentpage = page,
                Comments = data

            };
            return ServiceUtilily.GetDataRes<CommentPageDTO>(list);
        }
        return ServiceUtilily.GetDataRes<CommentPageDTO>(null);
    }

    public Task<ServiceResponse<CommentPageDTO>> GetCommentsOfChapter(int chapterid, int page = 1, int step = 10)
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
        user.Avatar = _urlService.GetUserImagePath(user.Avatar);
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
    public async Task<ServiceResponse<string>> UpdateMaxim(string? maxim)
    {

        var user = await _userReposibility.GetUser(UserID);
        if (user == null)
        {
            return new ServiceResponse<string> { Status = 404, Message = "User not found" };
        }

        user.Maxim = maxim;
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<string> { Status = 200, Message = "Update success" };
    }
    public async Task<ServiceResponse<string>> UpdateAvatar(IFormFile avatar)
    {
        var response = new ServiceResponse<string>();

        try
        {
            var user = await _userReposibility.GetUser(UserID);
            if (user == null)
            {
                response.Status = 404;
                response.Message = "User not found";
                return response;
            }

            if (avatar == null || avatar.Length == 0)
            {
                return new ServiceResponse<string>
                {
                    Status = 400,
                    Message = "Invalid avatar file"
                };
            }

            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
            if (!validImageTypes.Contains(avatar.ContentType))
            {
                response.Status = 400;
                response.Message = "Invalid file type. Only JPEG, PNG, GIF, and BMP are allowed";
                return response;
            }

            if (avatar.Length > 3 * 1024 * 1024)
            {
                response.Status = 400;
                response.Message = "File size exceeds the 3MB limit";
                return response;
            }

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "StaticFiles\\Avatarimg");
            var fileExtension = Path.GetExtension(avatar.FileName);
            var fileName = $"{user.Email}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (user.Avatar != null)
            {
                var oldFilePath = Path.Combine(uploadsFolder, user.Avatar);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                await avatar.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var image = await Image.LoadAsync(memoryStream))
                {
                    int cropbox = Math.Min(image.Height, image.Width);
                    int cropWidth = Math.Min(cropbox, image.Width);
                    int cropHeight = Math.Min(cropbox, image.Height);
                    int cropX = (image.Width - cropWidth) / 2;
                    int cropY = (image.Height - cropHeight) / 2;

                    var cropRectangle = new Rectangle(cropX, cropY, cropWidth, cropHeight);

                    image.Mutate(x => x.Crop(cropRectangle));
                    image.Mutate(x => x.Resize(200, 200));

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

            user.Avatar = fileName;
            user.UpdateAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            response.Data = _urlService.GetUserImagePath(fileName);
            response.Status = 200;
            response.Message = "Avatar updated successfully";
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
        }

        return response;
    }
    public async Task<ServiceResponse<string>> UpdateExp(int exp)
    {
        var user = await _userReposibility.GetUser(UserID);
        if (user == null)
        {
            return new ServiceResponse<string> { Status = 404, Message = "User not found" };
        }
        user.Experience = exp;
        user.UpdateAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<string> { Status = 200, Message = "Success" };
    }
    public async Task<ServiceResponse<string>> UpdateTypelevel(int typelevel)
    {
        var user = await _userReposibility.GetUser(UserID);
        if (user == null)
        {
            return new ServiceResponse<string> { Status = 404, Message = "User not found" };
        }
        user.TypeLevel = typelevel;
        user.UpdateAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<string> { Status = 200, Message = "Success" };
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
        user.Avatar = _urlService.GetUserImagePath(user.Avatar);
        response.Data = _mapper.Map<UserDTO>(user); // Return the user
        return response;
    }

    public async Task<ServiceResponse<List<UserNotificationDTO>>> GetUserNotify()
    {

        var response = new ServiceResponse<List<UserNotificationDTO>>();
        var notifys = await _userReposibility.GetUserNotify(UserID);
        if (notifys == null)
        {
            response.Status = 404;
            response.Message = "User not found";
            return response;
        }

        response.Status = 200;
        response.Data = notifys;
        return response;
    }
    public async Task<ServiceResponse<string>> UpdateUserNotify(int? idNotify, bool? isRead = null)
    {

        var response = new ServiceResponse<string>();

        if (idNotify == null)
        {
            // Đánh dấu tất cả thông báo là đã đọc
            var notifys = await _dbContext.Notifications.ToListAsync();
            if (notifys == null)
            {
                response.Status = 404;
                response.Message = "Notifications not found";
                return response;
            }
            notifys.ForEach(n => n.IsRead = true);
            await _dbContext.SaveChangesAsync();

            response.Status = 200;
            response.Message = "All notifications have been marked as read";
        }
        else
        {
            var notify = await _dbContext.Notifications
                .Where(n => n.ID == idNotify && n.UserID == UserID)
                .FirstOrDefaultAsync();

            if (notify == null)
            {
                response.Status = 404;
                response.Message = "Notification not found";
            }
            else
            {
                notify.IsRead = isRead ?? true;
                await _dbContext.SaveChangesAsync();
                response.Status = 200;
                response.Message = "Notification has been marked as read";
            }
        }

        return response;
    }
    public async Task<ServiceResponse<string>> DeleteUserNotify(int? idNotify)
    {

        var response = new ServiceResponse<string>();



        if (idNotify == -1)
        {

            var notify = await _dbContext.Notifications
               .Where(n => n.UserID == UserID)
               .ExecuteDeleteAsync(); // Kiểm tra xem notification có tồn tại hay không    
            await _dbContext.SaveChangesAsync();
            response.Status = 200;
            response.Message = "Notification has been deleted";
        }
        else
        {
            var notify = await _dbContext.Notifications
                .Where(n => n.ID == idNotify && n.UserID == UserID)
                .ExecuteDeleteAsync(); // Kiểm tra xem notification có tồn tại hay không    
            await _dbContext.SaveChangesAsync();
            response.Status = 200;
            response.Message = "Notification has been deleted";
        }


        return response;
    }

    public async Task<ServiceResponse<int>> VoteComic(int userid, int comicid, int votePoint)
    {
        if (!_dbContext.Comics.Any(x => x.ID == comicid)) return new ServiceResponse<int> { Status = 0, Message = "Comic not found", Data = 0 };
        //check comicid exist
        if (votePoint < 0 || votePoint > 10) return new ServiceResponse<int> { Status = 0, Message = "Invalid vote point", Data = 0 };
        var user = await _userReposibility.GetUserVoteComic(userid, comicid);
        if (user != null) user.VotePoint = votePoint;
        else _dbContext.UserVoteComics.Add(new UserVoteComic { UserID = userid, ComicID = comicid, VotePoint = votePoint });
        await _dbContext.SaveChangesAsync();

        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 1 };
    }
    public async Task<ServiceResponse<int>> VoteComic(int comicid, int votePoint)
    {
        return await VoteComic(UserID, comicid, votePoint);
    }
    public async Task<ServiceResponse<int>> UnVoteComic(int userid, int comicid)
    {
        if (!_dbContext.Comics.Any(x => x.ID == comicid)) return new ServiceResponse<int> { Status = 0, Message = "Comic not found", Data = 0 };

        var user = await _userReposibility.GetUserVoteComic(userid, comicid);
        if (user == null) return new ServiceResponse<int> { Status = 0, Message = "Not Vote", Data = 0 };
        _dbContext.UserVoteComics.Remove(user);

        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 0 };
    }
    public async Task<ServiceResponse<int>> UnVoteComic(int comicid)
    {
        return await UnVoteComic(UserID, comicid);
    }
    public async Task<ServiceResponse<int>> GetUserVote(int comicid)
    {

        var user = await _userReposibility.GetUserVoteComic(UserID, comicid);
        var votepoint = user?.VotePoint ?? -1;
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = votepoint };
    }
}