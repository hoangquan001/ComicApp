using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Data;

namespace ComicAPI.Updater
{

    public class ComicUpdater : IHostedService, IDisposable
    {
        private Timer? _timer;
        private ulong  tick = 0;
        private readonly IServiceProvider _services;
        private List<XTask> _updaters = new List<XTask>();
        public ComicUpdater(IServiceProvider services)
        {
            _services = services;
        }
        private ulong  GetTick()
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

        void Init ()
        {
            _updaters.Add(new XTask(1));
            _updaters.Add(new XTask(60));
            _updaters.Add(new XTask(3600));
            _updaters.Add(new XTask(3600*24));
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