
using Microsoft.AspNetCore.Mvc;
using ComicAPI.Services;
[ApiController]
[Route("[controller]")]
public class MetricsController : ControllerBase
{
    private readonly MetricService _metricService;

    public MetricsController(MetricService metricService)
    {
        _metricService = metricService;
    }

    [HttpGet]
    public IActionResult GetMetrics()
    {
        var metrics = _metricService.GetMetrics();

        return Ok(metrics);
    }
}
