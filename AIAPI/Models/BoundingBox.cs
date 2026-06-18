using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIAPI.Models;

public class BoundingBox
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TrainingImageId { get; set; }

    [ForeignKey(nameof(TrainingImageId))]
    public TrainingImage? TrainingImage { get; set; }

    [Required]
    public string Label { get; set; } = string.Empty;

    // YOLO-formaat: genormaliseerd 0–1
    public float CenterX { get; set; }
    public float CenterY { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}
