using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicAPI.Classes;
using Microsoft.Extensions.Options;

namespace ComicAPI.Services
{
    public class UrlService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppSetting _config;

        public string Host => _host;
        public string ImgHost => _imgHost;
        private readonly string _imgHost = "https://img.metruyenmoi.com";
        private readonly string _host = "https://metruyenmoi.com";
        public UrlService(IWebHostEnvironment environment, IOptions<AppSetting> options)
        {
            _environment = environment;
            _config = options.Value;
            _host = _config.Host ?? _host;
            _imgHost = _config.ImgHost ?? _imgHost;
        }

        public string GetComicCoverImagePath(string? Image)
        {
            return $"{Host}/CoverImg/{Image}";
        }
        public string GetUserImagePath(string? Image)
        {
            return ServiceUtilily.AddTimestampToUrl($"{Host}/AvatarImg/{Image}");
        }
        public string GetPathSaveUserImage()
        {
            return Path.Combine(_environment.ContentRootPath, "wwwroot/AvatarImg"); ;
        }
        public string GetConfirmEmailPath(int UserId, string Code)
        {

            return $"{Host}/auth/confirm-email?UserId={UserId}&Code={Code}";
        }
    }
}