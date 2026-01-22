namespace TechVeo.Management.Application.Dto;

public record VideoDto(Guid id, string? fileName, double? intervalSeconds, int width, int height);
