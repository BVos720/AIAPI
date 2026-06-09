using System.Text.Json;

namespace AIAPI.Services;

public class GeocodingService(HttpClient httpClient, IConfiguration configuration, ILogger<GeocodingService> logger) : IGeocodingService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = configuration["Geocoding:ApiKey"] ?? throw new InvalidOperationException("Geocoding:ApiKey not configured.");
    private readonly ILogger<GeocodingService> _logger = logger;

    public async Task<string?> GetAddressAsync(double latitude, double longitude)
    {
        var url = $"https://geocode.maps.co/reverse?lat={latitude}&lon={longitude}&api_key={_apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("display_name", out var displayName))
            {
                _logger.LogWarning("Geocoding API returned no display_name for ({Lat},{Lng}).", latitude, longitude);
                return null;
            }

            return displayName.GetString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to geocode coordinates ({Lat},{Lng}).", latitude, longitude);
            return null;
        }
    }
}
