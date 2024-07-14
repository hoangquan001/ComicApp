using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComicAPI.DTOs;
using ComicAPI.Models;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ComicAPI.Reposibility
{
    public class UserReposibility : IUserReposibility
    {
        private readonly ComicDbContext _dbContext;


        public UserReposibility(ComicDbContext dbContext)
        {

            _dbContext = dbContext;

        }


        public async Task<User?> GetUser(int userid)
        {

            var user = await _dbContext.Users
            .Where(x => x.ID == userid)
            .AsSplitQuery()
            .FirstOrDefaultAsync();
            if (user == null)
            {
                return null;

            }
            return user;
        }

        public async Task<List<UserNotificationDTO>?> GetUserNotify(int userid)
        {
            var notifys = await _dbContext.Notifications
            .Where(x => x.UserID == userid)
            .OrderByDescending(x => x.NotificationTimestamp)
            .Select(n => new UserNotificationDTO
            {
                ID = n.ID,
                ComicID = n.ComicID,
                UserID = n.UserID,
                NotificationContent = n.NotificationContent,
                NotificationTimestamp = n.NotificationTimestamp,
                IsRead = n.IsRead,
                CoverImage = n.CoverImage,
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

        public async Task<UserVoteComic?> GetUserVoteComic(int userid, int comicid)
        {
            return await _dbContext.UserVoteComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
        }
    }
}