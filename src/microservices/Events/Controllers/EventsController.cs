using Events.Services;
using Microsoft.AspNetCore.Mvc;

namespace Events.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IKafkaProducer _kafkaProducer;

    public EventsController(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }
    
    [HttpPost("{eventType}")]
    public async Task<IActionResult> CreateEvent(string eventType, [FromBody] object eventData)
    {
        if (eventData.ToString() == null)
        {
            return BadRequest(new { Message = "Event data cannot be null." });
        }
        
        var isAllowedEventType = eventType.ToLower() switch
        {
            "user" => true,
            "payment" => true,
            "movie" => true,
            _ => false
        };
        
        if(!isAllowedEventType)
        {
            return BadRequest(new { Message = "Invalid event type. Allowed types are: User, Payment, Movie." });
        }
        
        await _kafkaProducer.ProduceAsync($"{eventType}-events", eventData.ToString() ?? "Error: Event data is null");
        return Created($"/api/events/{eventType}", new { Status = "success", message = "Event created successfully" });
    }
}