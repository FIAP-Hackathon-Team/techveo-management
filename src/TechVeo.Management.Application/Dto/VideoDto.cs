using TechVeo.Management.Domain.Enums;

namespace TechVeo.Management.Application.Dto;

public record VideoDto(
    Guid Id,
    Status Status,
    string? FileName,
    double? IntervalSeconds,
    int? SnapshotCount,
    int Width,
    int Height);
