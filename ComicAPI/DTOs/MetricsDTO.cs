
using System.ComponentModel.DataAnnotations;
using ComicAPI.Classes;

public class MetricsDTO
{

    public double CpuUsage { get; set; }
    public long MemoryUsage { get; set; }

    public long DiskIo { get; set; }

    public int GarbageCollectionCount { get; set; }

    public int ThreadCount { get; set; }

    public double Uptime { get; set; }

    public long RequestCount { get; set; }

    public long ResponseCount { get; set; }

    public long ErrorCount { get; set; }
    public List<EndpointMetrics>? Endpoints { get; set; }



}