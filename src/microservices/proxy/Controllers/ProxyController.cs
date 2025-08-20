using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Proxy.Controllers;

[ApiController]
[Route("api/")]
public class ProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ProxyController(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    [HttpGet("{*path}")]
    public async Task<IActionResult> Get(string path)
    {
        var monolithUrl = _configuration["MONOLITH_URL"];
        var moviesServiceUrl = _configuration["MOVIES_SERVICE_URL"];
        var gradualMigration = bool.Parse(_configuration["GRADUAL_MIGRATION"] ?? "false");
        var migrationPercent = int.Parse(_configuration["MOVIES_MIGRATION_PERCENT"] ?? "0");

        var targetUrl = monolithUrl;

        if (path.StartsWith("movies", StringComparison.OrdinalIgnoreCase) && gradualMigration)
        {
            var randomValue = new Random().Next(0, 100);
            targetUrl = randomValue < migrationPercent ? moviesServiceUrl : monolithUrl;
        }
        else if (path.StartsWith("events", StringComparison.OrdinalIgnoreCase))
        {
            targetUrl = _configuration["EVENTS_SERVICE_URL"];
        }

        var proxiedUrl = $"{targetUrl}/api/{path}";
        var response = await _httpClient.GetAsync(proxiedUrl);

        var content = await response.Content.ReadAsStringAsync();
        return Content(content, response.Content.Headers.ContentType?.ToString());
    }
}
