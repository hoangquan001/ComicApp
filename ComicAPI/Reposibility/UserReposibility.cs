using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ComicAPI.DTOs;
using ComicAPI.Models;
using ComicAPI.Services;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ComicAPI.Reposibility
{
    public class UserReposibility : IUserReposibility
    {
        private readonly ComicDbContext _dbContext;

        private readonly UrlService _urlService;


        public UserReposibility(ComicDbContext dbContext, UrlService urlService)
        {
            _urlService = urlService;
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
    }
}