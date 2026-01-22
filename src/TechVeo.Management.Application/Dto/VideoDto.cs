using System;

namespace TechVeo.Management.Application.Dto;

public record VideoDto(Guid id, string path, double? intervalSeconds, int width, int height);
