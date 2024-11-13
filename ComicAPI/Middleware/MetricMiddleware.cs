
using System.Diagnostics;
using System.Security.Claims;
using ComicAPI.Enums;
using ComicAPI.Reposibility;
using ComicAPI.Services;
using ComicApp.Models;
using ComicApp.Services;
public class MetricMiddleware
{
    private readonly RequestDelegate _next;
    private readonly MetricService _metricService;
    private readonly EndpointDataSource _endpointDataSource;

    private readonly ILogger _logger;



    public MetricMiddleware(RequestDelegate next, MetricService metricService, EndpointDataSource endpointDataSource,  ILogger<MetricMiddleware> logger)
    {
        _next = next;
        _metricService = metricService;
        _endpointDataSource = endpointDataSource;
        _logger = logger;
    }

    public IEnumerable<string> GetEndpointPatterns()
    {
        var endpoints = _endpointDataSource.Endpoints.OfType<RouteEndpoint>();

        var patterns = endpoints.Select(pattern => pattern.RoutePattern.RawText);

        return patterns!;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Value!.ToLower() == "/metrics")
        {
            await _next(context);
            return;
        }
        // var data = GetEndpointPatterns();

        var stopwatch = Stopwatch.StartNew();
        _metricService.IpCount(context.Connection.RemoteIpAddress!.MapToIPv4().ToString());
        _metricService.IncrementInt(MetricType.RequestCount);

        try
        {
            await _next(context);
            _metricService.IncrementInt(context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 ? MetricType.TotalSuccesses : MetricType.ErrorCount);

        }
        catch
        {
            _metricService.IncrementInt(MetricType.ErrorCount);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var  lowerPath = context.Request.Path.ToString().ToLower();

         

            if (lowerPath.Contains("/static/"))
            {
                _metricService.TrackRequestDuration("/static", stopwatch.ElapsedMilliseconds);

            }
            else if(lowerPath.Contains("/api/image/"))
            {
                _metricService.TrackRequestDuration("/api/image", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                string ipAddress = GetIpAddress(context);
                _metricService.TrackRequestDuration(context.Request.Path, stopwatch.ElapsedMilliseconds);
                _logger.LogInformation($"[{DateTime.Now.ToLocalTime()}] {context.Request.Method} {context.Request.Path}{context.Request.QueryString} {context.Response.StatusCode} {stopwatch.ElapsedMilliseconds}ms {ipAddress}");
            }

        }
    }

    private string GetIpAddress(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("CF-Connecting-IP"))
        {
            return context.Request.Headers["CF-Connecting-IP"].ToString();
        }
        else if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return context.Request.Headers["X-Forwarded-For"].ToString().Split(',').First().Trim();
        }
        return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }
}
