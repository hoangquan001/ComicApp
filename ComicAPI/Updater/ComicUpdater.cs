using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Data;
using ComicApp.Services;
using Microsoft.Extensions.Logging.Console;

namespace ComicAPI.Updater
{

    public class ComicUpdater : IHostedService, IDisposable
    {
        private Timer? _timer;
        private ulong tick = 0;
        private readonly IServiceProvider _services;
        private List<XTask> _updaters = new List<XTask>();
        public ComicUpdater(IServiceProvider services)
        {
            _services = services;
        }
        private ulong GetTick()
        {
            return tick + 1;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(Update, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)); // Update every hour
            Init();
            return Task.CompletedTask;
        }
        private void Update(object? state) // Call every 1 second
        {
            tick = GetTick();
            // Console.WriteLine(tick);
            //Implement Task Update Here
            for (int i = 0; i < _updaters.Count; i++)
            {
                _updaters[i].Update(tick);
            }
        }

        void Init()
        {

            var tasks = new XTask(second: 60);
            tasks.Register(UpdateViewChapter);
            tasks.Register(UpdateExp);

            var tasks2 = new XTask(second: 60);
            tasks2.Register(UpdateViewComic);
            AddUpdater(tasks);
            AddUpdater(tasks2);
        }

        async void UpdateViewComic()
        {

            using (var scope = _services.CreateScope())
            {
                var comicService = scope.ServiceProvider.GetRequiredService<IComicService>();
                await comicService.SyncViewComic();

            }

        }
        async void UpdateViewChapter()
        {

            using (var scope = _services.CreateScope())
            {
                var comicService = scope.ServiceProvider.GetRequiredService<IComicService>();
                await comicService.SyncViewChapter();

            }

        }
        async void UpdateExp()
        {
            using (var scope = _services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                await userService.SyncUserExp();
            }

        }
        public void AddUpdater(XTask updater)
        {
            _updaters.Add(updater);
        }

        public void RemoveUpdater(XTask updater)
        {
            _updaters.Remove(updater);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}