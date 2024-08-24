
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


    public MetricMiddleware(RequestDelegate next, MetricService metricService, EndpointDataSource endpointDataSource)
    {
        _next = next;
        _metricService = metricService;
        _endpointDataSource = endpointDataSource;
    }

    public IEnumerable<string> GetEndpointPatterns()
    {
        var endpoints = _endpointDataSource.Endpoints.OfType<RouteEndpoint>();

        var patterns = endpoints.Select(pattern => pattern.RoutePattern.RawText);

        return patterns!;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.ToString().ToLower() == "/metrics")
        {
            await _next(context);
            return;
        }
        var data = GetEndpointPatterns();

        var stopwatch = Stopwatch.StartNew();

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
            if (context.Request.Path.ToString().ToLower().Contains("/static/"))
            {
                _metricService.TrackRequestDuration("/static", stopwatch.ElapsedMilliseconds);

            }
            else
            {
                _metricService.TrackRequestDuration(context.Request.Path, stopwatch.ElapsedMilliseconds);

                Console.WriteLine($"Response: {context.Request.Method} {context.Request.Path} {context.Response.StatusCode} {stopwatch.ElapsedMilliseconds}ms");
            }

        }
    }
}
