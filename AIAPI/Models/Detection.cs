using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIAPI.Models;

public class Detection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Label { get; set; } = string.Empty;

    public float Confidence { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Locatie als adres (optioneel)
    public string? Location { get; set; }

    // GPS coördinaten
    public double? LocatieX { get; set; }
    public double? LocatieY { get; set; }

    [Required]
    public string CameraId { get; set; } = string.Empty;

    public string? ImagePath { get; set; }

    // Bounding box
    public float? BoundingBoxLB { get; set; }
    public float? BoundingBoxRB { get; set; }
    public float? BoundingBoxCenter { get; set; }
}
