using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using AIAPI.Data;
using AIAPI.Filters;
using AIAPI.Models;

namespace AIAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiKey("ApiKeys:Training")]
public class TrainingImageController(SensoringDbContext db) : ControllerBase
{
    private readonly SensoringDbContext _db = db;

    // ─── GET: Lijst van alle training afbeeldingen (zonder bytes) ──────
    [HttpGet]
    public async Task<ActionResult<List<object>>> GetAll()
    {
        var images = await _db.TrainingImages
            .Include(i => i.BoundingBoxes)
            .Select(i => new
            {
                i.Id,
                i.FileName,
                i.ContentType,
                i.UploadedAt,
                BoundingBoxes = i.BoundingBoxes.Select(b => new
                {
                    b.Id,
                    b.Label,
                    b.CenterX,
                    b.CenterY,
                    b.Width,
                    b.Height
                })
            })
            .ToListAsync();

        return Ok(images);
    }

    // ─── GET: Haal een afbeelding op als bestand ───────────────────────
    [HttpGet("{id}/image")]
    public async Task<ActionResult> GetImage(int id)
    {
        var image = await _db.TrainingImages.FindAsync(id);
        if (image == null)
            return NotFound();

        return File(image.Data, image.ContentType, image.FileName);
    }

    // ─── POST: Upload een training afbeelding met bounding boxes ───────
    [HttpPost("upload")]
    [EnableRateLimiting("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Upload(
        [FromForm] IFormFile image,
        [FromForm] string? boundingBoxes)
    {
        if (image == null || image.Length == 0)
            return BadRequest("Geen afbeelding ontvangen.");

        const long maxBytes = 15 * 1024 * 1024;
        if (image.Length > maxBytes)
            return BadRequest("Afbeelding is te groot. Maximum is 15 MB.");

        var toegestaneTypes = new[] { "image/jpeg", "image/png", "image/heic", "image/heif" };
        var toegestaneExts  = new[] { ".jpg", ".jpeg", ".png", ".heic", ".heif" };
        var ext = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (!toegestaneTypes.Contains(image.ContentType.ToLowerInvariant()) || !toegestaneExts.Contains(ext))
            return BadRequest("Alleen JPG, PNG en HEIC bestanden zijn toegestaan.");

        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);

        var trainingImage = new TrainingImage
        {
            FileName = image.FileName,
            ContentType = image.ContentType,
            Data = ms.ToArray(),
            UploadedAt = DateTime.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(boundingBoxes))
        {
            var boxes = JsonSerializer.Deserialize<List<BoundingBoxDto>>(
                boundingBoxes,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (boxes != null)
            {
                trainingImage.BoundingBoxes = boxes.Select(b => new BoundingBox
                {
                    Label = b.Label,
                    CenterX = b.CenterX,
                    CenterY = b.CenterY,
                    Width = b.Width,
                    Height = b.Height
                }).ToList();
            }
        }

        _db.TrainingImages.Add(trainingImage);
        await _db.SaveChangesAsync();

        return Ok(new { trainingImage.Id, trainingImage.FileName, trainingImage.UploadedAt });
    }
}

public record BoundingBoxDto(string Label, float CenterX, float CenterY, float Width, float Height);
