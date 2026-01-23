using Microsoft.AspNetCore.Http;

namespace TechVeo.Management.Contracts.Managements;

public record UploadVideoRequest(
    IFormFile File,
    int? SnapshotCount,
    double? IntervalSeconds,
    int Width,
    int Height);
