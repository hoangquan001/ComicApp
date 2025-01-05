using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicAPI.Enums;

namespace ComicAPI.Classes
{
    public class AppSetting
    {
        public string ReportApiUrl { get; set; } = string.Empty;

        public string ReportApiToken { get; set; } = string.Empty;

        public string? Host { get; set; } = null;
        public string? ImgHost { get; set; } = null;
    }
}