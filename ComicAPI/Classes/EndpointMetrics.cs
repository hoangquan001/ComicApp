using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicAPI.Enums;

namespace ComicAPI.Classes
{
    public class EndpointMetrics
    {
        public string Endpoint { get; set; } = string.Empty;
        public float Duration { get; set; }
        public int Count { get; set; }
        public int AvgDuration { get; set; }
    }
}