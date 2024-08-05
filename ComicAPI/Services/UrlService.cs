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
        public UrlService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentHost()
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = $"{scheme}://{host}";

            return url;
        }
        public string GetComicCoverImagePath(string? Image)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = $"{scheme}://{host}/static/CoverImg/{Image}";
            return url;
        }
        public string GetUserImagePath(string? Image)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = $"{scheme}://{host}/static/AvatarImg/{Image}";
            return GlobalConfig.AddTimestampToUrl(url);
        }
        public string GetPathSaveUserImage()
        {
            return Path.Combine(_environment.ContentRootPath, "StaticFiles\\Avatarimg"); ;
        }
        public string GetConfirmEmailPath(int UserId, string Code)
        {

            return $"{GetCurrentHost()}/Auth/ConfirmEmail?UserId={UserId}&Code={Code}";
        }
    }
}