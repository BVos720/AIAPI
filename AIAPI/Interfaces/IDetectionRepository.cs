using AIAPI.Models;

namespace AIAPI.Interfaces;

public interface IDetectionRepository
{
    Task<Detection> SelectAsync(int id);
    Task<List<Detection>> GetAllAsync();
    Task<List<Detection>> GetByCameraAsync(string cameraId);
    Task InsertAsync(Detection detection);
    Task UpdateAsync(Detection detection);
    Task DeleteAsync(int id);
}
