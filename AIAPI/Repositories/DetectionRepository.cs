using Microsoft.EntityFrameworkCore;
using AIAPI.Data;
using AIAPI.Interfaces;
using AIAPI.Models;

namespace AIAPI.Repositories;

public class DetectionRepository(SensoringDbContext db) : IDetectionRepository
{
    private readonly SensoringDbContext _db = db;

    public async Task<Detection> SelectAsync(int id)
    {
        return await _db.Detections.FindAsync(id)
            ?? throw new KeyNotFoundException($"Detection with id {id} not found.");
    }

    public Task<List<Detection>> GetAllAsync()
    {
        return _db.Detections.OrderByDescending(d => d.Timestamp).ToListAsync();
    }

    public Task<List<Detection>> GetByCameraAsync(string cameraId)
    {
        return _db.Detections
            .Where(d => d.CameraId == cameraId)
            .OrderByDescending(d => d.Timestamp)
            .ToListAsync();
    }

    public async Task InsertAsync(Detection detection)
    {
        _db.Detections.Add(detection);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Detection detection)
    {
        _db.Detections.Update(detection);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var detection = await SelectAsync(id);
        _db.Detections.Remove(detection);
        await _db.SaveChangesAsync();
    }
}
