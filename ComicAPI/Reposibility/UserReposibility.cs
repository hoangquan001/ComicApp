
using ComicAPI.DTOs;
using ComicAPI.Models;
using ComicAPI.Services;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ComicAPI.Reposibility
{
    public class UserReposibility : IUserReposibility
    {
        private readonly ComicDbContext _dbContext;

        private readonly IMemoryCache _memoryCache;
        private readonly UrlService _urlService;


        public UserReposibility(ComicDbContext dbContext, UrlService urlService, IMemoryCache memoryCache)
        {
            _urlService = urlService;
            _dbContext = dbContext;
            _memoryCache = memoryCache;

        }
        public async Task<bool> UnVoteComic(int userid, int comicid)
        {
            if (!_dbContext.Comics.Any(x => x.ID == comicid)) return false;
            var user = await GetUserVoteComic(userid, comicid);
            if (user == null) return false;
            _dbContext.UserVoteComics.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteUserNotify(int userId, int? idNotify)
        {
            if (idNotify == -1)
            {
                var notify = await _dbContext.Notifications
                .Where(n => n.UserID == userId)
                .ExecuteDeleteAsync(); // Kiểm tra xem notification có tồn tại hay không    
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                var notify = await _dbContext.Notifications
                .Where(n => n.ID == idNotify && n.UserID == userId)
                .ExecuteDeleteAsync(); // Kiểm tra xem notification có tồn tại hay không    
                await _dbContext.SaveChangesAsync();
                return true;
            }

        }
        public async Task<bool> UpdateUserNotify(int userId, int? idNotify, bool? isRead = null)
        {
            if (idNotify == null)
            {
                // Đánh dấu tất cả thông báo là đã đọc
                var notifys = await _dbContext.Notifications.Where(n => n.UserID == userId).ToListAsync();
                if (notifys == null)
                {
                    return false;
                }
                notifys.ForEach(n => n.IsRead = true);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            else
            {
                var notify = await _dbContext.Notifications
                    .Where(n => n.ID == idNotify && n.UserID == userId)
                    .FirstOrDefaultAsync();

                if (notify == null)
                {
                    return false;
                }

                notify.IsRead = isRead ?? true;
                await _dbContext.SaveChangesAsync();
                return true;

            }
        }
        private void ClearUserCache(int userid)
        {
            _memoryCache.Remove(string.Format("user-{0}", userid));
        }

        public async Task<User?> GetUser(int userid)
        {
            string key = string.Format("user-{0}", userid);
            if (_memoryCache.TryGetValue(key, out User? cachedData))
            {
                return cachedData;
            }
            var user = await _dbContext.Users.Where(x => x.ID == userid).AsNoTracking().FirstOrDefaultAsync();
            if (user != null)
            {
                user.Avatar = _urlService.GetUserImagePath(user.Avatar);
                _memoryCache.Set(key, user, TimeSpan.FromMinutes(5));
            }
            return user;
        }


        public async Task<List<UserNotificationDTO>?> GetUserNotify(int userid)
        {
            var notifys = await _dbContext.Notifications
            .Where(x => x.UserID == userid)
            .OrderByDescending(x => x.Timestamp)
            .Select(n => new UserNotificationDTO
            {
                ID = n.ID,
                ComicID = n.ComicID,
                UserID = n.UserID,
                Content = n.Content,
                Timestamp = n.Timestamp,
                IsRead = n.IsRead,
                CoverImage = _urlService.GetComicCoverImagePath(n.CoverImage),
                URLComic = n.URLComic,
                lastchapter = n.lastchapter,
                URLChapter = n.URLChapter

            })
            .Take(10)
            .ToListAsync();
            if (notifys == null)
            {
                return null;
            }
            return notifys;
        }
        public async Task<bool> VoteComic(int userid, int comicid, int votePoint)
        {
            if (!_dbContext.Comics.Any(x => x.ID == comicid)) return false;
            if (votePoint < 0 || votePoint > 10) return false;
            UserVoteComic? user = await GetUserVoteComic(userid, comicid);
            if (user != null)
                user.VotePoint = votePoint;
            else
                await _dbContext.UserVoteComics.AddAsync(new UserVoteComic { UserID = userid, ComicID = comicid, VotePoint = votePoint });
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<UserVoteComic?> GetUserVoteComic(int userid, int comicid)
        {
            return await _dbContext.UserVoteComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
        }

        public async Task UpdateUserExp(Dictionary<int, int> exps)
        {
            var userIds = exps.Keys.ToList();
            var usersToUpdate = await _dbContext.Users
            .Where(u => userIds.Contains(u.ID))
            .ToListAsync();

            if (usersToUpdate == null) return;
            foreach (var user in usersToUpdate)
            {
                user.Experience += exps[user.ID];
            }


            await _dbContext.SaveChangesAsync();

        }
        public async Task<int> UnFollowComic(int userid, int comicid)
        {
            if (!_dbContext.Comics.Any(x => x.ID == comicid)) return 0;
            var user = await _dbContext.UserFollowComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
            if (user == null) return 0;
            _dbContext.UserFollowComics.Remove(user);
            await _dbContext.SaveChangesAsync();
            return 1;
        }
        public async Task<int> FollowComic(int userid, int comicid)
        {
            if (!_dbContext.Comics.Any(x => x.ID == comicid)) return 0;
            var user = await _dbContext.UserFollowComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
            if (user != null) return 0;
            _dbContext.UserFollowComics.Add(new UserFollowComic { UserID = userid, ComicID = comicid });
            await _dbContext.SaveChangesAsync();
            return 1;
        }

        public async Task<User?> UpdateInfo(UpdateUserInfo request)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.ID == request.UserId);
            if (user == null) return null;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Dob = request.Dob;
            user.UpdateAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            user.Avatar = _urlService.GetUserImagePath(user.Avatar);
            ClearUserCache(user.ID);
            return user;
        }
        public async Task<User?> UpdatePassword(UpdateUserPassword request)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.ID == request.UserId);
            if (user == null) return null;
            user.HashPassword = request.NewPassword;
            user.UpdateAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            ClearUserCache(user.ID);
            return user;
        }

        public async Task<User?> UpdateAvatar(int userId, string fileName)
        {
            User? user = await _dbContext.Users.SingleOrDefaultAsync(x => x.ID == userId);
            if (user == null) return null;
            user.Avatar = fileName;
            user.UpdateAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            ClearUserCache(user.ID);
            return user;
        }

        public async Task<User?> UpdateMaxim(int UserID, string? maxim)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.ID == UserID);
            if (user == null) return null;

            user.Maxim = maxim;
            await _dbContext.SaveChangesAsync();
            ClearUserCache(user.ID);
            return user;
        }

        public async Task<User?> UpdateTypelevel(int userId, int typelevel)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.ID == userId);
            if (user == null) return null;
            user.TypeLevel = typelevel;
            user.UpdateAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            ClearUserCache(user.ID);
            return user;
        }
        public async Task<bool> IsFollowComic(int userid, int comicid)
        {
            //check comicid exist
            var user = await _dbContext.UserFollowComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
            return user != null;

        }

        public async Task<CommentDTO?> AddComment(int userid, string content, int chapterid, int parentcommentid = 0)
        {
            var chapter = _dbContext.Chapters.FirstOrDefault(x => x.ID == chapterid);
            if (chapter == null) return null;
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
            return cmtData;
        }
        public async Task<ListComicDTO?> GetFollowComics(int userid, int page, int size)
        {
            var data = await _dbContext.UserFollowComics
            .Where(x => x.UserID == userid)
            .Include(x => x.comic)
            .OrderByDescending(x => x.comic!.UpdateAt)
            .Select(x => new ComicDTO
            {
                ID = x.comic!.ID,
                Title = x.comic.Title,
                OtherName = x.comic.OtherName,
                Author = x.comic.Author,
                Url = x.comic.Url,
                CoverImage = _urlService.GetComicCoverImagePath(x.comic.CoverImage),
                Description = x.comic.Description,
                Status = x.comic.Status,
                Rating = x.comic.Rating,
                ViewCount = x.comic.ViewCount,
                UpdateAt = x.comic.UpdateAt,
                genres = x.comic.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
                Chapters = x.comic.Chapters.Where(c => c.ID == x.comic.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
            })
            .Skip((page - 1) * size)
            .Take(size).ToListAsync();
            if (data != null)
            {
                int totalcomic = _dbContext.UserFollowComics.Where(x => x.UserID == userid).Count();
                ListComicDTO list = new ListComicDTO
                {
                    totalpage = (int)MathF.Ceiling((float)totalcomic / size),
                    Page = page,
                    Step = size,
                    comics = data
                };
                return list;
            }
            return null;
        }

        public async Task<CommentPageDTO?> GetCommentsOfComic(int comicid, int page = 1, int step = 10)
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
                        UserName = y.User!.FirstName + " " + y.User.LastName,
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
                return list;
            }
            return null;
        }
        private static ChapterDTO ChapterSelector(Chapter x)
        {
            return new ChapterDTO
            {
                ID = x.ID,
                Title = x.Title,
                ViewCount = x.ViewCount,
                UpdateAt = x.UpdateAt,
                Slug = x.Url
            };
        }
    }
}