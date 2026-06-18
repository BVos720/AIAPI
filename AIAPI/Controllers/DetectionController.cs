using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIAPI.Data;
using AIAPI.Filters;
using AIAPI.Interfaces;
using AIAPI.Models;
using AIAPI.Services;

namespace AIAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetectionController(IDetectionRepository repository, IGeocodingService geocoding, SensoringDbContext db) : ControllerBase
{
    private readonly IDetectionRepository _repository = repository;
    private readonly IGeocodingService _geocoding = geocoding;
    private readonly SensoringDbContext _db = db;

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

        // Optioneel: afbeelding meegestuurd? Sla die op in de aparte DetectionImages-tabel.
        if (!string.IsNullOrWhiteSpace(detection.ImageBase64))
        {
            try
            {
                var data = Convert.FromBase64String(detection.ImageBase64);
                _db.DetectionImages.Add(new DetectionImage
                {
                    DetectionId = detection.Id,
                    ContentType = string.IsNullOrWhiteSpace(detection.ImageContentType) ? "image/jpeg" : detection.ImageContentType,
                    Data = data,
                    CreatedAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
            }
            catch (FormatException)
            {
                return BadRequest("ImageBase64 is geen geldige base64-string.");
            }
        }

        return Ok(detection);
    }

    // ─── GET: Afbeelding van een detectie ophalen ──────────────────────
    [HttpGet("{id}/image")]
    [ApiKey("ApiKeys:Monitoring")]
    public async Task<ActionResult> GetImage(int id)
    {
        var image = await _db.DetectionImages.FirstOrDefaultAsync(i => i.DetectionId == id);
        if (image == null)
            return NotFound();

        return File(image.Data, image.ContentType);
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
