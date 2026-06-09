namespace AIAPI.Services;

public interface IGeocodingService
{
    Task<string?> GetAddressAsync(double latitude, double longitude);
}
