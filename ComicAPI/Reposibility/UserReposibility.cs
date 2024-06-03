using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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


    }
}