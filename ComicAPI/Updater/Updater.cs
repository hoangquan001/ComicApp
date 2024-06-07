using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.Updater
{
    public static class Updater
    {
        static DateTime now = DateTime.Now;
        // static int FPS = 60;
        static Updater()
        {

        }
        public static async Task Update()
        {

            while (true)
            {
                DateTime _now = DateTime.Now;

                // int value = _now.Millisecond - now.Millisecond;
                // Console.WriteLine(value);
                // now = _now;
                await Task.Delay(1000);
            }
        }
    }
}