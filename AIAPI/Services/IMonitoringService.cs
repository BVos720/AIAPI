using AIAPI.Models;

namespace AIAPI.Services;

public record AiDetectionResult(string Label, float Confidence);

public interface IMonitoringService
{
    Task<AiDetectionResult> AnalyzeImageAsync(string imagePath);
    Task ForwardDetectionAsync(Detection detection);
}
