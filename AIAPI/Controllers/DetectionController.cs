using Microsoft.AspNetCore.Mvc;
using AIAPI.Interfaces;
using AIAPI.Models;
using AIAPI.Services;

namespace AIAPI.Controllers;

public class UploadRequest
{
    public required IFormFile Image { get; set; }
    public required string CameraId { get; set; }
    public string? Location { get; set; }
    public double? LocatieX { get; set; }
    public double? LocatieY { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class DetectionController(IDetectionRepository repository, IMonitoringService monitoringService) : ControllerBase
{
    private readonly IDetectionRepository _repository = repository;
    private readonly IMonitoringService _monitoringService = monitoringService;

    [HttpGet]
    public async Task<ActionResult<List<Detection>>> GetAll()
    {
        var detections = await _repository.GetAllAsync();
        return Ok(detections);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Detection>> GetById(int id)
    {
        try
        {
            var detection = await _repository.SelectAsync(id);
            return Ok(detection);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("camera/{cameraId}")]
    public async Task<ActionResult<List<Detection>>> GetByCamera(string cameraId)
    {
        var detections = await _repository.GetByCameraAsync(cameraId);
        return Ok(detections);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Detection>> Upload([FromForm] UploadRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
            return BadRequest("No image provided.");

        var imagePath = await SaveImageAsync(request.Image);
        var aiResult = await _monitoringService.AnalyzeImageAsync(imagePath);

        var detection = new Detection
        {
            Label = aiResult.Label,
            Confidence = aiResult.Confidence,
            Timestamp = DateTime.UtcNow,
            Location = request.Location,
            LocatieX = request.LocatieX,
            LocatieY = request.LocatieY,
            CameraId = request.CameraId,
            ImagePath = imagePath
        };

        await _repository.InsertAsync(detection);
        await _monitoringService.ForwardDetectionAsync(detection);

        return CreatedAtAction(nameof(GetById), new { id = detection.Id }, detection);
    }

    [HttpPost]
    public async Task<ActionResult<Detection>> Create([FromBody] Detection detection)
    {
        detection.Timestamp = DateTime.UtcNow;
        await _repository.InsertAsync(detection);
        await _monitoringService.ForwardDetectionAsync(detection);
        return CreatedAtAction(nameof(GetById), new { id = detection.Id }, detection);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private static async Task<string> SaveImageAsync(IFormFile image)
    {
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(stream);

        return filePath;
    }
}
