using AIAPI.Interfaces;
using AIAPI.Models;

namespace AIAPI.Repositories;

public class MockDetectionRepository : IDetectionRepository
{
    private readonly List<Detection> _detections =
    [
        new Detection { Id = 1, Label = "plastic", Confidence = 0.92f, Timestamp = DateTime.UtcNow.AddMinutes(-10), Location = "51.5872,4.7764", LocatieX = 51.5872, LocatieY = 4.7764, CameraId = "CAM-001", BoundingBoxLB = 0.1f, BoundingBoxRB = 0.4f, BoundingBoxCenter = 0.25f },
        new Detection { Id = 2, Label = "metal",   Confidence = 0.85f, Timestamp = DateTime.UtcNow.AddMinutes(-5),  Location = "51.5880,4.7770", LocatieX = 51.5880, LocatieY = 4.7770, CameraId = "CAM-001", BoundingBoxLB = 0.2f, BoundingBoxRB = 0.5f, BoundingBoxCenter = 0.35f },
        new Detection { Id = 3, Label = "paper",   Confidence = 0.78f, Timestamp = DateTime.UtcNow,                 Location = "51.5865,4.7750", LocatieX = 51.5865, LocatieY = 4.7750, CameraId = "CAM-002", BoundingBoxLB = 0.3f, BoundingBoxRB = 0.6f, BoundingBoxCenter = 0.45f },
    ];

    public Task<Detection> SelectAsync(int id)
    {
        var detection = _detections.FirstOrDefault(d => d.Id == id)
            ?? throw new KeyNotFoundException($"Detection with id {id} not found.");
        return Task.FromResult(detection);
    }

    public Task<List<Detection>> GetAllAsync()
        => Task.FromResult(_detections.OrderByDescending(d => d.Timestamp).ToList());

    public Task<List<Detection>> GetByCameraAsync(string cameraId)
        => Task.FromResult(_detections.Where(d => d.CameraId == cameraId).ToList());

    public Task InsertAsync(Detection detection)
    {
        detection.Id = _detections.Count + 1;
        _detections.Add(detection);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Detection detection)
    {
        var index = _detections.FindIndex(d => d.Id == detection.Id);
        if (index >= 0) _detections[index] = detection;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _detections.RemoveAll(d => d.Id == id);
        return Task.CompletedTask;
    }
}
