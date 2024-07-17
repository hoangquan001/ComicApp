using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.Services
{
    public class UrlService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlService(IHttpContextAccessor httpContextAccessor)
        {
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
        public string GetComicCoverImagePath(string ?Image)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = $"{scheme}://{host}/static/CoverImg/{Image}";
            return url;
        }
        public string GetUserImagePath(string ?Image)
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            var host = request.Host.Value;
            var scheme = request.Scheme;
            var url = $"{scheme}://{host}/static/AvatarImg/{Image}";
            return GlobalConfig.AddTimestampToUrl(url);
        }
        
    }
}