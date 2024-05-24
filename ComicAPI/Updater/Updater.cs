using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.Updater
{
    public static class Updater
    {
        static DateTime now = DateTime.Now;
        static Updater()
        {
            
        }
        public static async Task  Update()
        {
            
            while (true)
            {
                now = DateTime.Now;
                // Console.WriteLine(now.ToString());
                Task.Delay(1000).Wait();
            }
        }
    }
}