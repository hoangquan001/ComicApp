

using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using ComicAPI.Enums;
using System.Collections.Concurrent;
using System.Text.Json;
using ComicAPI.Classes;
using Newtonsoft.Json;

namespace ComicAPI.Services
{

    public class MetricService
    {
        private readonly ConcurrentDictionary<MetricType, long> _intMetrics = new ConcurrentDictionary<MetricType, long>();
        private readonly ConcurrentDictionary<string, (float duration, int count)> _endpointDurations = new ConcurrentDictionary<string, (float duration, int count)>();

        private readonly ConcurrentDictionary<string, int> _ips = new ConcurrentDictionary<string, int>();
        private readonly Process _process;

        public MetricService()
        {
            _process = Process.GetCurrentProcess();
        }

        public void IpCount(string ip)
        {
            _ips.AddOrUpdate(ip, 1, (key, value) => value + 1);
        }

        public double GetCpuUsage()
        {

            return _process.TotalProcessorTime.TotalMilliseconds / (Environment.ProcessorCount * 1000.0);
        }

        public long GetMemoryUsage()
        {
            return _process.WorkingSet64;
        }

        // public float GetAvailableMemory()
        // {
        //     return _process.PrivateMemorySize64 / (1024.0f * 1024.0f);
        // }

        public double GetUptime()
        {
            return (DateTime.Now - _process.StartTime).TotalSeconds;
        }

        public int GetThreadCount()
        {
            return _process.Threads.Count;
        }

        public long GetDiskIo()
        {
            return 0;
        }

        public int GetGarbageCollectionCount()
        {
            return GC.CollectionCount(0);
        }

        public void IncrementInt(MetricType metricType)
        {
            _intMetrics.AddOrUpdate(metricType, 1, (key, value) => value + 1);
        }



        public void TrackRequestDuration(string endpoint, float duration)
        {
            _endpointDurations.AddOrUpdate(endpoint, (duration, 1), (key, value) => (value.duration + duration, value.count + 1));
        }

        public MetricsDTO GetMetrics()
        {

            var endpointMetrics = _endpointDurations.Select(kvp => new EndpointMetrics
            {
                Endpoint = kvp.Key,
                Duration = kvp.Value.duration,
                Count = kvp.Value.count,
                AvgDuration = (int)(kvp.Value.duration / kvp.Value.count)
            }).ToList();

            return new MetricsDTO
            {
                CpuUsage = GetCpuUsage(),
                MemoryUsage = GetMemoryUsage(),
                DiskIo = GetDiskIo(),
                GarbageCollectionCount = GetGarbageCollectionCount(),
                ThreadCount = GetThreadCount(),
                Uptime = GetUptime(),
                RequestCount = _intMetrics.GetValueOrDefault(MetricType.RequestCount, 0),
                ErrorCount = _intMetrics.GetValueOrDefault(MetricType.ErrorCount, 0),
                Endpoints = endpointMetrics
            };
            // return result;
        }
    }

}
