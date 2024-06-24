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
        private static ChapterDTO ChapterSelector(Chapter x)
        {
            return new ChapterDTO
            {
                ID = x.ID,
                Title = x.Title,
                ChapterNumber = x.ChapterNumber,
                ViewCount = x.ViewCount,
                UpdateAt = x.UpdateAt,
                Slug = x.Url
            };
        }
        public async Task<List<UserNotificationDTO>?> GetUserNotify(int userid)
        {
            var notifys = await _dbContext.Notifications
            .Where(x => x.UserID == userid)
            .Select(n => new UserNotificationDTO
            {
                ID = n.ID,
                ComicID = n.ComicID,
                UserID = n.UserID,
                NotificationContent = n.NotificationContent,
                NotificationTimestamp = n.NotificationTimestamp,
                IsRead = n.IsRead,
                comic = _dbContext.Comics.Where(c => c.ID == n.ComicID).Select(x => new ComicDTO
                {
                    ID = x.ID,
                    Title = x.Title,
                    OtherName = x.OtherName,
                    Author = x.Author,
                    Url = x.Url,
                    Description = x.Description,
                    Status = x.Status,
                    Rating = x.Rating,
                    UpdateAt = x.UpdateAt,
                    CoverImage = x.CoverImage,
                    ViewCount = x.ViewCount,
                    genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
                    Chapters = _dbContext.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
                }).FirstOrDefault()
            })
            .Take(10)
            .ToListAsync();
            if (notifys == null)
            {
                return null;
            }
            return notifys;
        }


    }
}