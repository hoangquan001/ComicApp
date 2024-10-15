using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.Services
{
    public class UrlService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        private readonly string _url = "https://metruyenmoi.com";
        public UrlService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;

        }

        public string GetCurrentHost()
        {
            return _url;
        }
        public string GetComicCoverImagePath(string? Image)
        {
            return $"{_url}/CoverImg/{Image}";
        }
        public string GetUserImagePath(string? Image)
        {
            return GlobalConfig.AddTimestampToUrl($"{_url}/AvatarImg/{Image}");
        }
        public string GetPathSaveUserImage()
        {
            return Path.Combine(_environment.ContentRootPath, "wwwroot\\Avatarimg"); ;
        }
        public string GetConfirmEmailPath(int UserId, string Code)
        {

            return $"{GetCurrentHost()}/auth/confirm-email?UserId={UserId}&Code={Code}";
        }
    }
}