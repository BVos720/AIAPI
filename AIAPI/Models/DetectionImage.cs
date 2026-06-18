using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIAPI.Models;

// Aparte tabel voor de afbeeldingsdata (blob), zodat de Detections-tabel
// klein en snel blijft. Optioneel: niet elke detectie heeft een afbeelding.
public class DetectionImage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int DetectionId { get; set; }

    [ForeignKey(nameof(DetectionId))]
    public Detection? Detection { get; set; }

    [Required]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public byte[] Data { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
