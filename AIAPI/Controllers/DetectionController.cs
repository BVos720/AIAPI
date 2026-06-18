using Microsoft.AspNetCore.Mvc;
using AIAPI.Filters;
using AIAPI.Interfaces;
using AIAPI.Models;
using AIAPI.Services;

namespace AIAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetectionController(IDetectionRepository repository, IGeocodingService geocoding) : ControllerBase
{
    private readonly IDetectionRepository _repository = repository;
    private readonly IGeocodingService _geocoding = geocoding;

    // ─── GET: Alle detecties ophalen (monitoring key) ─────────────────
    [HttpGet]
    [ApiKey("ApiKeys:Monitoring")]
    public async Task<ActionResult<List<Detection>>> GetAll()
    {
        var detections = await _repository.GetAllAsync();
        return Ok(detections);
    }

    // ─── POST: AI model post zijn detecties (camera/ESP32) ─────────────
    [HttpPost("ai")]
    [ApiKey("ApiKeys:Training")]
    public async Task<ActionResult<Detection>> PostFromAI([FromBody] Detection detection)
    {
        if (detection == null)
            return BadRequest("Geen data ontvangen.");

        detection.Timestamp = DateTime.UtcNow;

        if (detection.LocatieX.HasValue && detection.LocatieY.HasValue)
            detection.Location = await _geocoding.GetAddressAsync(detection.LocatieX.Value, detection.LocatieY.Value);

        await _repository.InsertAsync(detection);

        return Ok(detection);
    }

    // ─── POST: Frontend upload een afbeelding ──────────────────────────
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Upload([FromForm] IFormFile image, [FromForm] string cameraId, [FromForm] string? location)
    {
        if (image == null || image.Length == 0)
            return BadRequest("Geen afbeelding ontvangen.");

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        // TODO: AI verwerking toevoegen als het model beschikbaar is
        var detection = new Detection
        {
            Label = "pending",
            Confidence = 0,
            Timestamp = DateTime.UtcNow,
            Location = location,
            CameraId = cameraId,
            ImageId = filePath
        };

        await _repository.InsertAsync(detection);

        return Ok(detection);
    }
}
