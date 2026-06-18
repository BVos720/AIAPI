using Microsoft.EntityFrameworkCore;
using AIAPI.Models;

namespace AIAPI.Data;

public class SensoringDbContext(DbContextOptions<SensoringDbContext> options) : DbContext(options)
{
    public DbSet<Detection> Detections { get; set; }
    public DbSet<TrainingImage> TrainingImages { get; set; }
    public DbSet<BoundingBox> BoundingBoxes { get; set; }
}
