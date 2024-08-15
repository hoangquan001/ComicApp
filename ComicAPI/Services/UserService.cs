
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
using ComicAPI.Services;
using ComicAPI.DTOs;
using AutoMapper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ComicAPI.Enums;
using System.Collections.Concurrent;

namespace ComicApp.Services;
public class UserService : IUserService
{
    private readonly IComicReposibility _comicReposibility;
    private readonly IUserReposibility _userReposibility;
    private readonly UrlService _urlService;
    private readonly IMapper _mapper;

    private static ConcurrentDictionary<int, int> _exps = new ConcurrentDictionary<int, int>();

    private User? _CurrentUser;
    public User? CurrentUser
    {
        get
        {
            return _CurrentUser;
        }
        set
        {
            _CurrentUser = value;
        }
    }

    public bool HasLogin()
    {
        return _CurrentUser != null;
    }

    //Contructor
    public UserService( IComicReposibility comicReposibility,
      IUserReposibility userReposibility, IMapper mapper, UrlService urlService)
    {
        _urlService = urlService;
        _mapper = mapper;
        _userReposibility = userReposibility;
        _comicReposibility = comicReposibility;
    }

    public async Task<ServiceResponse<int>> FollowComic(int comicid)
    {
        if(_CurrentUser == null) return new ServiceResponse<int> { Status = 404, Message = "User not found", Data = 0 };
        int status = await _userReposibility.FollowComic(_CurrentUser.ID, comicid);
        if (status == 0) return new ServiceResponse<int> { Status = 0, Message = "Failed", Data = 0 };
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 0 };
    }

    public async Task<ServiceResponse<int>> UnFollowComic(int comicid)
    {
        if (_CurrentUser == null) return new ServiceResponse<int> { Status = 404, Message = "User not found", Data = 0 };
        int status = await _userReposibility.UnFollowComic(_CurrentUser.ID, comicid);
        if (status == 0) return new ServiceResponse<int> { Status = 0, Message = "Failed", Data = 0 };
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 0 };
    }

    public async Task<ServiceResponse<ListComicDTO>> GetFollowComics(int page = 1, int step = 40)
    {
        if (_CurrentUser == null) return new ServiceResponse<ListComicDTO> { Status = 404, Message = "User not found", Data = null };
        ListComicDTO? data = await _userReposibility.GetFollowComics(_CurrentUser.ID, page, step);
        return ServiceUtilily.GetDataRes(data);
    }

    public async Task<bool> IsFollowComic(int comicid)
    {
        if (_CurrentUser == null) return false;
        return await _userReposibility.IsFollowComic(_CurrentUser.ID, comicid);
    }

    public async Task<ServiceResponse<CommentDTO>> AddComment( string content, int chapterid, int parentcommentid = 0)
    {
        if (_CurrentUser == null) return new ServiceResponse<CommentDTO> { Status = 404, Message = "User not found", Data = null };
        var cmtData = await _userReposibility.AddComment(_CurrentUser.ID, content, chapterid, parentcommentid);
        if (cmtData == null) return new ServiceResponse<CommentDTO> { Status = 0, Message = "Failed", Data = null };
        return new ServiceResponse<CommentDTO> { Status = 1, Message = "Success", Data = cmtData };
    }

    public async Task<ServiceResponse<CommentPageDTO>> GetCommentsOfComic(int comicid, int page = 1, int step = 10)
    {
        var data = await _userReposibility.GetCommentsOfComic(comicid, page, step);
        return ServiceUtilily.GetDataRes(data);
    }

    public Task<ServiceResponse<CommentPageDTO>> GetCommentsOfChapter(int chapterid, int page = 1, int step = 10)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResponse<UserDTO>> UpdateInfo(UpdateUserInfo request)
    {
        var response = new ServiceResponse<UserDTO>();
        if (CurrentUser == null)
        {
            response.Status = 404;
            response.Message = "User not found";
            response.Data = null;
            return response;
        }
        var user = await _userReposibility.UpdateInfo(request);
        CurrentUser = user;
        response.Data = _mapper.Map<UserDTO>(user);
        return response;
    }

    public async Task<ServiceResponse<string>> UpdatePassword(UpdateUserPassword request)
    {
        var response = new ServiceResponse<string>();
        if (CurrentUser == null)
        {
            response.Status = 404;
            response.Message = "User not found";
            return response;
        }
        else if (CurrentUser.HashPassword != request.OldPassword)
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

        await _userReposibility.UpdatePassword(request);
        response.Status = 200;
        response.Message = "Success";

        return response;
    }
    public async Task<ServiceResponse<string>> UpdateMaxim(string? maxim)
    {
        if (CurrentUser == null)
        {
            return new ServiceResponse<string> { Status = 404, Message = "User not found" };
        }
        var user = await _userReposibility.UpdateMaxim(CurrentUser.ID, maxim);
        return new ServiceResponse<string> { Status = 200, Message = "Update success" };
    }
    public async Task<ServiceResponse<string>> UpdateAvatar(IFormFile avatar)
    {
        var response = new ServiceResponse<string>();

        try
        {
            if (CurrentUser == null)
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

            var uploadsFolder = _urlService.GetPathSaveUserImage();
            var fileExtension = Path.GetExtension(avatar.FileName);
            var fileName = $"{CurrentUser.Email}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (CurrentUser.Avatar != null)
            {
                var oldFilePath = Path.Combine(uploadsFolder, CurrentUser.Avatar);
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
            await _userReposibility.UpdateAvatar(CurrentUser.ID, fileName);
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

    public async Task<ServiceResponse<string>> UpdateTypelevel(int typelevel)
    {
        if (CurrentUser == null)
        {
            return new ServiceResponse<string> { Status = 404, Message = "User not found" };
        }
        await _userReposibility.UpdateTypelevel(CurrentUser.ID, typelevel);
        return new ServiceResponse<string> { Status = 200, Message = "Success" };
    }

    public async Task<ServiceResponse<UserDTO>> GetMyUserInfo()
    {
        var response = new ServiceResponse<UserDTO>();

        if (_CurrentUser == null)
        {
            response.Status = 404;
            response.Message = "User not found";
            return response;
        }
        response.Status = 200;
        _CurrentUser.Avatar = _urlService.GetUserImagePath(_CurrentUser.Avatar);
        response.Data = _mapper.Map<UserDTO>(_CurrentUser); // Return the user
        return await Task.FromResult(response);
    }

    public async Task<ServiceResponse<List<UserNotificationDTO>>> GetUserNotify()
    {
        if (_CurrentUser == null) return new ServiceResponse<List<UserNotificationDTO>> { Status = 404, Message = "User not found", Data = null };
        var response = new ServiceResponse<List<UserNotificationDTO>>();
        var notifys = await _userReposibility.GetUserNotify(_CurrentUser.ID);
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
        if (_CurrentUser == null) return new ServiceResponse<string> { Status = 404, Message = "User not found" };
        var response = new ServiceResponse<string>();
        if (await _userReposibility.UpdateUserNotify(_CurrentUser.ID, idNotify, isRead))
        {
            response.Status = 200;
            response.Message = "Success";
        }
        else
        {
            response.Status = 404;
            response.Message = "Notification not found";
        }
        return response;
    }
    public async Task<ServiceResponse<string>> DeleteUserNotify(int? idNotify)
    {
        if (_CurrentUser == null) return new ServiceResponse<string> { Status = 404, Message = "User not found" };

        var response = new ServiceResponse<string>();
        if (await _userReposibility.DeleteUserNotify(_CurrentUser.ID, idNotify))
        {
            response.Status = 200;
            response.Message = "Success";

        }
        else
        {
            response.Status = 404;
            response.Message = "Notification not found";
        }
        return response;
    }

    public async Task<ServiceResponse<int>> VoteComic( int comicid, int votePoint)
    {
        if (_CurrentUser == null) return new ServiceResponse<int> { Status = 404, Message = "User not found", Data = 0 };
        bool flag = await _userReposibility.VoteComic(_CurrentUser.ID, comicid, votePoint);
        if (flag) return new ServiceResponse<int> { Status = 0, Message = "Failed", Data = 0 };
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 1 };
    }
    public async Task<ServiceResponse<int>> UnVoteComic( int comicid)
    {
        if (_CurrentUser == null) return new ServiceResponse<int> { Status = 404, Message = "User not found", Data = 0 };
        if(await _userReposibility.UnVoteComic(_CurrentUser.ID, comicid))
        {
            return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 1 };
        }
        return new ServiceResponse<int> { Status = 0, Message = "Failed", Data = 0 };
    }
    public async Task<ServiceResponse<int>> GetUserVote(int comicid)
    {
        if (_CurrentUser == null) return new ServiceResponse<int> { Status = 404, Message = "User not found", Data = 0 };
        var user = await _userReposibility.GetUserVoteComic(_CurrentUser.ID, comicid);
        var votepoint = user?.VotePoint ?? -1;
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = votepoint };
    }

    public async Task<ServiceResponse<int>> TotalExpUser(UserExpType expt = UserExpType.Chapter)
    {
        if (_CurrentUser == null) return await Task.FromResult(ServiceUtilily.GetDataRes<int>(-1));
        _exps.AddOrUpdate(_CurrentUser.ID, 0, (key, oldValue) => oldValue + (int)expt);
        return await Task.FromResult(ServiceUtilily.GetDataRes<int>(1));
    }
    public async Task UpdateExp()
    {
        await _userReposibility.UpdateUserExp(_exps.ToDictionary());
        _exps.Clear();
    }
}