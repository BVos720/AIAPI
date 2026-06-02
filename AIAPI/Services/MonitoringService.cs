using System.Text;
using System.Text.Json;
using AIAPI.Models;

namespace AIAPI.Services;

public class MonitoringService(HttpClient httpClient, IConfiguration configuration, ILogger<MonitoringService> logger) : IMonitoringService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _aiModelUrl = configuration["AiModel:Url"] ?? throw new InvalidOperationException("AiModel:Url not configured.");
    private readonly string _monitoringUrl = configuration["Monitoring:Url"] ?? throw new InvalidOperationException("Monitoring:Url not configured.");
    private readonly ILogger<MonitoringService> _logger = logger;

    public async Task<AiDetectionResult> AnalyzeImageAsync(string imagePath)
    {
        await using var stream = File.OpenRead(imagePath);
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(stream), "image", Path.GetFileName(imagePath));

        var response = await _httpClient.PostAsync(_aiModelUrl, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AiDetectionResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Invalid response from AI model.");

        return result;
    }

    public async Task ForwardDetectionAsync(Detection detection)
    {
        var json = JsonSerializer.Serialize(detection);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(_monitoringUrl, content);
            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Monitoring team returned {StatusCode} for detection {Id}.", response.StatusCode, detection.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to forward detection {Id} to monitoring team.", detection.Id);
        }
    }
}
